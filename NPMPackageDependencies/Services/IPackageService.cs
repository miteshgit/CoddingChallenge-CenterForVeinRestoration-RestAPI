using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPMPackageDependencies.Services
{
    public interface IPackageService
    {
        public Task<string[]> GetAllPackageDependenciesByName(string packageName);
    }
}
