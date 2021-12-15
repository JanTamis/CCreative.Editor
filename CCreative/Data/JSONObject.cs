using System.Text.Json;

#pragma warning disable CS1591

namespace CCreative.Data;

public class JsonObject
{
	private readonly JsonElement element;

	protected JsonObject(JsonElement jsonElement)
	{
		element = jsonElement;
	}

	public string? GetString(string key)
	{
		return element
			.GetProperty(key)
			.GetString();
	}

	public int GetInt(string key)
	{
		return element.GetProperty(key)
			.GetInt32();
	}

	public float GetFloat(string key)
	{
		return element.GetProperty(key)
			.GetSingle();
	}

	public bool GetBoolean(string key)
	{
		return element.GetProperty(key)
			.GetBoolean();
	}

	public JsonObject GetJsonObject(string key)
	{
		return new JsonObject(element.GetProperty(key));
	}

	public bool IsNull(string key)
	{
		return element.GetProperty(key)
			.ValueKind == JsonValueKind.Null;
	}
}