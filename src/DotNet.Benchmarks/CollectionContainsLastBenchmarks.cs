namespace DotNet.Benchmarks;

[SimpleJob()]
[MemoryDiagnoser]
public class CollectionContainsLastBenchmarks
{
    [Params(64, 2048, 10240, 1_048_576)]
    public int SampleSize { get; set; }

    private string[] _valuesAsArray;
    private Dictionary<string, string> _valuesAsDictionary;
    private HashSet<string> _valuesAsHashSet;
    private List<string> _valuesAsList;
    private string _valueToFind;

    [GlobalSetup]
    public void SetUp()
    {
        _valuesAsArray = Enumerable.Range(1, SampleSize)
                .Select(x => Guid.NewGuid().ToString())
                .ToArray();

        _valuesAsDictionary = _valuesAsArray.ToDictionary(x => x);
        _valuesAsHashSet = _valuesAsArray.ToHashSet();
        _valuesAsList = _valuesAsArray.ToList();

        _valueToFind = _valuesAsArray[SampleSize - 1];
    }

    [Benchmark]
    public void Array_IndexOf()
    {
        bool containsValue = Array.IndexOf<string>(_valuesAsArray, _valueToFind) > -1;
    }

    [Benchmark]
    public void HashSet_Contains()
    {
        bool containsValue = _valuesAsHashSet.Contains(_valueToFind);
    }

    [Benchmark]
    public void List_Contains()
    {
        bool containsValue = _valuesAsList.Contains(_valueToFind);
    }
}