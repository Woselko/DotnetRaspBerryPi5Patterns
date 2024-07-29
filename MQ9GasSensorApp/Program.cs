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