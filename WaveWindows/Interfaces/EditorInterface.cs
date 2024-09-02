using Newtonsoft.Json;

namespace WaveWindows.Interfaces;

internal class EditorInterface
{
	internal class MinimapOptions
	{
		[JsonProperty("enabled")]
		internal bool Enabled { get; set; }
	}

	internal class InlayHintsOptions
	{
		[JsonProperty("enabled")]
		internal bool Enabled { get; set; }
	}

	internal class EditorOptions
	{
		[JsonProperty("minimap")]
		internal MinimapOptions Minimap { get; set; }

		[JsonProperty("inlayHints")]
		internal InlayHintsOptions InlayHints { get; set; }

		[JsonProperty("fontSize")]
		internal int FontSize { get; set; }

		public bool ShouldSerializeFontSize()
		{
			return FontSize != 0;
		}

		public override string ToString()
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			return JsonConvert.SerializeObject((object)this, (Formatting)1, new JsonSerializerSettings
			{
				NullValueHandling = (NullValueHandling)1
			});
		}
	}
}
