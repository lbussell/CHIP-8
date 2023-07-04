using System.Collections;

namespace Cship8;

public class Display : I1BitDisplay
{
    public int Width { get; }
    public int Height { get; }

    public BitArray Pixels { get; }

    public Display(int width, int height)
    {
        Width = width;
        Height = height;
        Pixels = new BitArray(width * height);
    }

    public bool GetPixel(int row, int col)
        => Pixels.Get(ToIndex(row, col, Width));

    public void SetPixel(int row, int col, bool value = true)
        => Pixels.Set(ToIndex(row, col, Width), value);

    public void FlipPixel(int row, int col)
        => SetPixel(row, col, !GetPixel(row, col));

    public void Clear(bool value = false)
        => Pixels.SetAll(value);

    private static int ToIndex(int row, int col, int width)
        => row * width + col;
}
