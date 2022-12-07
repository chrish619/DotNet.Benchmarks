namespace DotNet.Benchmarks;

[SimpleJob()]
[MemoryDiagnoser()]
public class SetOrderBenchmarks
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

    private List<Orderable> _ordered;
    private List<Orderable> _random;

    [GlobalSetup]
    public void SetUp()
    {
        var baseTime = DateTime.UtcNow;

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
    public void UsingHashSet_Ordered()
    {
        var hashSet = new HashSet<Orderable>();

        foreach (var sample in _ordered)
        {
            AddSample_HashSet(hashSet, sample, out Orderable? prevSample);
        }
    }

    [Benchmark]
    public void UsingHashSet_Random()
    {
        var hashSet = new HashSet<Orderable>();

        foreach (var sample in _random)
        {
            AddSample_HashSet(hashSet, sample, out Orderable? prevSample);
        }
    }

    private bool AddSample_HashSet(HashSet<Orderable> set, Orderable addSample, out Orderable? prevSample)
    {
        if (!set.Add(addSample))
        {
            prevSample = null;
            return false;
        }

        prevSample = set.LastOrDefault(o => o.TimeStamp < addSample.TimeStamp);
        return true;
    }

    [Benchmark]
    public void UsingSortedSet_Ordered()
    {
        var comparer = Comparer<Orderable>.Create((a, b) => a.TimeStamp.CompareTo(b.TimeStamp));
        var hashSet = new SortedSet<Orderable>(comparer);

        foreach (var sample in _ordered)
        {
            AddSample_SortedSet(hashSet, sample, out Orderable? prevSample);
        }
    }

    [Benchmark]
    public void UsingSortedSet_Random()
    {
        var comparer = Comparer<Orderable>.Create((a, b) => a.TimeStamp.CompareTo(b.TimeStamp));
        var hashSet = new SortedSet<Orderable>(comparer);

        foreach (var sample in _random)
        {
            AddSample_SortedSet(hashSet, sample, out Orderable? prevSample);
        }
    }

    private bool AddSample_SortedSet(SortedSet<Orderable> set, Orderable addSample, out Orderable? prevSample)
    {
        if (!set.Add(addSample))
        {
            prevSample = null;
            return false;
        }

        var lower = new Orderable { TimeStamp = addSample.TimeStamp.AddSeconds(-5) };
        var upper = new Orderable { TimeStamp = addSample.TimeStamp.AddMilliseconds(-5) };

        var view = set.GetViewBetween(lower, upper);

        prevSample = view.Count > 0 ? view.Max : null;
        return true;
    }
}
