using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Pushpay.SemVerAnalyzer.Nuget;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public class FakeNugetClient : INugetClient
	{
		public Task<byte[]> GetAssemblyBytesFromPackage(string packageName, string fileName, List<string> comments)
		{
			var bytes = File.ReadAllBytes($"{packageName}.dll");
			return Task.FromResult(bytes);
		}
	}
}
