using System.Collections;

public interface I1BitDisplay
{
    public int Width { get; }
    public int Height { get; }
    public bool GetPixel(int row, int col);
    public void SetPixel(int row, int col, bool value);
    public void FlipPixel(int row, int col);
    public void Clear(bool value = false);
}
