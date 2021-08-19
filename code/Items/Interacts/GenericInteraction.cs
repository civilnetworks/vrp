using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Items.Interacts
{
	public class GenericInteraction : VrpItemInteraction
	{
		private string _iconPath;

		public GenericInteraction( string key, string name, string iconPath,
			Func<Client, VrpItemEntity, string[], VrpItemInteractResult> serverCallback,
			Func<Client, VrpItemEntity, string[], VrpItemInteractResult> clientCallback ) : base( key, name )
		{
			_iconPath = iconPath;
			this.ServerCallback = serverCallback;
			this.ClientCallback = clientCallback;
		}

		public Func<Client, VrpItemEntity, string[], VrpItemInteractResult> ServerCallback { get; }

		public Func<Client, VrpItemEntity, string[], VrpItemInteractResult> ClientCallback { get; }

		public override string GetIconPath( Client caller, VrpItemEntity ent )
		{
			return _iconPath;
		}

		public override void ClientInteract( Client caller, VrpItemEntity ent, string[] args )
		{
			this.ClientCallback?.Invoke( caller, ent, args );
		}

		public override VrpItemInteractResult Interact( Client caller, VrpItemEntity ent, string[] args )
		{
			if ( this.ServerCallback == null)
			{
				return new VrpItemInteractResult( true );
			}

			return this.ServerCallback.Invoke( caller, ent, args );
		}
	}
}
