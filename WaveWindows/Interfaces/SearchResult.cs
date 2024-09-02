using System.Collections.Generic;
using Newtonsoft.Json;

namespace WaveWindows.Interfaces;

internal class SearchResult
{
	[JsonProperty("totalPages")]
	internal int TotalPages { get; set; }

	[JsonProperty("nextPage")]
	internal int NextPage { get; set; }

	[JsonProperty("scripts")]
	internal List<Script> Scripts { get; set; }
}
