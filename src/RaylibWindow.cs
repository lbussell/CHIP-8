using System.Diagnostics;
using Raylib_cs;

namespace Cship8;

internal class RaylibWindow
{
    public bool WindowShouldClose { get => Raylib.WindowShouldClose(); }

    private int _scale;
    private I1BitDisplay _display;

    public RaylibWindow(string title, I1BitDisplay display, int scale = 8)
    {
        _scale = scale;
        _display = display;

        Raylib.InitWindow(
            display.Width * _scale,
            display.Height * _scale,
            title);

        Raylib.ClearBackground(Color.BLACK);

        DrawScreen();
    }

    public void Loop(Action runCpuCycle, Action updateTimers, int cpuCyclesPerSec = 15*60, int targetFps = 60)
    {
        int cycleIntervalMs = (int)(1000.0 / cpuCyclesPerSec);
        int drawIntervalMs = (int)(1000.0 / targetFps);

        long elapsedMs;

        Stopwatch cycleTimer = new Stopwatch();
        long lastCycleTime = 0;
        long lastCpuCycleLength = 0;
        long lastDrawTime = 0;

        cycleTimer.Start();

        while (!Raylib.WindowShouldClose())
        {
            elapsedMs = cycleTimer.ElapsedMilliseconds;

            if (elapsedMs - lastCycleTime >= cycleIntervalMs)
            {
                lastCycleTime = elapsedMs;
                runCpuCycle();
                lastCpuCycleLength = elapsedMs - lastCycleTime;
                lastCycleTime = elapsedMs;
            }

            if (elapsedMs - lastDrawTime >= drawIntervalMs)
            {
                updateTimers();
                DrawScreen();
                lastDrawTime = elapsedMs;
            }

            Thread.Sleep((int) (cycleIntervalMs - lastCpuCycleLength));
        }

        cycleTimer.Stop();
        Raylib.CloseWindow();
    }

    public void DrawScreen()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.BLACK);

        for (int r = 0; r < _display.Height; r++)
        {
            for (int c = 0; c < _display.Width; c++)
            {
                if (_display.GetPixel(r, c))
                {
                    DrawPixel(r, c, _display, _scale);
                }
            }
        }

        Raylib.EndDrawing();
    }

    private static void DrawPixel(int r, int c, I1BitDisplay display, int scale) =>
        Raylib.DrawRectangle(c * scale, r * scale, scale, scale, Color.WHITE);
}
