using PublicVpn;
using PublicVpn.Enums;
using PublicVpn.Interfaces;
using PublicVpn.Models;
using System.Collections.Generic;
using System.Drawing;

namespace ConsoleTst;

internal class Program
{
    static ConsoleKey lastkey;
    static string whiteSpace = string.Empty;
    static Size consoleSize = Size.Empty;
    static void Main(string[] args)
    {
        Loging.LogEvent += Loging_LogEvent;
        Task.Run(() =>
        {
            while (lastkey != ConsoleKey.Escape)
                lastkey = Console.ReadKey().Key;
        });
        while(!NetworkTools.IsOnline().Result)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("No Internet connection");
            Thread.Sleep(2000);
        }
        var list = Levpro_ruVpnModel.GetVpnList().Result;
        if (list?.Count() > 0)
        {
            while (lastkey != ConsoleKey.Escape)
            {
                if(consoleSize.Width != Console.WindowWidth || 
                   consoleSize.Height != Console.WindowHeight) 
                    Resize(list);

                foreach (var host in list)
                {
                    if (lastkey == ConsoleKey.Escape) break;
                    if (host.IsStatusChanged)
                    {
                        PrintLineHost(host);
                    }
                }
                Console.Title = $"{list.Where(x => x.Status == StatusEnum.Online).Count()} vpn servers is online";
                Thread.Sleep(10000);
            }
        }
        else Console.WriteLine("Список хостов пуст");

    }

    private static void Loging_LogEvent(Exception ex, IHost host)
    {
        //PrintLineColor(ex.Message, 0, ConsoleColor.DarkYellow);
    }
    private static void PrintLineHost(Levpro_ruVpnModel host)
    {
        if (Console.WindowWidth < 130) Console.SetWindowSize(130, Console.WindowHeight);
        var top = host.Id < 0 ? 0 : (int)host.Id;
        var color = ConsoleColor.Blue;
        var prefix = "[?]";
        if (host.Status == StatusEnum.Online)
        {
            color = ConsoleColor.Green;
            prefix = "[+]";
        }
        else if (host.Status == StatusEnum.Offline)
        {
            color = ConsoleColor.Red;
            prefix = "[X]";
        }

        if (whiteSpace.Length != Console.WindowWidth)
        {
            if (whiteSpace.Length > Console.WindowWidth) whiteSpace = string.Empty;
            while (whiteSpace.Length < Console.WindowWidth)
            {
                whiteSpace += " ";
            }
        }

        Console.SetCursorPosition(0, top);
        Console.Write(whiteSpace);
        Console.SetCursorPosition(0, top);
        Console.BackgroundColor = color;
        Console.Write(prefix);
        Console.ResetColor();
        Console.Write(" ");
        Console.ForegroundColor = color;
        Console.SetCursorPosition(5, top);
        Console.Write($"{host.Country}");
        Console.SetCursorPosition(21, top);
        Console.Write($" L: {host.Login}");
        Console.SetCursorPosition(29, top);
        Console.Write($"P: {host.Password}");
        Console.SetCursorPosition(36, top);
        Console.Write($"Speed:{host.Quality}");
        Console.SetCursorPosition(54, top);
        Console.Write($"Uptime:{host.UpTime}");
        Console.SetCursorPosition(69, top);
        Console.Write($"Ping:{host.Ping}");
        Console.SetCursorPosition(80, top);
        Console.Write($"IP: {host.IP}");
        Console.SetCursorPosition(100, top);
        Console.Write($"DDNS: {host.DDNS}");
        Console.ResetColor();
    }
    private static void Ascii()
    {
        for (int i = 0; i < 256; i++)
        {
            if (i == 27) continue;
            Console.Write($"{i}[{(char)i}] ");
        }
        Console.Write("Press any key...");
        Console.ReadKey();
        Console.Clear();
    }
    private static void Resize(IEnumerable<Levpro_ruVpnModel> list)
    {
        Console.Clear();
        foreach (var host in list)
        {
            PrintLineHost(host);
        }
        consoleSize = new Size(Console.WindowWidth, Console.WindowHeight);
    }
}