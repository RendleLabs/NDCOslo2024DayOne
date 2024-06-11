using System.Runtime.InteropServices;
using OneBRC.Shared;

namespace OneBRC.Tests;

public class IntAccumulatorTests
{
    [Fact]
    public void UpdateInDictionary()
    {
        var dict = new Dictionary<string, IntAccumulator>
        {
            ["foo"] = new IntAccumulator()
        };

        ref var actual = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, "foo", out bool exists);
        
        Assert.True(exists);
        
        actual.SetCity("Oslo");
        
        Assert.Equal("Oslo", actual.City);
        
        Assert.Equal("Oslo", dict["foo"].City);
    }
}