using Raylib_cs;

namespace Cship8;

public class RaylibInput : IChip8Input
{
    private static KeyboardKey[] keys = new KeyboardKey[]
    {
        Raylib_cs.KeyboardKey.KEY_ZERO,
        Raylib_cs.KeyboardKey.KEY_ONE,
        Raylib_cs.KeyboardKey.KEY_TWO,
        Raylib_cs.KeyboardKey.KEY_THREE,

        Raylib_cs.KeyboardKey.KEY_FOUR,
        Raylib_cs.KeyboardKey.KEY_FIVE,
        Raylib_cs.KeyboardKey.KEY_SIX,
        Raylib_cs.KeyboardKey.KEY_SEVEN,

        Raylib_cs.KeyboardKey.KEY_EIGHT,
        Raylib_cs.KeyboardKey.KEY_NINE,
        Raylib_cs.KeyboardKey.KEY_A,
        Raylib_cs.KeyboardKey.KEY_B,

        Raylib_cs.KeyboardKey.KEY_C,
        Raylib_cs.KeyboardKey.KEY_D,
        Raylib_cs.KeyboardKey.KEY_E,
        Raylib_cs.KeyboardKey.KEY_F,
    };

    public RaylibInput() { }

    public bool IsKeyDown(byte key) => CheckKeyPressed(key);

    public bool AnyKeyDown() => keys
            .Select(key => CheckKeyPressed(key))
            .Any(isDown => isDown);

    public byte GetFirstKeyDown()
    {
        Raylib_cs.KeyboardKey key = (Raylib_cs.KeyboardKey) Raylib.GetKeyPressed();
        var res = (byte) Array.IndexOf(keys, key);
        return res;
    }

    private static bool CheckKeyPressed(byte key) 
    {
        return Raylib.IsKeyDown(keys[key]);
    }

    private static bool CheckKeyPressed(KeyboardKey key)
    {
        return Raylib.IsKeyDown(key);
    }
}