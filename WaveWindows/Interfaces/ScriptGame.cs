using Newtonsoft.Json;

namespace WaveWindows.Interfaces;

internal class ScriptGame
{
	[JsonProperty("name")]
	internal string Name { get; set; }

	[JsonProperty("imageUrl")]
	internal string Image { get; set; }
}
