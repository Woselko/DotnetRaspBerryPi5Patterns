using System.Device.Gpio;

public class Dht22Reader
{
    private const int MaxUnchangeCount = 100;
    private const byte StateInitPullDown = 1;
    private const byte StateInitPullUp = 2;
    private const byte StateDataFirstPullDown = 3;
    private const byte StateDataPullUp = 4;
    private const byte StateDataPullDown = 5;
    private readonly int DhtPin;
    private GpioController gpio;

    public Dht22Reader(int pinDht)
    {
        DhtPin = pinDht;
        gpio = new GpioController();
        gpio.OpenPin(DhtPin, PinMode.Output);
    }
    
    public (double humidity, double temperature)? ReadDht22()
    {
        gpio.SetPinMode(DhtPin, PinMode.Output);
        gpio.Write(DhtPin, PinValue.High);
        Thread.Sleep(50);
        gpio.Write(DhtPin, PinValue.Low);
        Thread.Sleep(20);
        gpio.Write(DhtPin, PinValue.High);
        gpio.SetPinMode(DhtPin, PinMode.Input);

        int unchangedCount = 0;
        int last = -1;
        List<int> data = new List<int>();

        while (true)
        {
            int current = (int)gpio.Read(DhtPin);
            data.Add(current);
            if (last != current)
            {
                unchangedCount = 0;
                last = current;
            }
            else
            {
                unchangedCount++;
                if (unchangedCount > MaxUnchangeCount)
                {
                    break;
                }
            }
        }

        byte state = StateInitPullDown;
        List<int> lengths = new List<int>();
        int currentLength = 0;

        foreach (int current in data)
        {
            currentLength++;

            if (state == StateInitPullDown)
            {
                if (current == 0)
                {
                    state = StateInitPullUp;
                }
                else
                {
                    continue;
                }
            }
            if (state == StateInitPullUp)
            {
                if (current == 1)
                {
                    state = StateDataFirstPullDown;
                }
                else
                {
                    continue;
                }
            }
            if (state == StateDataFirstPullDown)
            {
                if (current == 0)
                {
                    state = StateDataPullUp;
                }
                else
                {
                    continue;
                }
            }
            if (state == StateDataPullUp)
            {
                if (current == 1)
                {
                    currentLength = 0;
                    state = StateDataPullDown;
                }
                else
                {
                    continue;
                }
            }
            if (state == StateDataPullDown)
            {
                if (current == 0)
                {
                    lengths.Add(currentLength);
                    state = StateDataPullUp;
                }
                else
                {
                    continue;
                }
            }
        }

        if (lengths.Count != 40)
        {
            // Data not good, skip
            return null;
        }

        int shortestPullUp = lengths.Min();
        int longestPullUp = lengths.Max();
        int halfway = (longestPullUp + shortestPullUp) / 2;

        List<int> bits = new List<int>();
        List<byte> theBytes = new List<byte>();
        byte currentByte = 0;

        foreach (int length in lengths)
        {
            int bit = 0;
            if (length > halfway)
            {
                bit = 1;
            }
            bits.Add(bit);
        }

        for (int i = 0; i < bits.Count; i++)
        {
            currentByte <<= 1;
            if (bits[i] != 0)
            {
                currentByte |= 1;
            }
            else
            {
                currentByte |= 0;
            }
            if ((i + 1) % 8 == 0)
            {
                theBytes.Add(currentByte);
                currentByte = 0;
            }
        }

        byte checksum = (byte)((theBytes[0] + theBytes[1] + theBytes[2] + theBytes[3]) & 0xFF);

        if (theBytes[4] != checksum)
        {
            // Data not good, skip
            return null;
        }

        double humidity = ((theBytes[0] << 8) | theBytes[1]) * 0.1;
        double temperature = (((theBytes[2] & 0x7F) << 8) | theBytes[3]) * 0.1;

        if ((theBytes[2] & 0x80) != 0)
        {
            temperature *= -1;
        }

        return (humidity, temperature);
    }
}