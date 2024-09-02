using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WaveWindows.Modules.Behaviour;

internal class ClientBehaviour : WebSocketBehavior
{
	private static Dictionary<string, WebSocket> Clients = new Dictionary<string, WebSocket>();

	private static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
	{
		NullValueHandling = (NullValueHandling)1,
		MissingMemberHandling = (MissingMemberHandling)0
	};

	private static Dictionary<string, List<object>> WaitForCodes = new Dictionary<string, List<object>>();

	internal string Service { get; set; }

	private void Emit(string OpCode, object Data)
	{
		if (string.IsNullOrEmpty(Service))
		{
			throw new NullReferenceException("Service is not set");
		}
		SocketServer.Emit(Service, new Types.ClientBehaviour.ClientEmittedEventArgs(OpCode, Data));
	}

	private void TryAddToCode<T>(string code, T data)
	{
		if (WaitForCodes.ContainsKey(code))
		{
			WaitForCodes[code].Add(data);
		}
	}

    protected override void OnMessage(MessageEventArgs e)
    {
        base.OnMessage(e); // Call the base class's OnMessage method if ClientBehaviour directly inherits from WebSocketBehavior

        // Deserialize client message
        Types.ClientBehaviour.ClientMessage clientMessage = JsonConvert.DeserializeObject<Types.ClientBehaviour.ClientMessage>(e.Data, JsonSerializerSettings);
        string text = clientMessage.Data.ToString();

        switch (clientMessage.OpCode)
        {
            case "OP_IDENTIFY":
                {
                    Types.ClientBehaviour.ClientIdentity clientIdentity = JsonConvert.DeserializeObject<Types.ClientBehaviour.ClientIdentity>(text, JsonSerializerSettings);
                    if (Clients.FirstOrDefault((KeyValuePair<string, WebSocket> x) => x.Value == Context.WebSocket).Value != null)
                    {
                        Emit("OP_UPDATE", clientIdentity);
                        break;
                    }
                    TryAddToCode("OP_IDENTIFY", clientIdentity);
                    Clients.Add(clientIdentity.Process.Id, Context.WebSocket);
                    Emit("OP_IDENTIFY", clientIdentity);
                    break;
                }
            case "OP_HEARTBEAT":
                {
                    KeyValuePair<string, WebSocket> keyValuePair = Clients.FirstOrDefault((KeyValuePair<string, WebSocket> x) => x.Value == Context.WebSocket);
                    if (keyValuePair.Value == null)
                    {
                        Context.WebSocket.CloseAsync();
                        break;
                    }
                    WebSocket value = keyValuePair.Value;
                    value.Send(JsonConvert.SerializeObject((object)new Types.ClientBehaviour.ClientMessage
                    {
                        OpCode = "OP_HEARTBEAT_ACK",
                        Data = new { }
                    }));
                    break;
                }
            case "OP_SCRIPT":
                break;
            default:
                {
                    if (Clients.FirstOrDefault((KeyValuePair<string, WebSocket> x) => x.Value == Context.WebSocket).Value == null)
                    {
                        Context.WebSocket.CloseAsync();
                    }
                    else
                    {
                        Emit(clientMessage.OpCode, JsonConvert.DeserializeObject<Types.ClientBehaviour.ClientError>(text));
                    }
                    break;
                }
        }
    }

    protected override void OnClose(CloseEventArgs e)
    {
        base.OnClose(e); // Call the base class's OnClose method

        KeyValuePair<string, WebSocket> keyValuePair = Clients.FirstOrDefault((KeyValuePair<string, WebSocket> x) => x.Value == Context.WebSocket);
        if (keyValuePair.Value != null)
        {
            Clients.Remove(keyValuePair.Key);
            Emit("OP_DISCONNECT", keyValuePair.Key);
        }
    }


    public static string[] GetAllClients()
	{
		return Clients.Keys.ToArray();
	}

	public static WebSocket GetClient(string name)
	{
		return Clients[name];
	}

	public static async Task<List<T>> WaitFor<T>(string code, int seconds)
	{
		if (!WaitForCodes.ContainsKey(code))
		{
			WaitForCodes.Add(code, new List<object>());
		}
		await Task.Delay(seconds * 1000);
		List<T> result = WaitForCodes[code].Cast<T>().ToList();
		WaitForCodes.Remove(code);
		return result;
	}

	public static async Task ExecuteScriptAsync(string name, int id, string data)
	{
		WebSocket client = GetClient(name);
		if (client != null && client.IsAlive && (int)client.ReadyState == 1)
		{
			string json = JsonConvert.SerializeObject((object)new Types.ClientBehaviour.ClientMessage
			{
				OpCode = "OP_EXECUTE",
				Data = new
				{
					id = id,
					source = data
				}
			});
			await Task.Run(delegate
			{
				client.Send(json);
			});
		}
	}
}
