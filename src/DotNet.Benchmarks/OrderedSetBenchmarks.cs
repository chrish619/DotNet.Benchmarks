namespace DotNet.Benchmarks;

[SimpleJob()]
[MemoryDiagnoser()]
public class OrderedSetBenchmarks
{

    private struct Orderable
    {
        public DateTime TimeStamp { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Orderable orderable &&
                   TimeStamp == orderable.TimeStamp;
        }

        public override int GetHashCode()
        {
            return 1713031131 + TimeStamp.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    [Params(64, 128, 4096, 10240)]
    public int SampleSize { get; set; }

    private Comparer<Orderable> _comparer;
    private List<Orderable> _ordered;
    private List<Orderable> _random;

    [GlobalSetup]
    public void SetUp()
    {
        var baseTime = DateTime.UtcNow;

        _comparer = Comparer<Orderable>.Create((a, b) => a.TimeStamp.CompareTo(b.TimeStamp));

        _ordered = Enumerable.Range(0, SampleSize)
            .Select(i => new Orderable
            {
                TimeStamp = baseTime.AddSeconds((SampleSize * -1) + i),
            })
            .OrderBy(o => o.TimeStamp)
            .ToList();

        _random = _ordered
           .OrderBy(o => Guid.NewGuid())
           .ToList();
    }

    [Benchmark]
    public void Ordered_GetViewBetween()
    {
        var hashSet = new SortedSet<Orderable>(_comparer);

        foreach (var sample in _ordered)
        {
            AddSample_GetViewBetween(hashSet, sample, out Orderable? prevSample);
        }
    }

    [Benchmark]
    public void Random_GetViewBetween()
    {
        var hashSet = new SortedSet<Orderable>(_comparer);

        foreach (var sample in _random)
        {
            AddSample_GetViewBetween(hashSet, sample, out Orderable? prevSample);
        }
    }

    private bool AddSample_GetViewBetween(SortedSet<Orderable> set, Orderable addSample, out Orderable? prevSample)
    {
        prevSample = null;

        if (!set.Add(addSample))
        {
            return false;
        }

        var lower = new Orderable { TimeStamp = addSample.TimeStamp.AddSeconds(-5) };
        var upper = new Orderable { TimeStamp = addSample.TimeStamp.AddMilliseconds(-5) };

        var view = set.GetViewBetween(lower, upper);

        if (view.Count > 0)
        {
            prevSample = view.Max;
        }


        return true;
    }


    [Benchmark]
    public void Ordered_GetLast()
    {

        var hashSet = new SortedSet<Orderable>(_comparer);

        foreach (var sample in _ordered)
        {
            AddSample_GetLast(hashSet, sample, out Orderable? prevSample);
        }
    }

    [Benchmark]
    public void Random_GetLast()
    {
        var hashSet = new SortedSet<Orderable>(_comparer);

        foreach (var sample in _random)
        {
            AddSample_GetLast(hashSet, sample, out Orderable? prevSample);
        }
    }

    private bool AddSample_GetLast(SortedSet<Orderable> set, Orderable addSample, out Orderable? prevSample)
    {
        if (!set.Add(addSample))
        {
            prevSample = null;
            return false;
        }

        prevSample = set.LastOrDefault(s => set.Comparer.Compare(s, addSample) == -1);

        return true;
    }
}
