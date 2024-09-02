using Newtonsoft.Json;

namespace WaveWindows.Interfaces;

internal class SearchResponse
{
	[JsonProperty("result")]
	internal SearchResult Result { get; set; }
}
