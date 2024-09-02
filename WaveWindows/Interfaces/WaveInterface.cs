using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace WaveWindows.Interfaces;

internal static class WaveInterface
{
	private static readonly RestClient Client = new RestClient("https://api.getwave.gg/v1", (ConfigureRestClient)null, (ConfigureHeaders)null, (ConfigureSerialization)null);

	internal static async Task<string> Login(string identity, string password)
	{
		RestRequest request = RestRequestExtensions.AddHeader(new RestRequest("auth/login", (Method)1), "Content-Type", "application/json");
		RestRequestExtensions.AddJsonBody<string>(request, JsonConvert.SerializeObject((object)new { identity, password }), (ContentType)null);
		RestResponse response = await Client.ExecuteAsync(request, default(CancellationToken));
		if (((RestResponseBase)response).StatusCode == (HttpStatusCode)429)
		{
			throw new Exception("You have exceeded the rate limit of requests. Please try again later.");
		}
		if (((RestResponseBase)response).StatusCode != HttpStatusCode.OK)
		{
			ThrowError(((RestResponseBase)response).Content);
		}
		return ((Parameter)((RestResponseBase)response).Headers.First((HeaderParameter header) => ((Parameter)header).Name == "authorization")).Value.ToString();
	}

	internal static async Task Register(string username, string email, string password)
	{
		RestRequest request = RestRequestExtensions.AddHeader(new RestRequest("auth/register", (Method)1), "Content-Type", "application/json");
		RestRequestExtensions.AddJsonBody<string>(request, JsonConvert.SerializeObject((object)new { username, email, password }), (ContentType)null);
		RestResponse response = await Client.ExecuteAsync(request, default(CancellationToken));
		if (((RestResponseBase)response).StatusCode == (HttpStatusCode)429)
		{
			throw new Exception("You have exceeded the rate limit of requests. Please try again later.");
		}
		if (((RestResponseBase)response).StatusCode != HttpStatusCode.OK)
		{
			ThrowError(((RestResponseBase)response).Content);
		}
	}

	internal static async Task<Types.WaveAPI.User> GetUserAsync(string session)
	{
		RestRequest request = RestRequestExtensions.AddHeader(new RestRequest("user", (Method)0), "Authorization", session);
		RestResponse response = await Client.ExecuteAsync(request, default(CancellationToken));
		if (((RestResponseBase)response).StatusCode == (HttpStatusCode)429)
		{
			throw new Exception("You have exceeded the rate limit of requests. Please try again later.");
		}
		if (((RestResponseBase)response).StatusCode != HttpStatusCode.OK)
		{
			ThrowError(((RestResponseBase)response).Content);
		}
		return JsonConvert.DeserializeObject<Types.WaveAPI.User>(((RestResponseBase)response).Content);
	}

	internal static async Task RequestPasswordReset(string identity)
	{
		RestRequest request = RestRequestExtensions.AddHeader(new RestRequest("/auth/password/forgot", (Method)1), "Content-Type", "application/json");
		RestRequestExtensions.AddJsonBody<string>(request, JsonConvert.SerializeObject((object)new { identity }), (ContentType)null);
		RestResponse response = await Client.ExecuteAsync(request, default(CancellationToken));
		if (((RestResponseBase)response).StatusCode == (HttpStatusCode)429)
		{
			throw new Exception("You have exceeded the rate limit of requests. Please try again later.");
		}
		if (((RestResponseBase)response).StatusCode != HttpStatusCode.OK)
		{
			ThrowError(((RestResponseBase)response).Content);
		}
	}

	internal static async Task RequestEmailVerification(string identity)
	{
		RestRequest request = RestRequestExtensions.AddHeader(new RestRequest("auth/email/request", (Method)1), "Content-Type", "application/json");
		RestRequestExtensions.AddJsonBody<string>(request, JsonConvert.SerializeObject((object)new { identity }), (ContentType)null);
		RestResponse response = await Client.ExecuteAsync(request, default(CancellationToken));
		if (((RestResponseBase)response).StatusCode == (HttpStatusCode)429)
		{
			throw new Exception("You have exceeded the rate limit of requests. Please try again later.");
		}
		if (((RestResponseBase)response).StatusCode != HttpStatusCode.OK)
		{
			ThrowError(((RestResponseBase)response).Content);
		}
	}

	internal static async Task RedeemAsync(string session, string license)
	{
		RestRequest request = RestRequestExtensions.AddHeader(RestRequestExtensions.AddHeader(new RestRequest("subscription/claim", (Method)1), "Content-Type", "application/json"), "Authorization", session);
		RestRequestExtensions.AddJsonBody<string>(request, JsonConvert.SerializeObject((object)new
		{
			licenseKey = license
		}), (ContentType)null);
		RestResponse response = await Client.ExecuteAsync(request, default(CancellationToken));
		if (((RestResponseBase)response).StatusCode == (HttpStatusCode)429)
		{
			throw new Exception("You have exceeded the rate limit of requests. Please try again later.");
		}
		if (((RestResponseBase)response).StatusCode != HttpStatusCode.OK)
		{
			ThrowError(((RestResponseBase)response).Content);
		}
	}

	internal static async Task<Types.WaveAPI.PromptResponse> SendPromptAsync(string session, string message)
	{
		RestRequest request = RestRequestExtensions.AddHeader(RestRequestExtensions.AddHeader(new RestRequest("ai/chat/prompt", (Method)1), "Content-Type", "application/json"), "Authorization", session);
		RestRequestExtensions.AddJsonBody<string>(request, JsonConvert.SerializeObject((object)new { message }), (ContentType)null);
		RestResponse response = await Client.ExecuteAsync(request, default(CancellationToken));
		if (((RestResponseBase)response).StatusCode == (HttpStatusCode)429)
		{
			throw new Exception("You have exceeded the rate limit of requests. Please try again later.");
		}
		if (((RestResponseBase)response).StatusCode != HttpStatusCode.OK)
		{
			ThrowError(((RestResponseBase)response).Content);
		}
		return JsonConvert.DeserializeObject<Types.WaveAPI.PromptResponse>(((RestResponseBase)response).Content);
	}

	internal static async Task<Types.WaveAPI.HistoryResponse> FetchHistoryAsync(string session)
	{
		RestRequest request = RestRequestExtensions.AddHeader(new RestRequest("ai/chat/history", (Method)0), "Authorization", session);
		RestResponse response = await Client.ExecuteAsync(request, default(CancellationToken));
		if (((RestResponseBase)response).StatusCode == (HttpStatusCode)429)
		{
			throw new Exception("You have exceeded the rate limit of requests. Please try again later.");
		}
		if (((RestResponseBase)response).StatusCode != HttpStatusCode.OK)
		{
			ThrowError(((RestResponseBase)response).Content);
		}
		return JsonConvert.DeserializeObject<Types.WaveAPI.HistoryResponse>(((RestResponseBase)response).Content);
	}

	internal static async Task ClearHistoryAsync(string session)
	{
		RestRequest request = RestRequestExtensions.AddHeader(RestRequestExtensions.AddHeader(new RestRequest("ai/chat", (Method)3), "Content-Type", "application/json"), "Authorization", session);
		RestResponse response = await Client.ExecuteAsync(request, default(CancellationToken));
		if (((RestResponseBase)response).StatusCode == (HttpStatusCode)429)
		{
			throw new Exception("You have exceeded the rate limit of requests. Please try again later.");
		}
		if (((RestResponseBase)response).StatusCode != HttpStatusCode.OK)
		{
			ThrowError(((RestResponseBase)response).Content);
		}
	}

	internal static void ThrowError(string responseContent)
	{
		Types.WaveAPI.ErrorResponse error = ParseError(responseContent);
		throw new Exception(GetError(error));
	}

	internal static Types.WaveAPI.ErrorResponse ParseError(string error)
	{
		try
		{
			return JsonConvert.DeserializeObject<Types.WaveAPI.ErrorResponse>(error);
		}
		catch
		{
			throw new Exception("Unable to establish a connection with the servers.");
		}
	}

	internal static string GetError(Types.WaveAPI.ErrorResponse error)
	{
		return error.Code switch
		{
			"server#0001" => "An internal server error has occurred, please try again later.", 
			"server#0002" => "Access has been temporarily restricted due to exceeding the rate limit of requests.", 
			"session#0001" => "Your current session has become invalid. Please log in again.", 
			"session#0002" => "Please verify your email address. Try check your spam or junk folders.", 
			"session#0003" => "Your account has been terminated. Please seek support for further assistance.", 
			"user#0001" => "The user does not exist or has been terminated.", 
			"user#0002" => "A user with that username already exists.", 
			"user#0003" => "A user with that email already exists.", 
			"user#0004" => "Usernames can only contain alphanumeric characters and one underscore.", 
			"user#0005" => "Usernames should be between 3 and 16 characters long.", 
			"user#0006" => "The email provided is invalid. Please check your spelling.", 
			"user#0007" => "The email provided exceeds RFC standards.", 
			"user#0008" => "Your username/email or password is incorrect.", 
			"user#0009" => "Your account lacks permission to access this content.", 
			"user#0010" => "The email has already been verified for this account.", 
			"user#0011" => "Your authentication has failed. Please try again.", 
			"subscription#0001" => "The provided license key is invalid, does not exist, or has expired.", 
			"subscription#0002" => "The license key has already been redeemed.", 
			"subscription#0003" => "An additional license cannot be claimed as this account holds an indefinite license.", 
			"subscription#0004" => "The expiration date cannot be set in the past; it must be in the future.", 
			"subscription#0005" => "The specified product does not exist or is invalid.", 
			"token#0001" => "Your current session has become invalid. Please log in again.", 
			_ => "An unknown error has occurred. Please try again later.", 
		};
	}
}
