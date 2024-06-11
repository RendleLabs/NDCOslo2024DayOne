namespace OneBRC.Shared;

public struct IntAccumulator
{
    private const float Divider = 1000f;
    public IntAccumulator()
    {
    }

    public string City { get; private set; } = string.Empty;
    private long _total;
    private int _count;
    private int _min;
    private int _max;

    public void SetCity(string city) => City = city;

    public float Min => _min / Divider;
    public float Max => _max / Divider;

    public float Mean => (_total / Divider) / _count;

    public void Record(int value)
    {
        _total += value;
        _count++;
        _max = Math.Max(_max, value);
        _min = Math.Min(_min, value);
    }

    public void Combine(IntAccumulator other)
    {
        _total += other._total;
        _count += other._count;
        _max = Math.Max(_max, other._max);
        _min = Math.Min(_min, other._min);
    }
}