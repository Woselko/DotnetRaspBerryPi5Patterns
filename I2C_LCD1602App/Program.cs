while (true)
{
    try
    {
        if (!I2C_LCD1602.Instance.WriteLine("Hello, World!", 0) ||
            !I2C_LCD1602.Instance.WriteLine(DateTime.Now.ToString(), 1))
        {
            throw new Exception("Write operation failed.");
        }
        Thread.Sleep(1000);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception caught in main loop: {ex.Message}");
        Thread.Sleep(5000); // Czekaj, daj systemowi czas na ustabilizowanie się
    }
}
