using System.Device.Gpio;

Console.WriteLine("Blinking LED. Press Ctrl+C to end.");
const int LED_PIN = 17;
using var controller = new GpioController();
controller.OpenPin(LED_PIN, PinMode.Output);
bool ledOn = true;
while (true)
{
    controller.Write(LED_PIN, ((ledOn) ? PinValue.High : PinValue.Low));
    Console.WriteLine("LED is {0}", ledOn ? "ON" : "OFF");
    ledOn = !ledOn;
    Thread.Sleep(1000);
}