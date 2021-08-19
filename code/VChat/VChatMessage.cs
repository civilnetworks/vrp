using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VChat
{
	public class VChatMessage
	{
		private bool _abort;

		[JsonIgnore]
		public Client Client { get; set; }

		public string SenderName { get; set; }

		public ulong? SenderSteamId { get; set; }

		public string Message { get; set; }

		public To To { get; set; }

		public bool Abort
			=> _abort;

		public void AbortMessage()
		{
			_abort = true;
		}
	}
}
