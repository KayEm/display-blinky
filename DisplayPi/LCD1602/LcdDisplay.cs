using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace DisplayPi.LCD1602
{
    public sealed partial class LcdDisplay
    {
        public int DisplayControl { get; private set; }
        public int DisplayFunction { get; private set; }
        public int DisplayMode { get; private set; }


        public IList<GpioPin> Data { get; set; } = new List<GpioPin>();
        public GpioPin RS { get; set; }
        public GpioPin E { get; private set; }

        public bool InitGpio(int rs, int e, [ReadOnlyArray] int[] data)
        {
            Debug.WriteLine("Init");

            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                return false;
            }

            RS = gpio.OpenPin(rs);
            RS.Write(GpioPinValue.Low);
            RS.SetDriveMode(GpioPinDriveMode.Output);

            E = gpio.OpenPin(e);
            E.Write(GpioPinValue.Low);
            E.SetDriveMode(GpioPinDriveMode.Output);

            foreach (var item in data)
            {
                var pin = gpio.OpenPin(item);
                pin.Write(GpioPinValue.Low);
                pin.SetDriveMode(GpioPinDriveMode.Output);
                Data.Add(pin);
            }

            Write4Bits(0x33);
            Write4Bits(0x32);
            Write4Bits(0x28);
            Write4Bits(0x0C);
            Write4Bits(0x06);

            DisplayControl = LCD_DISPLAYON | LCD_CURSOROFF | LCD_BLINKOFF;
            DisplayFunction = LCD_4BITMODE | LCD_1LINE | LCD_5x8DOTS;
            DisplayFunction |= LCD_2LINE;
            DisplayMode = LCD_ENTRYLEFT | LCD_ENTRYSHIFTDECREMENT;

            Write4Bits((LCD_ENTRYMODESET | DisplayMode));

            Clear();

            return RS != null && E != null && Data.All(x => x != null);
        }

        public void Clear()
        {
            Debug.WriteLine("Clear");

            Write4Bits(LCD_CLEARDISPLAY);
            Task.Delay(TimeSpan.FromMilliseconds(3)).Wait();
        }

        public void NoDisplay()
        {
            Debug.WriteLine("NoDisplay");

            DisplayControl &= ~LCD_DISPLAYON;
            Write4Bits(LCD_DISPLAYCONTROL | DisplayControl);
        }

        public void Display()
        {
            Debug.WriteLine("Display");

            DisplayControl |= LCD_DISPLAYON;
            Write4Bits(LCD_DISPLAYCONTROL | DisplayControl);
        }

        private void Write4Bits(int value, bool charMode = false)
        {
            Task.Delay(TimeSpan.FromMilliseconds(1)).Wait();

            var stringBits = string.Format("{0:D8}", int.Parse(Convert.ToString(value, 2))).ToCharArray();

            RS.Write(charMode ? GpioPinValue.High : GpioPinValue.Low);

            Reset4BitDataBuffer();

            for (var i = 0; i < 4; i++)
            {
                if (stringBits[i] == '1')
                {
                    Data[i].Write(GpioPinValue.High);
                }
            }

            PulseEnable();

            Reset4BitDataBuffer();

            for (var i = 4; i < 8; i++)
            {
                if (stringBits[i] == '1')
                {
                    Data[i - 4].Write(GpioPinValue.High);
                }
            }

            PulseEnable();
        }

        public void SendMessage(string textMessage)
        {
            foreach (var @char in textMessage.ToCharArray())
            {
                if (@char == '\n')
                {
                    Write4Bits(0xC0); //next line
                }
                else
                {
                    Write4Bits(@char, charMode: true);
                }
            }
        }

        private void Reset4BitDataBuffer()
        {
            foreach (var t in Data)
            {
                t.Write(GpioPinValue.Low);
            }
        }

        private void PulseEnable()
        {
            E.Write(GpioPinValue.Low);
            Task.Delay(TimeSpan.FromMilliseconds(0.1)).Wait();
            E.Write(GpioPinValue.High);
            Task.Delay(TimeSpan.FromMilliseconds(0.1)).Wait();
            E.Write(GpioPinValue.Low);
            Task.Delay(TimeSpan.FromMilliseconds(0.1)).Wait();
        }
    }
}

