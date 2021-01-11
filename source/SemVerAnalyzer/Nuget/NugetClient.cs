using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Pushpay.SemVerAnalyzer.Engine;

namespace Pushpay.SemVerAnalyzer.Nuget
{
	internal class NugetClient : INugetClient
	{
		readonly NugetConfiguration _config;

		public NugetClient(NugetConfiguration config)
		{
			_config = config;
		}

		public async Task<byte[]> GetAssemblyBytesFromPackage(string packageName, string fileName, List<string> comments)
		{
			try
			{
				using var client = new HttpClient();
				
				using var feedRequest = new HttpRequestMessage(HttpMethod.Get, Path.Combine(_config.RepositoryUrl, "v3", "index.json"));
				using var feedResponse = await client.SendAsync(feedRequest);
				if (!feedResponse.IsSuccessStatusCode)
				{
					var content = await feedResponse.Content.ReadAsStringAsync();
					comments.Add($"Error retrieving Nuget feed:\n{content}");
					return null;
				}

				var feed = JsonSerializer.Deserialize<NugetPackageFeed>(await feedResponse.Content.ReadAsStringAsync());
				var packageBaseAddress = feed.Resources.Single(r => r.Type.StartsWith("PackageBaseAddress")).Id;
				
				using var versionRequest = new HttpRequestMessage(HttpMethod.Get, Path.Combine(packageBaseAddress, packageName, "index.json"));
				using var versionResponse = await client.SendAsync(versionRequest);
				if (!versionResponse.IsSuccessStatusCode)
				{
					var content = await versionResponse.Content.ReadAsStringAsync();
					comments.Add($"Error retrieving package versions:\n{content}");
					return null;
				}

				var versions = JsonSerializer.Deserialize<VersionsFeed>(await versionResponse.Content.ReadAsStringAsync()).Versions;
				var highestVersion = versions.OrderByDescending(v => v.MajorVersion()).ThenByDescending(v => v.MinorVersion()).ThenByDescending(v => v.PatchVersion()).First();

				using var request = new HttpRequestMessage(HttpMethod.Get, Path.Join(packageBaseAddress, packageName, highestVersion, $"{packageName.ToLower()}.{highestVersion}.nupkg"));
				using var response = await client.SendAsync(request);
				if (!response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					comments.Add($"Error retrieving Nuget package:\n{content}");
					return null;
				}

				using var archive = new ZipArchive(await response.Content.ReadAsStreamAsync());
				await using var unzippedEntryStream = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith($"{fileName}.dll"))?.Open();
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
