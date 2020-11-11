using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pushpay.SemVerAnalyzer.Nuget
{
	internal class NugetClient : INugetClient
	{
		readonly NugetConfiguration _config;

		public NugetClient(NugetConfiguration config)
		{
			_config = config;
		}

		public async Task<byte[]> GetAssemblyBytesFromPackage(string packageName, List<string> comments)
		{

			try {
				using var client = new HttpClient();
				using var request = new HttpRequestMessage(HttpMethod.Get, Path.Join(_config.RepositoryUrl, packageName));
				using var response = await client.SendAsync(request);
				if (!response.IsSuccessStatusCode) {
					comments.Add("Error contacting ProGet.  If working outside of the office, please ensure you are on the VPN.");
					return null;
				}

				using var archive = new ZipArchive(await response.Content.ReadAsStreamAsync());
				await using var unzippedEntryStream = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith($"{packageName}.dll"))?.Open();
				if (unzippedEntryStream == null) {
					comments.Add("Found NuGet package, but could not find DLL within it.");
					return null;
				}

				return ReadAllBytes(unzippedEntryStream);
			} catch (HttpRequestException e) {
				comments.Add($"Error contacting ProGet.  If working outside of the office, please ensure you are on the VPN. {e.Message}");
				return null;
			}
		}

		static byte[] ReadAllBytes(Stream input)
		{
			var buffer = new byte[1 << 20];
			using var ms = new MemoryStream();
			int read;
			while ((read = input.Read(buffer, 0, buffer.Length)) > 0) {
				ms.Write(buffer, 0, read);
			}

			return ms.ToArray();
		}
	}
}
