namespace Cship8;

public class Cpu : IChip8Cpu
{
    private readonly IChip8Memory _memory;
    private readonly I1BitDisplay _display;
    private readonly IChip8Input _input;

    private const int STACK_SIZE = 16;
    private const int NUM_V_REGISTERS = 16;
    private const int INSTRUCTION_SIZE = 2;

    private ushort _opcode;
    private ushort _programCounter;
    private byte _stackPointer = 0;
    private ushort[] _stack = new ushort[STACK_SIZE];
    private byte[] _v = new byte[NUM_V_REGISTERS];
    private ushort _i;
    private byte _delay;
    private byte _sound;
    private bool _halted = false;

    private readonly Random _random;

    public Cpu(IChip8Memory memory, I1BitDisplay display, IChip8Input input, int? seed = null)
    {
        _memory = memory;
        _display = display;
        _input = input;
        _programCounter = Memory.RomOffset;
        _random = seed is not null ? new Random((int)seed) : new Random();
    }

    public void UpdateTimers()
    {
        if (_delay > 0)
        {
            _delay -= 1;
        }
        if (_sound > 0)
        {
            _sound -= 1;
        }
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
        byte x = (byte)((_opcode & 0x0F00) >> 8);
        byte y = (byte)((_opcode & 0x00F0) >> 4);
        byte n = (byte)((_opcode & 0x000F));
        byte kk = (byte)(_opcode & 0x00FF);
        ushort nnn = (ushort)(_opcode & 0x0FFF);

        bool incrementPc = true;
        bool underflow = false;

        switch (_opcode & 0xF000)
        {
            case 0x0000:
                if (kk == 0xEE)
                {
                    // 0x00EE: Return from a subroutine
                    _programCounter = _stack[--_stackPointer];
                }
                else if (y == 0xE)
                {
                    // 0x00E0: Clear display
                    _display.Clear();
                }
                break;
            case 0x1000: // 1NNN: Jump to NNN
                _programCounter = nnn;
                incrementPc = false;
                break;
            case 0x2000: // 2NNN: Call subroutine at NNN
                _stack[_stackPointer++] = _programCounter;
                _programCounter = nnn;
                incrementPc = false;
                break;
            case 0x3000: // 3XKK: skip next instruction if VX = KK
                if (_v[x] == kk)
                {
                    _programCounter += INSTRUCTION_SIZE;
                }
                break;
            case 0x4000:
                if (_v[x] != kk)
                {
                    _programCounter += INSTRUCTION_SIZE;
                }
                break;
            case 0x5000:
                if (_v[x] == _v[y])
                {
                    _programCounter += INSTRUCTION_SIZE;
                }
                break;
            case 0x6000: // 6XKK: Set register VX to KK
                _v[x] = kk;
                break;
            case 0x7000: // 7XKK: Add KK to VX
                _v[x] += kk;
                break;
            case 0x8000:
                switch (n)
                {
                    case 0x0: // 8XY0: Set VX = VY
                        _v[x] = _v[y];
                        break;
                    case 0x1: // 8XY1: Set VX = VX OR VY
                        _v[x] = (byte)(_v[x] | _v[y]);
                        break;
                    case 0x2: // 8XY2: Set VX = VX AND VY
                        _v[x] = (byte)(_v[x] & _v[y]);
                        break;
                    case 0x3: // 8XY3: Set VX = VX XOR VY 
                        _v[x] = (byte)(_v[x] ^ _v[y]);
                        break;
                    case 0x4: // 8XY4
                        // add vY to vX, vF is set to 1 if an overflow happened, to 0 if not, even if X=F!
                        int result = _v[x] + _v[y];
                        _v[x] = (byte)(result & 0xFF);
                        _v[0xF] = (byte)((result & 0xFF00) > 0 ? 1 : 0);
                        break;
                    case 0x5: // 8XY5
                        // subtract vY from vX, vF is set to 0 if an underflow happened, to 1 if not, even if X=F!
                        underflow = _v[y] > _v[x];
                        _v[x] = (byte)(_v[x] - _v[y]);
                        _v[0xF] = (byte)(underflow ? 0 : 1);
                        break;
                    case 0x6: // 8XY6
                    case 0xE: // 8XYE
                        _v[x] = _v[y];
                        byte shiftedOut;
                        if (n == 0x6)
                        {
                            shiftedOut = (byte) (_v[x] & 0x1);
                            _v[x] >>= 1;
                        }
                        else
                        {
                            shiftedOut = (byte) (_v[x] & 0x80);
                            _v[x] <<= 1;
                        }
                        _v[0xF] = (byte) (shiftedOut > 0 ? 1 : 0);
                        break;
                    case 0x7: // 8XY7
                        // set vX to the result of subtracting vX from vY, vF is set to 0 if an underflow happened, to 1 if not, even if X=F!
                        underflow = _v[x] > _v[y];
                        _v[x] = (byte)(_v[y] - _v[x]);
                        _v[0xF] = (byte)(underflow ? 0 : 1);
                        break;
                    default:
                        break;
                }
                break;
            case 0x9000: // 9XY0: Skip next instruction if Vx != Vy
                if (_v[x] != _v[y])
                {
                    _programCounter += INSTRUCTION_SIZE;
                }
                break;
            case 0xA000: // ANNN: Set register i to NNN
                _i = nnn;
                break;
            case 0xB000: // BNNN: Jump to location NNN + V0
                _programCounter = (ushort)(_v[0] + nnn);
                incrementPc = false;
                break;
            case 0xC000: // CXKK: Set VX = random byte AND KK
                byte[] randomByte = new byte[1];
                _random.NextBytes(randomByte);
                _v[x] = (byte)(randomByte[0] & kk);
                break;
            case 0xD000:
                Dxyn(x, y, n);
                break;
            case 0xE000:
                bool keyDown = _input.IsKeyDown(_v[x]);
                if (kk == 0x9E && keyDown || kk == 0xA1 && !keyDown)
                {
                    // Skip next instruction if key with the value of VX is pressed
                    _programCounter += INSTRUCTION_SIZE;
                }
                break;
            case 0xF000:
                switch (kk)
                {
                    case 0x07:
                        _v[x] = _delay;
                        break;
                    case 0x0A:
                        // Wait for a key press, store the value of the key in VX
                        _halted = true;
                        byte? k = _input.GetFirstKeyUp();
                        if (k != null)
                        {
                            _halted = false;
                            _v[x] = (byte) k;
                        }
                        break;
                    case 0x15:
                        _delay = _v[x];
                        break;
                    case 0x18:
                        _sound = _v[x];
                        break;
                    case 0x1E:
                        _i += _v[x];
                        break;
                    case 0x29:
                        _i = _memory.Get((ushort)(Memory.FontOffset + _v[x] * Memory.FontLength));
                        break;
                    case 0x33:
                        byte vx = _v[x];
                        _memory.Set(_i, (byte)(vx / 100));
                        _memory.Set((ushort)(_i + 1), (byte)((vx / 10) % 10));
                        _memory.Set((ushort)(_i + 2), (byte)(vx % 10));
                        break;
                    case 0x55:
                        for (byte i = 0; i <= x; i += 1)
                        {
                            _memory.Set((ushort)(_i++), _v[i]);
                        }
                        break;
                    case 0x65:
                        for (byte i = 0; i <= x; i += 1)
                        {
                            _v[i] = _memory.Get((ushort)(_i++));
                        }
                        break;
                    default:
                        break;
                }
                break;
            default:
                Console.WriteLine($"Instruction {_opcode.ToString("X4")} not implemented");
                break;
        }

        if (incrementPc && !_halted)
        {
            _programCounter += INSTRUCTION_SIZE;
        }
    }

    private void Dxyn(byte xRegister, byte yRegister, byte spriteHeight)
    {
        // All sprites are 8 pixels wide
        byte spriteWidth = 8;

        byte x = (byte) (_v[xRegister] % (_display.Width));
        byte y = (byte) (_v[yRegister] % (_display.Height));

        // We will set VF if we turn a pixel off
        _v[0xF] = 0;

        for (byte spriteRow = 0; spriteRow < spriteHeight; spriteRow++)
        {
            // The whole sprite row fits in one byte
            byte sprite = _memory.Get((ushort)(_i + spriteRow));

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
