using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using RestSharp;

namespace WaveWindows.Interfaces;

internal class ScriptBloxInterface
{
	internal static RestClient Client = new RestClient("https://scriptblox.com/api", (ConfigureRestClient)null, (ConfigureHeaders)null, (ConfigureSerialization)null);

	internal static async Task<SearchResult> Search(string query, int page)
	{
		RestRequest request = RestRequestExtensions.AddParameter(RestRequestExtensions.AddParameter<int>(new RestRequest("script/fetch", (Method)0), "page", page, true), "q", query, true);
		RestResponse response = await RestClientExtensions.GetAsync((IRestClient)(object)Client, request, default(CancellationToken));
		if (!((RestResponseBase)response).IsSuccessStatusCode)
		{
			throw new HttpRequestException(((RestResponseBase)response).ErrorMessage);
		}
		SearchResponse result = JsonConvert.DeserializeObject<SearchResponse>(((RestResponseBase)response).Content, new JsonSerializerSettings
		{
			NullValueHandling = (NullValueHandling)1,
			MissingMemberHandling = (MissingMemberHandling)0
		});
		return result.Result;
	}

	internal static async Task<Script> GetScript(Script script)
	{
		RestRequest request = new RestRequest("script/" + script.Slug, (Method)0);
		RestResponse response = await RestClientExtensions.GetAsync((IRestClient)(object)Client, request, default(CancellationToken));
		if (!((RestResponseBase)response).IsSuccessStatusCode)
		{
			throw new HttpRequestException(((RestResponseBase)response).ErrorException.ToString());
		}
		ScriptResult result = JsonConvert.DeserializeObject<ScriptResult>(((RestResponseBase)response).Content, new JsonSerializerSettings
		{
			NullValueHandling = (NullValueHandling)1,
			MissingMemberHandling = (MissingMemberHandling)0
		});
		return result.Script;
	}

	internal static string GetImageUrl(string Image)
	{
		return Image.StartsWith("https://tr.rbxcdn.com") ? Image : ("https://scriptblox.com" + Image);
	}

	internal static ImageSource ToImage(string url)
	{
		BitmapImage bitmapImage = new BitmapImage();
		bitmapImage.BeginInit();
		bitmapImage.UriSource = new Uri(url);
		bitmapImage.EndInit();
		return bitmapImage;
	}

	internal static async Task<string> ImageToBase64(string image)
	{
		HttpClient client = new HttpClient();
		try
		{
			return "data:image/webp;base64," + Convert.ToBase64String(await client.GetByteArrayAsync(image));
		}
		finally
		{
			((IDisposable)client)?.Dispose();
		}
	}

	internal static ImageSource Base64ToImage(string base64)
	{
		BitmapImage bitmapImage = new BitmapImage();
		bitmapImage.BeginInit();
		bitmapImage.StreamSource = new MemoryStream(Convert.FromBase64String(base64));
		bitmapImage.EndInit();
		return bitmapImage;
	}
}
