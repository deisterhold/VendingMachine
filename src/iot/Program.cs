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
            using (var controller = new GpioController(PinNumberingScheme.Logical))
            {
                controller.OpenPin(INPUT_1, PinMode.InputPullUp);
                controller.OpenPin(INPUT_2, PinMode.InputPullUp);
                controller.OpenPin(INPUT_3, PinMode.InputPullUp);

                PinChangeEventHandler callback = (object sender, PinValueChangedEventArgs args) => {
                    Console.WriteLine("Pin: {0}, Change Type: {0}", args.PinNumber, args.ChangeType);
                };

                controller.RegisterCallbackForPinValueChangedEvent(INPUT_1, PinEventTypes.Falling, callback);
                controller.RegisterCallbackForPinValueChangedEvent(INPUT_2, PinEventTypes.Falling, callback);
                controller.RegisterCallbackForPinValueChangedEvent(INPUT_3, PinEventTypes.Falling, callback);

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
