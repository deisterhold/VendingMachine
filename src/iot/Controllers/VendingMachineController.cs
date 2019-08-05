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

        [HttpPost("{column:int}")]
        public void Vend(VendingMachineColumn column)
        {
            using (var machine = new VendingMachine())
            {

            }
        }
    }
}
