namespace OneBRC.Shared;

public class Accumulator
{
    public string? City { get; private set; }
    private float _total;
    private int _count;
    private float _min = float.MaxValue;
    private float _max = float.MinValue;

    public Accumulator(string city)
    {
        City = city;
    }

    public Accumulator()
    {
    }

    public void Record(float value)
    {
        if (value < _min) _min = value;
        if (value > _max) _max = value;
        _total += value;
        ++_count;
    }

    public float Mean => _total / _count;
    public float Min => _min;
    public float Max => _max;
    public int Count => _count;

    public void SetCity(string city)
    {
        City = city;
    }
}