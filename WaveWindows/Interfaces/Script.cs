using Newtonsoft.Json;

namespace WaveWindows.Interfaces;

internal class Script
{
	[JsonProperty("title")]
	internal string Title { get; set; }

	[JsonProperty("game")]
	internal ScriptGame Game { get; set; }

	[JsonProperty("verified")]
	internal bool Verified { get; set; }

	[JsonProperty("key")]
	internal bool HasKey { get; set; }

	[JsonProperty("isPatched")]
	internal bool Patched { get; set; }

	[JsonProperty("script")]
	internal string Source { get; set; }

	[JsonProperty("slug")]
	internal string Slug { get; set; }
}
