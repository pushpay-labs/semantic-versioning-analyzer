using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Pushpay.SemVerAnalyzer.Engine;

namespace Pushpay.SemVerAnalyzer.Nuget
{
	internal class NugetClient : INugetClient
	{
		static readonly HttpClient client = new HttpClient();
		readonly NugetConfiguration _config;
		readonly AppSettings _settings;

		public NugetClient(NugetConfiguration config, AppSettings settings)
		{
			_config = config;
			_settings = settings;
		}

		public async Task<byte[]> GetAssemblyBytesFromPackage(string packageName, string fileName, List<string> comments)
		{
			try
			{
				var feedMeta = await GetUrlContents<NugetPackageFeed>(Path.Combine(_config.RepositoryUrl, "v3", "index.json"));
				if (feedMeta.Success == false){
					comments.Add($"Error retrieving Nuget feed:\n{feedMeta.ErrorMessage}");
					return null;
				}
					
				var packageBaseAddress = feedMeta.Result.Resources.Single(r => r.Type.StartsWith("PackageBaseAddress")).Id;


				var feedVersions = await GetUrlContents<VersionsFeed>(Path.Combine(packageBaseAddress, packageName, "index.json"));
				if (feedVersions.Success == false){
					comments.Add($"Error retrieving package versions:\n{feedMeta.ErrorMessage}");
					return null;
				}

				var highestVersion = feedVersions.Result.Versions
					.Select(v => v.ToSemver())
					.OrderByDescending(v => v)
					.First()
					.ToString();

				using var request = new HttpRequestMessage(HttpMethod.Get, Path.Join(packageBaseAddress, packageName, highestVersion, $"{packageName.ToLower()}.{highestVersion}.nupkg"));
				using var response = await client.SendAsync(request);
				if (!response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					comments.Add($"Error retrieving Nuget package:\n{content}");
					return null;
				}

				using var archive = new ZipArchive(await response.Content.ReadAsStreamAsync());
				ZipArchiveEntry entry = string.IsNullOrEmpty(_settings.Framework)
					? archive.Entries.FirstOrDefault(e => e.FullName.EndsWith($"{fileName}.dll"))
					: archive.Entries.FirstOrDefault(e => e.FullName.Contains(_settings.Framework) && e.FullName.EndsWith($"{fileName}.dll"));
				await using var unzippedEntryStream = entry?.Open();
				if (unzippedEntryStream == null)
				{
					comments.Add("Found NuGet package, but could not find DLL within it.");
					return null;
				}

				return ReadAllBytes(unzippedEntryStream);
			}
			catch (HttpRequestException e)
			{
				comments.Add($"Error retrieving Nuget package:\n{e.Message}");
				return null;
			}
		}

		static byte[] ReadAllBytes(Stream input)
		{
			var buffer = new byte[1 << 20];
			using var ms = new MemoryStream();
			int read;
			while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				ms.Write(buffer, 0, read);
			}

			return ms.ToArray();
		}

		async Task<GetUrlContentsResult<T>> GetUrlContents<T>(string requestUri) where T : class {
			using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
			using var response = await client.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				return new GetUrlContentsResult<T> {
					Success = false,
					ErrorMessage = content,
				};
			}

			var responseContent = await response.Content.ReadAsStringAsync();
			return new GetUrlContentsResult<T> {
				Result = JsonSerializer.Deserialize<T>(responseContent),
				Success = true,
			};
		}
		
		class GetUrlContentsResult<T> where T : class {
			public T Result;
			public bool Success;
			public string ErrorMessage;
		}

		class NugetPackageFeed {
			[JsonPropertyName("resources")]
			public IEnumerable<Resource> Resources { get; set; }
		}

		class Resource {
			[JsonPropertyName("@id")]
			public string Id { get; set; }
			[JsonPropertyName("@type")]
			public string Type { get; set; }
		}

		class VersionsFeed {
			[JsonPropertyName("versions")]
			public IEnumerable<string> Versions { get; set; }
		}

	}

}
