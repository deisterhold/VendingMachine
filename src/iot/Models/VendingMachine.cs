using Iot.Device.Pca9685;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Threading.Tasks;

namespace Vending.Iot.Models
{
    public enum VendingMachineColumn
    {
        Left = 13,
        Middle = 14,
        Right = 15
    }

    public class VendingMachine : IDisposable
    {
        private I2cDevice _device;
        private Pca9685 _pwm;

        public VendingMachine()
        {
            const int busId = 1; // /dev/i2c-1
            const int deviceAddressFixed = 0x40;
            const int deviceAddressSelectable = 0b000000; // A5 A4 A3 A2 A1 A0
            const int deviceAddress = deviceAddressFixed | deviceAddressSelectable;

            var settings = new I2cConnectionSettings(busId, deviceAddress);

            _device = I2cDevice.Create(settings);
            _pwm = new Pca9685(_device) { PwmFrequency = 60000 };
        }

        public async Task Vend(VendingMachineColumn column, TimeSpan duration)
        {
            _pwm.SetPwm(0, 120, (int)column);
            await Task.Delay(duration);
            _pwm.SetPwm(0, 0, (int)column);
        }

        public async Task Vend(VendingMachineColumn column, int on, int off, TimeSpan duration)
        {
            _pwm.SetPwm(0, 120, (int)column);
            await Task.Delay(duration);
            _pwm.SetPwm(0, 0, (int)column);
        }

        public void Dispose()
        {
            _device?.Dispose();
            _device = null;

            _pwm?.Dispose();
            _pwm = null;
        }
    }
}
