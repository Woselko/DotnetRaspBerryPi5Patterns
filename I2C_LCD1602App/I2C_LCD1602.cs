using System;
using System.Device.Gpio;
using System.Device.I2c;
using Iot.Device.CharacterLcd;
using Iot.Device.Pcx857x;

public class I2C_LCD1602 : IDisposable
{
    private static readonly Lazy<I2C_LCD1602> _instance = new Lazy<I2C_LCD1602>(() => new I2C_LCD1602());
    public static I2C_LCD1602 Instance => _instance.Value;

    private Lcd1602 lcd;
    private I2cDevice i2cDevice;
    private Pcf8574 driver;
    private GpioController gpioController;

    private I2C_LCD1602()
    {
        Initialize();
    }

    private bool Initialize()
    {
        try
        {
            i2cDevice = I2cDevice.Create(new I2cConnectionSettings(1, 0x27));
            driver = new Pcf8574(i2cDevice);
            gpioController = new GpioController(PinNumberingScheme.Logical, driver);
            lcd = new Lcd1602(
                registerSelectPin: 0,
                enablePin: 2,
                dataPins: new int[] { 4, 5, 6, 7 },
                backlightPin: 3,
                backlightBrightness: 0.1f,
                readWritePin: 1,
                controller: gpioController);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize LCD: {ex.Message}");
            Dispose();
            return false;
        }
    }

    public bool WriteLine(string text, int line)
    {
        if (lcd != null)
        {
            try
            {
                lcd.SetCursorPosition(0, line);
                lcd.Write(text.PadRight(lcd.Size.Width));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying text: {ex.Message}");
                if (!Reinitialize())
                {
                    throw new InvalidOperationException("Failed to reinitialize LCD.", ex);
                }
            }
        }
        else if (!Reinitialize())
        {
            throw new InvalidOperationException("Failed to initialize on first attempt.");
        }
        return false;
    }


    private bool Reinitialize()
    {
        Dispose();
        return Initialize();
    }

    public void Dispose()
    {
        lcd?.Dispose();
        gpioController?.Dispose();
        driver?.Dispose();
        i2cDevice?.Dispose();

        lcd = null;
        gpioController = null;
        driver = null;
        i2cDevice = null;
    }
}
