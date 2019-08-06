using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vending.Iot.Models;

namespace Vending.Iot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendingMachineController : ControllerBase
    {
        private readonly ILogger<VendingMachineController> _logger;

        public VendingMachineController(ILogger<VendingMachineController> logger)
        {
            _logger = logger;
        }

        [HttpGet("vend/{column:int}/{duration:min(1):max(5000)}")]
        public async Task<IActionResult> Vend(VendingMachineColumn column, int duration)
        {
            if (!Enum.IsDefined(typeof(VendingMachineColumn), column)) return BadRequest("Unknown column.");

            using (var machine = new VendingMachine())
            {
                await machine.Vend(column, TimeSpan.FromMilliseconds(duration));
                return Ok();
            }
        }
    }
}
