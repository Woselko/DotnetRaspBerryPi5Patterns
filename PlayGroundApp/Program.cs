using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Device.Analog;

// // Numer pinu GPIO, do którego podłączony jest sygnał cyfrowy z czujnika
// int pinDigital = 24;
// // Utworzenie instancji kontrolera GPIO
// using GpioController controller = new GpioController();
// // Ustawienie pinu jako wejścia
// controller.OpenPin(pinDigital, PinMode.Input);
// Console.WriteLine($"Monitoring digital signal on GPIO pin {pinDigital}...");
// while (true)
// {
//     // Odczyt stanu pinu
//     PinValue pinValue = controller.Read(pinDigital);
//     // Wyświetlenie odczytanej wartości
//     Console.WriteLine($"Digital signal value: {pinValue}");
//     // Opóźnienie przed kolejnym odczytem
//     System.Threading.Thread.Sleep(1000);
// }


// using System;
// using System.Device.Gpio;
// using System.Device.Spi;
// using Iot.Device.Adc;

// class Program
// {
//     static void Main(string[] args)
//     {
//         // Konfiguracja SPI
//         var settings = new SpiConnectionSettings(0, 0)
//         {
//             ClockFrequency = 500000,
//             Mode = SpiMode.Mode0
//         };

//         using var spi = SpiDevice.Create(settings);
//         using var adc = new Adc0834(spi);

//         // MQ-9 podłączony do kanału 0
//         int channel = 0;

//         while (true)
//         {
//             double voltage = adc.ReadVoltage(channel);
//             Console.WriteLine($"Odczytane napięcie: {voltage} V");
//             System.Threading.Thread.Sleep(1000); // Odczekaj 1 sekundę przed kolejnym odczytem
//         }
//     }
// }

// // Klasa ADC0834
// public class Adc0834 : IDisposable
// {
//     private SpiDevice _spiDevice;

//     public Adc0834(SpiDevice spiDevice)
//     {
//         _spiDevice = spiDevice;
//     }

//     public double ReadVoltage(int channel)
//     {
//         if (channel < 0 || channel > 3)
//         {
//             throw new ArgumentOutOfRangeException(nameof(channel), "Channel must be between 0 and 3");
//         }

//         // Wysłanie komendy odczytu
//         byte[] readBuffer = new byte[2];
//         byte[] writeBuffer = new byte[2]
//         {
//             (byte)(0b1000_0000 | (channel << 4)),
//             0x00
//         };

//         _spiDevice.TransferFullDuplex(writeBuffer, readBuffer);

//         // Przetwarzanie wyniku
//         int result = ((readBuffer[0] & 0x03) << 8) | readBuffer[1];
//         double voltage = result * 3.3 / 1023; // Zakładamy, że napięcie zasilania to 3.3V

//         return voltage;
//     }

//     public void Dispose()
//     {
//         _spiDevice?.Dispose();
//     }
// }
//===============================================================
// using System;
// using System.Device.Gpio;
// using System.Device.Spi;
// using Iot.Device.Adc;

// class Program
// {
//     static void Main(string[] args)
//     {
//         // Konfiguracja SPI
//         var spiSettings = new SpiConnectionSettings(0, 0)
//         {
//             ClockFrequency = 500000,
//             Mode = SpiMode.Mode0
//         };
//         using (SpiDevice spiDevice = SpiDevice.Create(spiSettings))
//         {
//             while (true)
//             {
//                 int value = ReadAdcChannel(spiDevice, 0);
//                 Console.WriteLine($"Odczytana wartość: {value}");

//                 // Przekształcenie odczytu na napięcie (jeśli potrzebne)
//                 double voltage = value * 5.0 / 255.0;
//                 Console.WriteLine($"Napięcie: {voltage:F2} V");

//                 // Dodaj opóźnienie, aby nie zapełniać konsoli zbyt szybko
//                 Thread.Sleep(1000);
//             }
//         }
//     }

//     static int ReadAdcChannel(SpiDevice spiDevice, int channel)
//     {
//         // Przygotowanie danych do wysłania
//         byte startBit = 0x01;
//         byte mode = (byte)(0x08 | (channel << 4)); // Dla kanału 0: 1000 0000
//         byte[] writeBuffer = new byte[] { startBit, mode, 0x00 };
//         byte[] readBuffer = new byte[3];

//         // Wysłanie i odbiór danych SPI
//         spiDevice.TransferFullDuplex(writeBuffer, readBuffer);

//         // Przetworzenie otrzymanych danych
//         int result = ((readBuffer[1] & 0x03) << 8) | readBuffer[2];

//         return result;
//     }
// }

//===============================================================

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
