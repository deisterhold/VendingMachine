﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.I2c;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.Pca9685;
using System.Runtime.InteropServices;

namespace Vending.Iot
{
    public interface IHardwareAccess : IDisposable
    {
        I2cDevice I2cDevice { get; }
        Pca9685 Pwm { get; }
        GpioController Gpio { get; }
    }

    public class HardwareAccess : IDisposable, IHardwareAccess
    {
        #region I2C Configuration

        const int busId = 1;
        const int deviceAddressFixed = 0x40;
        const int deviceAddressSelectable = 0b000000; // A5 A4 A3 A2 A1 A0
        const int deviceAddress = deviceAddressFixed | deviceAddressSelectable;

        #endregion

        #region Button Configuration

        public const int INPUT_1 = 26;
        public const int INPUT_2 = 20;
        public const int INPUT_3 = 21;

        #endregion

        #region Motor Configuration

        public const int MOTOR_1 = 13;
        public const int MOTOR_2 = 14;
        public const int MOTOR_3 = 15;

        #endregion

        #region IHardwareAccess

        public I2cDevice I2cDevice { get; private set; }
        public Pca9685 Pwm { get; private set; }
        public GpioController Gpio { get; private set; }

        #endregion

        #region Constructors

        public HardwareAccess()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Setup PWM Controller
                var settings = new I2cConnectionSettings(busId, deviceAddress);

                I2cDevice = I2cDevice.Create(settings);
                Pwm = new Pca9685(I2cDevice);

                // Setup the pins for the buttons
                Gpio = new GpioController(PinNumberingScheme.Logical);
            }
        }
        
        #endregion

        #region IDisposable

        public void Dispose()
        {
            I2cDevice?.Dispose();
            I2cDevice = null;
            Pwm?.Dispose();
            Pwm = null;
            Gpio?.Dispose();
            Gpio = null;
        }

        #endregion
    }

    public class VendingService : IHostedService, IDisposable
    {
        public readonly IHardwareAccess Hardware;

        public VendingService(IHardwareAccess hardwareAccess)
        {
            Hardware = hardwareAccess;
        }

        #region IHostedService

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting Vending Service...");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Setup the pins for the buttons
                Hardware.Gpio.OpenPin(HardwareAccess.INPUT_1, PinMode.InputPullUp);
                Hardware.Gpio.OpenPin(HardwareAccess.INPUT_2, PinMode.InputPullUp);
                Hardware.Gpio.OpenPin(HardwareAccess.INPUT_3, PinMode.InputPullUp);

                // Register Event Listeners
                Hardware.Gpio.RegisterCallbackForPinValueChangedEvent(HardwareAccess.INPUT_1, PinEventTypes.Falling, PinFalling);
                Hardware.Gpio.RegisterCallbackForPinValueChangedEvent(HardwareAccess.INPUT_2, PinEventTypes.Falling, PinFalling);
                Hardware.Gpio.RegisterCallbackForPinValueChangedEvent(HardwareAccess.INPUT_3, PinEventTypes.Falling, PinFalling);
                Hardware.Gpio.RegisterCallbackForPinValueChangedEvent(HardwareAccess.INPUT_1, PinEventTypes.Rising, PinRising);
                Hardware.Gpio.RegisterCallbackForPinValueChangedEvent(HardwareAccess.INPUT_2, PinEventTypes.Rising, PinRising);
                Hardware.Gpio.RegisterCallbackForPinValueChangedEvent(HardwareAccess.INPUT_3, PinEventTypes.Rising, PinRising);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping Vending Service...");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Unregister Event Listeners
                Hardware.Gpio.UnregisterCallbackForPinValueChangedEvent(HardwareAccess.INPUT_1, PinFalling);
                Hardware.Gpio.UnregisterCallbackForPinValueChangedEvent(HardwareAccess.INPUT_2, PinFalling);
                Hardware.Gpio.UnregisterCallbackForPinValueChangedEvent(HardwareAccess.INPUT_3, PinFalling);
                Hardware.Gpio.UnregisterCallbackForPinValueChangedEvent(HardwareAccess.INPUT_1, PinRising);
                Hardware.Gpio.UnregisterCallbackForPinValueChangedEvent(HardwareAccess.INPUT_2, PinRising);
                Hardware.Gpio.UnregisterCallbackForPinValueChangedEvent(HardwareAccess.INPUT_3, PinRising);
            }

            return Task.CompletedTask;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Hardware?.Dispose();
        }

        #endregion

        #region Event Listeners

        private void PinRising(object sender, PinValueChangedEventArgs args)
        {
            Console.WriteLine("Falling Pin: {0}", args.PinNumber);

            switch (args.PinNumber)
            {
                case HardwareAccess.INPUT_1:
                    Hardware.Pwm.SetPwm(0, 120, HardwareAccess.MOTOR_1);
                    break;
                case HardwareAccess.INPUT_2:
                    Hardware.Pwm.SetPwm(0, 120, HardwareAccess.MOTOR_2);
                    break;
                case HardwareAccess.INPUT_3:
                    Hardware.Pwm.SetPwm(0, 120, HardwareAccess.MOTOR_3);
                    break;
                default:
                    Console.WriteLine("Unknown Rising Pin: {0}", args.PinNumber);
                    break;
            }
        }

        private void PinFalling(object sender, PinValueChangedEventArgs args)
        {
            Console.WriteLine("Rising Pin: {0}", args.PinNumber);

            switch (args.PinNumber)
            {
                case HardwareAccess.INPUT_1:
                    Hardware.Pwm.SetPwm(0, 0, HardwareAccess.MOTOR_1);
                    break;
                case HardwareAccess.INPUT_2:
                    Hardware.Pwm.SetPwm(0, 0, HardwareAccess.MOTOR_2);
                    break;
                case HardwareAccess.INPUT_3:
                    Hardware.Pwm.SetPwm(0, 0, HardwareAccess.MOTOR_3);
                    break;
                default:
                    Console.WriteLine("Unknown Falling Pin: {0}", args.PinNumber);
                    break;
            }
        }

        #endregion
    }
}
