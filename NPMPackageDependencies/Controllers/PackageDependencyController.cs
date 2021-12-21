using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPMPackageDependencies.Services;
using System;
using System.Threading.Tasks;

namespace NPMPackageDependencies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageDependencyController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageDependencyController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [Route("GetAllDependencies")]
        [HttpGet]
        public async Task<IActionResult> GetAsync(string packageName)
        {
            if (!string.IsNullOrWhiteSpace(packageName))
            {
                try
                {
                    var packages = await _packageService.GetAllPackageDependenciesByName(packageName);
                    return Ok(packages);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        ex.Message);
                }
            }
            else
            {
                return BadRequest("packageName is required.");
            }
        }
    }
}
