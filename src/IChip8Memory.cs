namespace Cship8;

public interface IChip8Memory
{
    public static ushort FontOffset { get; }
    public static ushort FontLength { get; }
    public static ushort RomOffset { get; }

    public void LoadRom(byte[] rom);
    public byte Get(ushort address);
    public ushort GetInstruction(ushort pc);
    public void Set(ushort addr, byte value);
}
