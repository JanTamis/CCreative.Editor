using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

#pragma warning disable CS1591

namespace CCreative.Data;

public class XML
{
	private readonly XElement node;

	public XML(string tag)
	{
		node = new XElement(tag);
	}

	private XML(XElement? node)
	{
		ArgumentNullException.ThrowIfNull(node);

		this.node = node;
	}

	public XML GetParent()
	{
		return new XML(node.Parent);
	}

	public string GetName()
	{
		return node.Name.LocalName;
	}

	public bool HasChildren()
	{
		return node.HasElements;
	}

	public IEnumerable<string> ListChildren()
	{
		return node
			.Elements()
			.Select(x => x.Name.LocalName);
	}

	public IEnumerable<XML> GetChildren()
	{
		return node
			.Elements()
			.Select(x => new XML(x));
	}

	public IEnumerable<XML> GetChildren(string name)
	{
		return node
			.Elements(name)
			.Select(x => new XML(x));
	}

	public XML? GetChild(string name)
	{
		return node
			.Elements(name)
			.Select(x => new XML(x))
			.FirstOrDefault();
	}

	public XML AddChild(string tag)
	{
		var element = new XElement(tag);
		node.Add(element);

		return new XML(element);
	}

	public void RemoveChild(XML child)
	{
		child.node.RemoveAll();
	}

	public int GetAttibuteCount()
	{
		return node
			.Attributes()
			.Count();
	}

	public IEnumerable<string> ListAttributes()
	{
		return node
			.Attributes()
			.Select(x => x.Name.LocalName);
	}

	public bool HasAttributes()
	{
		return node.HasAttributes;
	}

	public string GetString(string name, string defaultValue = "")
	{
		var attribute = node.Attribute(name);

		return attribute is not null ? attribute.Value : defaultValue;
	}

	public int GetInt(string name, int defaultValue = default)
	{
		var attribute = node.Attribute(name);

		if (attribute is not null && Int32.TryParse(attribute.Value, out var result)) return result;

		return defaultValue;
	}

	public float GetFloat(string name, float defaultValue = default)
	{
		var attribute = node.Attribute(name);

		if (attribute is not null && Single.TryParse(attribute.Value, out var result)) return result;

		return defaultValue;
	}

	public T Get<T>(string name, T defaultValue)
	{
		var attribute = node.Attribute(name);

		if (attribute is not null) return (T)Convert.ChangeType(attribute.Value, typeof(T));

		return defaultValue;
	}

	public void SetString(string name, string value)
	{
		Set(name, value);
	}

	public void SetString(string name, int value)
	{
		Set(name, value);
	}

	public void SetFloat(string name, float value)
	{
		Set(name, value);
	}

	public void Set<T>(string name, T value)
	{
		node.SetAttributeValue(name, value);
	}

	public string GetContent()
	{
		return node.Value;
	}

	public string GetContent(string defaultValue)
	{
		var value = node.Value;

		return String.IsNullOrEmpty(value) ? defaultValue : value;
	}

	public int GetIntContent()
	{
		return GetIntContent(default);
	}

	public int GetIntContent(int defaultValue)
	{
		return Int32.TryParse(GetContent(), out var result) ? result : defaultValue;
	}

	public float GetFloatContent()
	{
		return GetFloatContent(default);
	}

	public float GetFloatContent(float defaultValue)
	{
		if (Single.TryParse(GetContent(), out var result)) return result;

		return defaultValue;
	}

	public void SetContent(string text)
	{
		node.SetValue(text);
	}

	public string Format(int indent)
	{
		var settings = new XmlWriterSettings
		{
			Indent = indent >= 0,
			IndentChars = new String(' ', indent),
			NewLineChars = Environment.NewLine
		};

		var builder = new StringBuilder();
		using var xmlWriter = XmlWriter.Create(builder, settings);

		node.Save(xmlWriter);

		return builder.ToString();
	}

	public override string ToString()
	{
		return Format(2);
	}
}