using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Vending.Iot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendingMachineController : ControllerBase
    {
        private readonly ILogger<VendingMachineController> _logger;
        private readonly IHardwareAccess _hardware;

        public VendingMachineController(ILogger<VendingMachineController> logger, IHardwareAccess hardware)
        {
            _logger = logger;
            _hardware = hardware;
        }

        [HttpGet("vend/{column:int:min(0):max(15)}/{duration:min(1):max(5000)}")]
        public async Task<IActionResult> Vend(int column, int duration)
        {
            _hardware.Pwm?.SetPwm(0, 120, column);
            await Task.Delay(duration);
            _hardware.Pwm?.SetPwm(0, 0, column);

            return Ok();
        }

        [HttpGet("vend/{column:int:min(0):max(15)}/{on:int}/{off:int}/{duration:min(1):max(5000)}")]
        public async Task<IActionResult> Vend(int column, int on, int off, int duration)
        {
            _hardware.Pwm?.SetPwm(on, off, column);
            await Task.Delay(duration);
            _hardware.Pwm?.SetPwm(0, 0, column);

            return Ok();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(Request.Scheme + "://" + Request.Host + "/" + Request.PathBase);
        }
    }
}
