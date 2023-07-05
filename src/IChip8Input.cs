namespace Cship8;

public interface IChip8Input
{
    public bool IsKeyDown(byte key);
    public bool AnyKeyDown();
    public byte GetFirstKeyDown();
}
