namespace Cship8;

public class Cpu : IChip8Cpu {
    private readonly IChip8Memory _memory;
    private readonly I1BitDisplay _display;

    private const int STACK_SIZE = 16;
    private const int NUM_V_REGISTERS = 16;

    private ushort _opcode;
    private ushort _programCounter;
    private byte _stackPointer;
    private ushort[] _stack = new ushort[STACK_SIZE];
    private byte[] _v = new byte[NUM_V_REGISTERS];
    private ushort _i;
    private byte _delay;
    private byte _sound;

    public Cpu(IChip8Memory memory, I1BitDisplay display)
    {
        _memory = memory;
        _display = display;
        _programCounter = Memory.RomOffset;
    }

    public void EmulateCycle()
    {
        _opcode = _memory.GetInstruction(_programCounter);

        /**
            CHIP-8 Opcode Formats
            * X K K
            * X Y N
            * N N N
        **/
        byte x = (byte) ((_opcode & 0x0F00) >> 8);
        byte y = (byte) ((_opcode & 0x00F0) >> 4);
        byte n = (byte) ((_opcode & 0x000F));
        byte kk = (byte) (_opcode & 0x00FF);
        ushort nnn = (ushort) (_opcode & 0x0FFF);

        bool incrementOpcode = true;

        switch (_opcode & 0xF000)
        {
            case 0x0000:
                if (kk == 0xEE)
                {
                    // Return from a subroutine

                }
                else if (y == 0xE)
                {
                    // Clear display
                    _display.Clear();
                }
                break;
            case 0x1000:
                _programCounter = nnn;
                incrementOpcode = false;
                break;
            case 0x6000: // 6XKK: Set register VX to KK
                _v[x] = kk;
                break;
            case 0x7000: // 7XKK: Add KK to VX
                _v[x] += kk;
                break;
            case 0xA000: // ANNN: Set register i to NNN
                _i = nnn;
                break;
            case 0xD000:
                Dxyn(x, y, n);
                break;
            default:
                Console.WriteLine($"Instruction {_opcode.ToString("X4")} not implemented");
                break;
        }

        if (incrementOpcode)
        {
            _programCounter += 2;
        }
    }

    private void Dxyn(byte xRegister, byte yRegister, byte spriteHeight)
    {
        // All sprites are 8 pixels wide
        byte spriteWidth = 8;

        byte x = _v[xRegister];
        byte y = _v[yRegister];

        _v[0xF] = 0;

        for (byte spriteRow = 0; spriteRow < spriteHeight; spriteRow++)
        {
            byte sprite = _memory.Get((ushort) (_i + spriteRow));

            for (byte spriteCol = 0; spriteCol < spriteWidth; spriteCol++)
            {
                // Check sprite against a mask starting from the far left
                // sprite:  0b 12345678
                // mask:    0b 10000000 (0x80)
                if ((sprite & 0x80) > 1)
                {
                    // We are flipping a pixel!
                    byte row = (byte)(y + spriteRow);
                    byte col = (byte)(x + spriteCol);

                    if (_display.GetPixel(row, col))
                    {
                        // Set the carry bit only if we turn a pixel off
                        _v[0xF] = 1;
                    }

                    _display.FlipPixel(row, col);
                }

                // Move the sprite back so that we check the next bit...
                // sprite:  0b 23456780
                sprite <<= 1;
            }
        }
    }
}