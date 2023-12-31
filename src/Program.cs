﻿namespace Cship8;

internal class Program
{
    static void Main(string[] args)
    {
        string romFilePath = args[0];
        if (!File.Exists(romFilePath))
        {
            throw new ArgumentOutOfRangeException($"A file does not exist at {romFilePath}");
        }

        byte[] rom = File.ReadAllBytes(romFilePath);

        RaylibInput input = new();
        Chip8Console console = new(input);
        console.LoadRom(rom);

        RaylibWindow window = new("C#IP-8", console.Display);
        window.Loop(console.EmulateCycle, console.UpdateTimers);
    }
}
