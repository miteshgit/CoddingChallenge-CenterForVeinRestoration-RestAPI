using Newtonsoft.Json;
using NPMPackageDependencies.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public Task<string[]> GetAllPackageDependenciesByName(string packageName)
        {
            GetNPMPackageList(packageName);
            return Task.FromResult(allPackages.Distinct().OrderBy(o => o).ToArray());
        }

        private void GetNPMPackageList(string packageName)
        {
            var url = string.Format($"/{packageName}/latest");
            var response = client.GetAsync(url).Result;
            var stringResponse = response.Content.ReadAsStringAsync().Result;

            var packageData = JsonConvert.DeserializeObject<PackageData>(stringResponse);

            if (packageData.dependencies != null && packageData.dependencies.ToString() != "{}")
            {
                var dependencies = JsonConvert.DeserializeObject<Dictionary<string, string>>(packageData.dependencies.ToString());
                Parallel.ForEach(dependencies.Keys.ToList(), dependency => GetNPMPackageList(dependency.ToString()));
            }
            else
            {
                allPackages.Add(packageName);
            }
        }
    }
}
