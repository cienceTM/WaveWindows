using Newtonsoft.Json;

namespace WaveWindows.Interfaces;

internal class ScriptResult
{
	[JsonProperty("script")]
	internal Script Script { get; set; }
}
