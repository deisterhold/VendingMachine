using System;
using System.Device.I2c;
using System.Device.I2c.Drivers;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Iot.Device.Pca9685;
using VendingMachine.Service;

namespace VendingMachine.Iot
{
    class VendingMachineImpl : Service.VendingMachine.VendingMachineBase
    {
        public override Task<VendResponse> VendItem(VendRequest request, ServerCallContext context)
        {
            var pwmTask = new Task<UnixI2cDevice>(() =>
                {
                    const int busId = 1; // /dev/i2c-1
                    const int deviceAddressFixed = 0x40;
                    const int deviceAddressSelectable = 0b000000; // A5 A4 A3 A2 A1 A0
                    const int deviceAddress = deviceAddressFixed | deviceAddressSelectable;

                    var settings = new I2cConnectionSettings(busId, deviceAddress);

                    return new UnixI2cDevice(settings);
                }).ContinueWith(async (t) =>
                {
                    using var pca9685 = new Pca9685(t.Result) {PwmFrequency = 60000};
                    pca9685.SetPwm(0, 120);
                    await Task.Delay(1000);
                    pca9685.SetPwm(0, 0);
                    return new VendResponse {Quantity = request.Quantity};
                })
                .Unwrap();

            return pwmTask;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 1 && int.TryParse(args[0], out var index))
            {
                var impl = new VendingMachineImpl();
                await impl.VendItem(new VendRequest {Quantity = index}, null);
            }
        }
    }
}
