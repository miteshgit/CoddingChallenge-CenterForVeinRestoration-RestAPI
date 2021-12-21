using Newtonsoft.Json;
using NPMPackageDependencies.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NPMPackageDependencies.Services
{
    public class PackageService : IPackageService
    {
        private readonly HttpClient client;
        private List<string> allPackages;
        public PackageService()
        {
            client = new HttpClient()
            {
                BaseAddress = new Uri("http://registry.npmjs.org")
            };
            allPackages = new List<string>();
        }

        public async Task<string[]> GetAllPackageDependenciesByName(string packageName)
        {
            await GetNPMPackageList(packageName);
            return allPackages.Distinct().OrderBy(o => o).ToArray();
        }

        private async Task GetNPMPackageList(string packageName)
        {
            var url = string.Format($"/{packageName}/latest");
            var response = await client.GetAsync(url);
            var stringResponse = response.Content.ReadAsStringAsync().Result;

            var packageData = JsonConvert.DeserializeObject<PackageData>(stringResponse);

            if (packageData.dependencies != null && packageData.dependencies.ToString() != "{}")
            {
                var dependencies = JsonConvert.DeserializeObject<Dictionary<string, string>>(packageData.dependencies.ToString());
                Parallel.ForEach(dependencies.Keys.ToList(), async dependency => await GetNPMPackageList(dependency.ToString()));
            }
            else
            {
                allPackages.Add(packageName);
            }
        }       
    }
}
