using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.I2c;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.Pca9685;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Vending.Iot
{
    public class Program
    {
        const int INPUT_1 = 26;
        const int INPUT_2 = 20;
        const int INPUT_3 = 21;

        public static void Main(string[] args)
        {
            const int busId = 1;
            const int deviceAddressFixed = 0x40;
            const int deviceAddressSelectable = 0b000000; // A5 A4 A3 A2 A1 A0
            const int deviceAddress = deviceAddressFixed | deviceAddressSelectable;

            var settings = new I2cConnectionSettings(busId, deviceAddress);

            using (var i2cDevice = I2cDevice.Create(settings))
            using (var pwm = new Pca9685(i2cDevice))
            using (var controller = new GpioController(PinNumberingScheme.Logical))
            {
                controller.OpenPin(INPUT_1, PinMode.InputPullUp);
                controller.OpenPin(INPUT_2, PinMode.InputPullUp);
                controller.OpenPin(INPUT_3, PinMode.InputPullUp);

                PinChangeEventHandler callback = (object sender, PinValueChangedEventArgs args) => {
                    if (args.ChangeType == PinEventTypes.Rising)
                    {
                        switch (args.PinNumber)
                        {
                            case INPUT_1:
                                pwm.SetPwm(0, 120, 13);
                                break;
                            case INPUT_2:
                                pwm.SetPwm(0, 120, 14);
                                break;
                            case INPUT_3:
                                pwm.SetPwm(0, 120, 15);
                                break;
                            default:
                                Console.WriteLine("Unknown Rising Pin: {0}", args.PinNumber);
                                break;
                        }
                    }
                    else
                    {
                        switch (args.PinNumber)
                        {
                            case INPUT_1:
                                pwm.SetPwm(0, 0, 13);
                                break;
                            case INPUT_2:
                                pwm.SetPwm(0, 0, 14);
                                break;
                            case INPUT_3:
                                pwm.SetPwm(0, 0, 15);
                                break;
                            default:
                                Console.WriteLine("Unknown Falling Pin: {0}", args.PinNumber);
                                break;
                        }
                    }
                };

                controller.RegisterCallbackForPinValueChangedEvent(INPUT_1, PinEventTypes.Falling | PinEventTypes.Rising, callback);
                controller.RegisterCallbackForPinValueChangedEvent(INPUT_2, PinEventTypes.Falling | PinEventTypes.Rising, callback);
                controller.RegisterCallbackForPinValueChangedEvent(INPUT_3, PinEventTypes.Falling | PinEventTypes.Rising, callback);

                while (true)
                {
                    Thread.Sleep(60 * 1000);
                }
            }

            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
