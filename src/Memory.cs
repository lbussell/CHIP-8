using System.Text;

namespace Cship8;

public class Memory : IChip8Memory
{
    public static ushort RomOffset { get => 0x0200; }
    public static ushort FontOffset { get => 0x0050; }

    private const uint _Size = 4096;
    private const uint _FontEnd = 0x009F;

    private static readonly byte[] s_font = new byte[] {
        0xF0, 0x90, 0x90, 0x90, 0xF0,		// 0
        0x20, 0x60, 0x20, 0x20, 0x70,		// 1
        0xF0, 0x10, 0xF0, 0x80, 0xF0,		// 2
        0xF0, 0x10, 0xF0, 0x10, 0xF0,		// 3
        0x90, 0x90, 0xF0, 0x10, 0x10,		// 4
        0xF0, 0x80, 0xF0, 0x10, 0xF0,		// 5
        0xF0, 0x80, 0xF0, 0x90, 0xF0,		// 6
        0xF0, 0x10, 0x20, 0x40, 0x40,		// 7
        0xF0, 0x90, 0xF0, 0x90, 0xF0,		// 8
        0xF0, 0x90, 0xF0, 0x10, 0xF0,		// 9
        0xF0, 0x90, 0xF0, 0x90, 0x90,		// A
        0xE0, 0x90, 0xE0, 0x90, 0xE0,		// B
        0xF0, 0x80, 0x80, 0x80, 0xF0,		// C
        0xE0, 0x90, 0x90, 0x90, 0xE0,		// D
        0xF0, 0x80, 0xF0, 0x80, 0xF0,		// E
        0xF0, 0x80, 0xF0, 0x80, 0x80		// F
    };

    private byte[] _memory = new byte[_Size];

    public Memory()
    {
        s_font.CopyTo(_memory, FontOffset);
    }

    public void LoadRom(byte[] rom)
    {
        rom.CopyTo(_memory, RomOffset);
    }

    public byte Get(ushort addr) => _memory[addr];

    public ushort GetInstruction(ushort pc) => (ushort) (_memory[pc] << 8 | _memory[pc + 1]);

    public void Set(ushort addr, byte value) => _memory[addr] = value;
}