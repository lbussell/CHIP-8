namespace Cship8;

public class Chip8Console
{
    public I1BitDisplay Display { get; }

    private static int _displayWidth = 64;
    private static int _displayHeight = 32;

    private IChip8Cpu _cpu;
    private IChip8Memory _memory;

    public Chip8Console()
    {
        Display = new Display(_displayWidth, _displayHeight);
        _memory = new Memory();
        _cpu = new Cpu(_memory, Display);
    }

    public void LoadRom(byte[] rom) => _memory.LoadRom(rom);

    public void EmulateCycle() => _cpu.EmulateCycle();
}
