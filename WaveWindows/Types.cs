using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WaveWindows;

internal class Types
{
	internal class ClientBehaviour
	{
		internal class ClientEmittedEventArgs : EventArgs
		{
			internal string OpCode { get; set; }

			internal object Data { get; set; }

			internal ClientEmittedEventArgs(string OpCode, object Data)
			{
				this.OpCode = OpCode;
				this.Data = Data;
			}
		}

		internal class ClientMessage
		{
			[JsonProperty("op")]
			internal string OpCode { get; set; }

			[JsonProperty("data")]
			internal object Data { get; set; }
		}

		internal class ClientIdentity
		{
			internal class ClientObject
			{
				[JsonProperty("id")]
				internal string Id { get; set; }

				[JsonProperty("name")]
				internal string Name { get; set; }
			}

			[JsonProperty("process")]
			internal ClientObject Process { get; set; }

			[JsonProperty("player")]
			internal ClientObject Player { get; set; }

			[JsonProperty("game")]
			internal ClientObject Game { get; set; }
		}

		internal class ClientScript
		{
			[JsonProperty("name")]
			internal string Name { get; set; }

			[JsonProperty("script")]
			internal string Script { get; set; }
		}

		internal class ClientError
		{
			[JsonProperty("type")]
			internal string Type { get; set; }

			[JsonProperty("error_code")]
			internal int Code { get; set; }

			[JsonProperty("error_message")]
			internal string Message { get; set; }

			[JsonProperty("logout_clear_session")]
			internal bool ClearSession { get; set; }
		}
	}

	internal class RobloxClientVersion
	{
		[JsonProperty("version")]
		internal string Version { get; set; }

		[JsonProperty("clientVersionUpload")]
		internal string Upload { get; set; }

		[JsonProperty("bootstrapperVersion")]
		internal string Bootstrapper { get; set; }
	}

	internal class RobloxThumbnail
	{
		internal class ThumbnailResponse
		{
			[JsonProperty("data")]
			internal List<ThumbnailData> Data { get; set; }
		}

		internal class ThumbnailData
		{
			[JsonProperty("targetId")]
			internal long TargetId { get; set; }

			[JsonProperty("state")]
			internal string State { get; set; }

			[JsonProperty("imageUrl")]
			internal string Image { get; set; }

			[JsonProperty("version")]
			internal string Version { get; set; }
		}
	}

	internal class WaveAPI
	{
		internal class User
		{
			[JsonProperty("id")]
			internal string Id { get; set; }

			[JsonProperty("products")]
			internal List<Product> Products { get; set; }
		}

		internal class Product
		{
			[JsonProperty("uuid")]
			internal string Id { get; set; }

			[JsonProperty("expiration")]
			internal long Timestamp { get; set; }

			[JsonProperty("product")]
			internal string Name { get; set; }
		}

		internal class ErrorResponse
		{
			[JsonProperty("code")]
			internal string Code { get; set; }

			[JsonProperty("message")]
			internal string Error { get; set; }

			[JsonProperty("userFacingMessage")]
			internal string Message { get; set; }
		}

		internal class PromptResponse
		{
			[JsonProperty("message")]
			internal string Message { get; set; }
		}

		internal class HistoryResponse
		{
			[JsonProperty("messages")]
			internal List<Message> Messages { get; set; }
		}

		internal class Message
		{
			[JsonProperty("role")]
			internal string Role { get; set; }

			[JsonProperty("content")]
			internal string Content { get; set; }
		}
	}
}
