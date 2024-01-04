using System.Device.Gpio;

const int LED_PIN = 17;
const int PIR_PIN = 23;
const int BUZZ_PIN = 24;

using var controller = new GpioController();
controller.OpenPin(LED_PIN, PinMode.Output);
controller.OpenPin(BUZZ_PIN, PinMode.Output);
controller.OpenPin(PIR_PIN, PinMode.Input);
controller.Write(LED_PIN, PinValue.Low);

controller.RegisterCallbackForPinValueChangedEvent(PIR_PIN, PinEventTypes.Rising, (sender, args)=>{
    controller.Write(LED_PIN, PinValue.High);
    controller.Write(BUZZ_PIN, PinValue.High);
    System.Console.WriteLine("Led is ON");
});

controller.RegisterCallbackForPinValueChangedEvent(PIR_PIN, PinEventTypes.Falling, (sender, args)=>{
    controller.Write(LED_PIN, PinValue.Low);
    controller.Write(BUZZ_PIN, PinValue.Low);
    System.Console.WriteLine("Led is OFF");
});

System.Console.WriteLine("Awaiting for user action");
Console.ReadLine();
controller.Write(LED_PIN, PinValue.Low);
controller.Dispose();
