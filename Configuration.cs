using Microsoft.Extensions.Configuration;
using Quicktup.Util;
using Quicktup.Variables;

namespace Quicktup;

public static class ConfigurationNew
{
    private static IConfigurationRoot? Config;
    public static string? ReadConfig(string key) => Config?[key];
    public static void ConfigureApplication()
    {
        if(!File.Exists("config.cfg"))
        {
            GenerateConfig();
        }

        Config = new ConfigurationBuilder()
            .AddIniFile($"{Directory.GetCurrentDirectory()}\\config.cfg", optional: false, reloadOnChange: false)
            .Build();

        foreach(Setting setting in Setup.settings.Values)
            ConfigureSetting(setting);
        
        Config = new ConfigurationBuilder()
            .AddIniFile($"{Directory.GetCurrentDirectory()}\\config.cfg", optional: false, reloadOnChange: false)
            .Build();
    }
    private static void GenerateConfig()
    {   
        List<string> config_cfg = [
            $"VarExtension=\"{ConfigurationVariables.VarExtension}\"",
            "AskAll=true"
        ];  
        File.WriteAllLines("config.cfg", config_cfg);
    }
    private static void ConfigureSetting(Setting setting)
    {
        foreach(var variable in setting.Variables)
        {
            variable.Value.Value = ReadConfig($"{setting.ConfigName}.{variable.Key}");
        }
    }


    static bool Ask(string message)
    {
        string[] options = ["Yes", "No"];
        int index = 0;
        ConsoleKey key;

        int crrLine = Console.CursorTop;
        do
        {
            Console.SetCursorPosition(0, crrLine);
            Console.WriteLine($"{message}        ");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == index)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"> {options[i]} ");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write($"  {options[i]} ");
                }
            }

            Console.Write("      ");

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.LeftArrow && index > 0)
                index--;
            else if (key == ConsoleKey.RightArrow && index < options.Length-1)
                index++;

        } while (key != ConsoleKey.Enter);
        
        Console.SetCursorPosition(0, crrLine);
        return options[index].Equals("Yes");
    }
}