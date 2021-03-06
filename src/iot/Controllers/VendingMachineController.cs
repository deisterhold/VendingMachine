﻿using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Vending.Iot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendingMachineController : ControllerBase
    {
        private readonly ILogger<VendingMachineController> _logger;
        private readonly IHardwareAccess _hardware;

        public VendingMachineController(ILogger<VendingMachineController> logger, IHardwareAccess hardware)
        {
            _logger = logger;
            _hardware = hardware;
        }

        [HttpGet("[action]/{column:int:min(0):max(15)}/{duration:min(1):max(5000)}")]
        public async Task<IActionResult> Vend(int column, int duration)
        {
            _hardware.Pwm?.SetPwm(0, 120, column);
            await Task.Delay(duration);
            _hardware.Pwm?.SetPwm(0, 0, column);

            return Ok();
        }

        [HttpGet("[action]/{column:int:min(0):max(15)}/{on:int}/{off:int}/{duration:min(1):max(5000)}")]
        public async Task<IActionResult> Vend(int column, int on, int off, int duration)
        {
            _hardware.Pwm?.SetPwm(on, off, column);
            await Task.Delay(duration);
            _hardware.Pwm?.SetPwm(0, 0, column);

            return Ok();
        }

        [HttpGet("[action]")]
        public IActionResult Lights()
        {
            return Ok(_hardware.IsLightOn);
        }

        [HttpPut("[action]")]
        public IActionResult Lights([FromBody] bool value)
        {
            if (value)
            {
                _hardware.LightsOn();
            }
            else
            {
                _hardware.LightsOff();
            }

            return Ok(value);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(Request.Scheme + "://" + Request.Host + "/" + Request.PathBase);
        }
    }
}
