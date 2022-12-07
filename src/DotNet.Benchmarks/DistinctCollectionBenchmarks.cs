using System.Diagnostics;

namespace DotNet.Benchmarks;

[SimpleJob()]
[MemoryDiagnoser]
public class DistinctCollectionBenchmarks
{
    [Params(128, 2_048, 16_384, 65_000)]
    public int SampleSize { get; set; }

    private string[] _samples;

    [GlobalSetup]
    public void SetUp()
    {
        _samples = Enumerable.Range(1, SampleSize)
                .Select(x => Guid.NewGuid().ToString())
                .ToArray();
    }

    [Benchmark]
    public void Arrays_Distinct()
    {
        string[] values = new string[4];
        int l = 0;
        for (int i = 0; i < _samples.Length; i++)
        {
            int ix = Array.IndexOf<string>(values, _samples[i]);

            if (ix > -1)
            {
                continue;
            }

            if (l >= values.Length)
            {
                Array.Resize(ref values, values.Length * 2);
            }

            values[l] = _samples[i];

            l++;
        }

        Array.Resize(ref values, l);
    }

    [Benchmark]
    public void HashSet_Distinct()
    {
        HashSet<string> values = new HashSet<string>();

        for (int i = 0; i < _samples.Length; i++)
        {
            values.Add(_samples[i]);
        }
    }

    [Benchmark]
    public void List_Distinct()
    {
        List<string> values = new List<string>();

        for (int i = 0; i < _samples.Length; i++)
        {
            if (!values.Contains(_samples[i]))
            {
                values.Add(_samples[i]);
            }
        }
    }
}