using System.Reflection;

namespace RaspCameraLibrary.Helpers;

/// <summary>
/// Class to automatically generate arguments for start info.
/// </summary>
public abstract class Arguments
{
    /// <summary>
    /// Generate arguments.
    /// </summary>
    public override string ToString()
    {
        string args = "";

        foreach (PropertyInfo property in GetType().GetProperties())
        {
            ArgumentAttribute? argument = property.GetCustomAttribute<ArgumentAttribute>();

            if (argument is null) continue;

            var value = property.GetValue(this);

            if (value is null) continue;

            if (value is bool a && !a) continue;

            if (value is bool b && b)
            {
                args += $" {argument.Argument}";
                continue;
            }

            if (value is uint c)
            {
                args += $" {argument.Argument} {c}";
                continue;
            }

            if (value is decimal d)
            {
                args += $" {argument.Argument} {d}";
                continue;
            }

            if (value is string e)
            {
                args += $" {argument.Argument} {e}";
                continue;
            }

            if (value is Stringable f)
            {
                args += $" {argument.Argument} {f}";
                continue;
            }
        }

        return args;
    }
}