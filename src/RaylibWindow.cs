using System;
using System.Timers;
using Raylib_cs;

namespace Cship8;

internal class RaylibWindow
{
    public bool WindowShouldClose { get => Raylib.WindowShouldClose(); }

    private const int TARGET_FPS = 60;

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

    public void Loop(Action runCpuCycle, int cyclesPerSec = 500, int targetFps = 60)
    {
        Raylib.SetTargetFPS(targetFps);

        System.Timers.Timer cycleTimer = new System.Timers.Timer((int) (1000 / cyclesPerSec));
        cycleTimer.Elapsed += (sender, e) => runCpuCycle();

        cycleTimer.Start();

        while (!Raylib.WindowShouldClose())
        {
            DrawScreen();
        }

        cycleTimer.Stop();
        cycleTimer.Dispose();
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
