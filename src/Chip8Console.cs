namespace Cship8;

public class Chip8Console
{
    public static int DisplayWidth { get => 64; }
    public static int DisplayHeight { get => 32; }

    public I1BitDisplay Display { get; }

    private IChip8Cpu _cpu;
    private IChip8Memory _memory;
    private IChip8Input _input;

    public Chip8Console(IChip8Input input)
    {
        _input = input;
        _memory = new Memory();
        Display = new Display(DisplayWidth, DisplayHeight);
        _cpu = new Cpu(_memory, Display, _input);
    }

    public void LoadRom(byte[] rom) => _memory.LoadRom(rom);

    public void EmulateCycle() => _cpu.EmulateCycle();
}
