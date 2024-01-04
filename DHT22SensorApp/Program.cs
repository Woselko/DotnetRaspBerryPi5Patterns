using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading;
public class Program
{
    private const int DhtPin = 17;
    private static GpioController gpio;

    public static void Main()
    {
        gpio = new GpioController();
        Dht22Reader dht11Reader = new Dht22Reader(DhtPin);

        try
        {
            while (true)
            {
                var result = dht11Reader.ReadDht22();
                if (result != null)
                {
                    var (humidity, temperature) = result.Value;
                    Console.WriteLine($"Humidity: {humidity}%, Temperature: {temperature}℃");
                }
                Thread.Sleep(1000);
            }
        }
        finally
        {
            gpio.Dispose();
        }
    }
}