using System.Text.Json;

#pragma warning disable CS1591

namespace CCreative.Data;

public class JsonArray : JsonObject
{
	internal JsonArray(JsonElement element) : base(element)
	{
	}
}