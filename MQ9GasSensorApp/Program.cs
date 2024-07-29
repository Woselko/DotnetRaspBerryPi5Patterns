using System;
using System.Device.Gpio;
using System.Device.Spi;

class Program
{
    static void Main(string[] args)
    {
        // Ustawienia połączenia SPI
        var spiSettings = new SpiConnectionSettings(0, 0) // BusId 0, ChipSelect 0
        {
            ClockFrequency = 500000, // 500 KHz
            Mode = SpiMode.Mode0
        };

        using var spiDevice = SpiDevice.Create(spiSettings);

        // Chip Select (CS) na GPIO 5
        using var gpioController = new GpioController();
        int chipSelectPin = 25;
        gpioController.OpenPin(chipSelectPin, PinMode.Output);
        gpioController.Write(chipSelectPin, PinValue.High);

        while (true)
        {
            gpioController.Write(chipSelectPin, PinValue.Low);

            // Wysłanie polecenia odczytu dla kanału 0
            byte[] writeBuffer = new byte[3] { 0x18, 0x00, 0x00 }; // 0x18 = 00011000, które wybiera kanał 0
            byte[] readBuffer = new byte[3];

            spiDevice.TransferFullDuplex(writeBuffer, readBuffer);

            gpioController.Write(chipSelectPin, PinValue.High);

            // Przetwarzanie wyniku
            int result = ((readBuffer[1] & 0x0F) << 8) | readBuffer[2];
            Console.WriteLine($"Analogowa wartość z MQ-9: {result}");

            // Czekaj sekundę przed ponownym odczytem
            System.Threading.Thread.Sleep(1000);
        }
    }
}

// using System;
// using System.Device.Gpio;
// using System.Device.Spi;

// public class ADC0834
// {
//     private SpiDevice _spiDevice;
//     private GpioController _gpioController;
//     private int _chipSelectPin;

//     public ADC0834(SpiDevice spiDevice, GpioController gpioController, int chipSelectPin)
//     {
//         _spiDevice = spiDevice;
//         _gpioController = gpioController;
//         _chipSelectPin = chipSelectPin;
//         _gpioController.OpenPin(_chipSelectPin, PinMode.Output);
//         _gpioController.Write(_chipSelectPin, PinValue.High);
//     }

//     public double ReadChannel(int channel)
//     {
//         if (channel < 0 || channel > 3)
//         {
//             throw new ArgumentOutOfRangeException(nameof(channel), "Channel must be between 0 and 3.");
//         }

//         _gpioController.Write(_chipSelectPin, PinValue.Low);

//         byte startBit = 0x80;
//         byte singleEnded = 0x20;
//         byte channelBits = (byte)(channel << 4);

//         byte[] writeBuffer = new byte[] { (byte)(startBit | singleEnded | channelBits), 0x00, 0x00 };
//         byte[] readBuffer = new byte[3];

//         _spiDevice.TransferFullDuplex(writeBuffer, readBuffer);

//         _gpioController.Write(_chipSelectPin, PinValue.High);

//         int result = ((readBuffer[1] & 0x0F) << 8) | readBuffer[2];
//         return (double)result / 4096.0 * 3.3; // Przeskalowanie wyniku do zakresu 0-3.3V
//     }
// }

// class Program
// {
//     static void Main(string[] args)
//     {
//         var spiConnectionSettings = new SpiConnectionSettings(0, 0)
//         {
//             ClockFrequency = 500000,
//             Mode = SpiMode.Mode0
//         };

//         var spiDevice = SpiDevice.Create(spiConnectionSettings);
//         var gpioController = new GpioController();

//         // Chip Select (CS) na GPIO 25 (pin 22)
//         int chipSelectPin = 25;

//         var adc = new ADC0834(spiDevice, gpioController, chipSelectPin);

//         while (true)
//         {
//             double value = adc.ReadChannel(0); // Odczyt z kanału 0
//             Console.WriteLine($"Odczytana wartość z MQ-9: {value:F2} V");

//             System.Threading.Thread.Sleep(1000);
//         }
//     }
// }