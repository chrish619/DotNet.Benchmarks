using System.Collections;

namespace DotNet.Benchmarks;

[SimpleJob()]
[MemoryDiagnoser]
public class StringJoinBenchmarks
{
    private string[] _valuesAsArray;
    private List<string> _valuesAsList;
    private IEnumerable<string> _valuesAsEnumerable;

    [Params(4, 8, 16, 32, 256)]
    public int SampleSize { get; set; }

    [GlobalSetup]
    public void SetUp()
    {
        _valuesAsArray = Enumerable.Range(1, SampleSize)
                .Select(x => Guid.NewGuid().ToString())
                .ToArray();

        _valuesAsList = _valuesAsArray.ToList();
        _valuesAsEnumerable = _valuesAsArray.WithEnumerator();
    }

    [Benchmark]
    public void StringJoin_Enumerable()
    {
        string s = string.Join("\n", _valuesAsEnumerable);
    }

    [Benchmark]
    public void StringJoin_Array()
    {
        string s = string.Join("\n", _valuesAsArray);
    }

    [Benchmark]
    public void StringJoin_List()
    {
        string s = string.Join("\n", _valuesAsList);
    }
}

public static class EnumerableExtensions
{
    public static IEnumerable<T> WithEnumerator<T>(this IEnumerable<T> source)
    {
        if(source == null){
            throw new ArgumentNullException(nameof(source));
        }

        return new Enumerator<T>(source);
    }

    private class Enumerator<T> : IEnumerable<T>
    {
        private IEnumerable<T> _source;

        public Enumerator(IEnumerable<T> source)
        {
            this._source = source;
        }

        public IEnumerator<T> GetEnumerator()
            => ((IEnumerable<T>)_source).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)_source).GetEnumerator();
    }
}