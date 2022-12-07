using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace DotNet.Benchmarks;

[SimpleJob()]
[MemoryDiagnoser]
public class XmlAppendingBenchmarks
{
    [Params(64, 128, 4096, 10240)]
    public int SampleSize { get; set; }

    private string[] _values;

    [GlobalSetup]
    public void SetUp()
    {
        _values = Enumerable.Range(1, SampleSize)
                .Select(x => Guid.NewGuid().ToString())
                .ToArray();
    }

    [Benchmark]
    public void XmlDocumentAppendElement()
    {
        string outerXml = string.Empty;
        var xmlDocument = new System.Xml.XmlDocument();

        xmlDocument.LoadXml("<root></root>");
        var root = xmlDocument.DocumentElement;

        xmlDocument.CreateXmlDeclaration("1.0", "utf-8", string.Empty);

        foreach (var value in _values)
        {
            var valueNode = xmlDocument.CreateNode(System.Xml.XmlNodeType.Element, "element", "");
            var valueAttr = xmlDocument.CreateAttribute("value");

            valueAttr.Value = value;

            valueNode.Attributes.Append(valueAttr);
            root.AppendChild(valueNode);
        }

        outerXml = xmlDocument.OuterXml;
    }

    [Benchmark]
    public void XmlTextWriterWrite()
    {
        string outerXml = string.Empty;
        var sb = new System.Text.StringBuilder();
        var settings = new System.Xml.XmlWriterSettings()
        {
            OmitXmlDeclaration = false,
            Encoding = System.Text.Encoding.UTF8,
        };

        using (var xtw = System.Xml.XmlWriter.Create(sb, settings))
        {
            xtw.WriteStartElement("root");

            foreach (var value in _values)
            {
                xtw.WriteStartElement("element");
                xtw.WriteAttributeString("value", value);

                xtw.WriteEndElement(); // </element>
            }
        }

        outerXml = sb.ToString();
    }

    [Benchmark]
    public void XDocumentWrite()
    {
        string outerXml = string.Empty;
        var xmlDeclaration = new System.Xml.Linq.XDeclaration("1.0", "utf-8", "");
        var xmlDocument = System.Xml.Linq.XDocument.Parse("<root></root>");

        var root = xmlDocument.Root;

        foreach (var value in _values)
        {
            var valueNode = new System.Xml.Linq.XElement("element");
            var valueAttr = new System.Xml.Linq.XAttribute("value", value);

            valueNode.Add(valueAttr);
            root.Add(valueNode);
        }

        outerXml = xmlDocument.ToString();
    }

    
}