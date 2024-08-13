using System.Device.Gpio;
using System.Device.Spi;
using Iot.Device.Mfrc522;
using Iot.Device.Rfid;

string GetCardIdentifier(Data106kbpsTypeA card) => Convert.ToHexString(card.NfcId);

var controller = new GpioController();
int pinReset = 25;

var connection = new SpiConnectionSettings(0, 0)
{
    ClockFrequency = 10_000_000
};

var source = new CancellationTokenSource();
var token = source.Token;

var task = Task.Run(() => ReadData(token), token);

Console.WriteLine("Press any key to stop reading data...");
Console.ReadLine();
source.Cancel();

await task;

void ReadData(CancellationToken token)
{
    var active = true;
    do
    {
        if (token.IsCancellationRequested){ active = false; }
        try
        {
            using (SpiDevice spi = SpiDevice.Create(connection))
            {
                using(MfRc522 mfRc522 = new (spi, pinReset, controller, false))
                {
                    Data106kbpsTypeA card;
                    var result = mfRc522.ListenToCardIso14443TypeA(out card, TimeSpan.FromSeconds(2));
                    if (result)
                    {
                        Console.WriteLine($"Card detected: {GetCardIdentifier(card)}");
                    }
                }
            }
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }

    }while (active);

    Console.WriteLine("Task finished.");
}

