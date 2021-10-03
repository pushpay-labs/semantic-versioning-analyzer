using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Pushpay.SemVerAnalyzer.Nuget;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public class FakeNugetClient : INugetClient
	{
		readonly AppSettings _settings;

		public FakeNugetClient(AppSettings settings)
		{
			_settings = settings;
		}

		public Task<byte[]> GetAssemblyBytesFromPackage(string packageName, string fileName, string framework, List<string> comments)
		{
			if (_settings.Framework == "net5.0")
			{
				comments.Add("you will find nothing for .net 5");
				return null;
			}

			var bytes = File.ReadAllBytes($"{packageName}.dll");
			return Task.FromResult(bytes);
		}
	}
}
