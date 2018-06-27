using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Report2017.Controllers
{
    [Route("api/[controller]")]
    public class IsAliveController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new IsAliveResponse
            {
                Name = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationName,
                Version = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion,
#if DEBUG
                IsDebug = true,
#else
                IsDebug = false,
#endif
            });
        }
    }
}
