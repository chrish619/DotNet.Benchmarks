using System.Collections;
using System.Xml;

namespace DotNet.Benchmarks;

[SimpleJob()]
[MemoryDiagnoser]
public class XmlNodeCloneBenchmarks
{
    XmlDocument _sample;

    [Params(4, 32, 256, 10240)]
    public int SampleSize { get; set; }

    [GlobalSetup]
    public void SetUp()
    {
        _sample = new XmlDocument();

        _sample.AppendChild(_sample.CreateElement("values"));

        for (var i = 0; i < SampleSize; i++)
        {
            XmlElement element = _sample.CreateElement("value");
            element.InnerText = Guid.NewGuid().ToString();

            _sample.FirstChild
                .AppendChild(element);
        }
    }

    [Benchmark]
    public void XmlDocument_LoadXml()
    {
        XmlDocument document = new XmlDocument();

        document.LoadXml(_sample.OuterXml);
    }

    [Benchmark]
    public void XmlDocument_ImportNode()
    {
        XmlDocument document = new XmlDocument();

        XmlNode newNode = document.ImportNode(_sample.DocumentElement, true);
        document.AppendChild(newNode);
    }
}
