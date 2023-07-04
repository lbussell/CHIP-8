using Raylib_cs;

namespace Cship8;

internal class RaylibWindow
{
    private const int TARGET_FPS = 60;

    private int _scale;
    private I1BitDisplay _display;

    public RaylibWindow(string title, I1BitDisplay display, int scale = 8)
    {
        _scale = scale;
        _display = display;

        Raylib.InitWindow(
            _display.Width * _scale,
            _display.Height * _scale,
            title);

        Raylib.ClearBackground(Color.BLACK);

        DrawScreen(_display, _scale);
    }

    public void Loop(Action runLoop)
    {
        Raylib.SetTargetFPS(TARGET_FPS);

        while (!Raylib.WindowShouldClose())
        {
            runLoop();
            DrawScreen(_display, _scale);
        }

        Raylib.CloseWindow();
    }

    private static void DrawScreen(I1BitDisplay display, int scale)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.BLACK);

        for (int r = 0; r < display.Height; r++)
        {
            for (int c = 0; c < display.Width; c++)
            {
                if (display.GetPixel(r, c))
                {
                    DrawPixel(r, c, display, scale);
                }
            }
        }

        Raylib.EndDrawing();
    }

    private static void DrawPixel(int r, int c, I1BitDisplay display, int scale) =>
        Raylib.DrawRectangle(c * scale, r * scale, scale, scale, Color.WHITE);
}
