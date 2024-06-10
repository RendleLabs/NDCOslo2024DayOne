using System.Runtime.InteropServices;

namespace OneBRC;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Key : IEquatable<Key>
{
    private readonly long _part1;
    private readonly long _part2;

    public Key(long part1, long part2)
    {
        _part1 = part1;
        _part2 = part2;
    }

    public bool Equals(Key other)
    {
        return _part1 == other._part1 && _part2 == other._part2;
    }

    public override bool Equals(object? obj)
    {
        return obj is Key other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_part1, _part2);
    }

    public static bool operator ==(Key left, Key right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Key left, Key right)
    {
        return !left.Equals(right);
    }
}