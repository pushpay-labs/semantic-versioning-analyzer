using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Pushpay.SemVerAnalyzer.Nuget
{
	internal class NugetClient : INugetClient
	{
		static readonly HttpClient client = new HttpClient();
		static readonly ISettings nugetSettings = Settings.LoadDefaultSettings(null);

		readonly NugetConfiguration _config;
		readonly AppSettings _settings;

		public NugetClient(NugetConfiguration config, AppSettings settings)
		{
			_config = config;
			_settings = settings;
		}

		public async Task<byte[]> GetAssemblyBytesFromPackage(string packageName, string fileName, string framework, List<string> comments)
		{
			try
			{
				var packageSource = _config.PackageSource != null
					? SettingsUtility.GetEnabledSources(nugetSettings).FirstOrDefault(s => s.Name == _config.PackageSource)
					: new PackageSource(new Uri(new Uri(_config.RepositoryUrl), "v3/index.json").AbsoluteUri);
				if (packageSource == null){
					comments.Add($"The NuGet source '{_config.PackageSource}' was not found.");
					return null;
				}
				
				var repository = Repository.Factory.GetCoreV3(packageSource);

				var metadataResource = await repository.GetResourceAsync<PackageMetadataResource>();
				var cacheContext = new SourceCacheContext();
				var feedVersions = await metadataResource.GetMetadataAsync(
					packageName,
					includePrerelease: false,
					includeUnlisted: false,
					cacheContext,
					NullLogger.Instance,
					CancellationToken.None);
				var highestVersion = feedVersions.MaxBy(m => m.Identity.Version);
				if (highestVersion == null)
				{
					comments.Add($"Error retrieving Nuget package:\nNo versions found.");
					return null;
				}

				var downloadResource = await repository.GetResourceAsync<DownloadResource>();
				var download = await downloadResource.GetDownloadResourceResultAsync(
					highestVersion.Identity,
					new PackageDownloadContext(cacheContext),
					SettingsUtility.GetGlobalPackagesFolder(nugetSettings),
					NullLogger.Instance,
					CancellationToken.None);
				if (download.Status != DownloadResourceResultStatus.Available)
				{
					comments.Add($"Error retrieving Nuget package:\n{download.Status}");
					return null;
				}

				using PackageReaderBase packageReader = download.PackageReader;
				var libItems = await packageReader.GetLibItemsAsync(CancellationToken.None);
				IEnumerable<string> frameworkLibItems;
				if (!string.IsNullOrEmpty(_settings.Framework))
				{
					var targetFramework = NuGetFramework.Parse(_settings.Framework);
					frameworkLibItems = libItems.FirstOrDefault(g => g.TargetFramework == targetFramework)?.Items;
				}
				else
				{
					var targetFramework = !string.IsNullOrEmpty(framework) ? NuGetFramework.Parse(framework) : null;
					var frameworkSpecificGroups = libItems as FrameworkSpecificGroup[] ?? libItems.ToArray();
					frameworkLibItems = frameworkSpecificGroups.FirstOrDefault(g => g.TargetFramework == targetFramework)?.Items
					                    ?? frameworkSpecificGroups.SelectMany(g => g.Items);
				}

				var entry = frameworkLibItems?.FirstOrDefault(i => i.EndsWith($"{fileName}.dll"));
				if (entry == null)
				{
					comments.Add("Found NuGet package, but could not find DLL within it.");
					return null;
				}

				using MemoryStream ms = new MemoryStream();
				var input = await packageReader.GetStreamAsync(entry, CancellationToken.None);
				await input.CopyToAsync(ms);
				return ms.ToArray();
			}
			catch (HttpRequestException e)
			{
				comments.Add($"Error retrieving Nuget package:\n{e.Message}");
				return null;
			}
		}
	}
}
