using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Items.Interacts
{
	public abstract class VrpItemInteraction
	{
		public VrpItemInteraction( string key, string name )
		{
			this.Key = key;
			this.Name = name;
		}

		public string Key { get; }

		public string Name { get; }

		public virtual Color Color { get; } = Color.White;

		public virtual bool VisibleInInteractMenu { get; } = true;

		public virtual bool RequiresClaim { get; }

		public Func<Client, VrpItemEntity, VrpItemInteractResult> CanInteractFunc { get; set; }

		public virtual VrpItemInteractArg[] Args { get; set; } = new VrpItemInteractArg[] { };

		public virtual string GetDisplayName( Client caller, VrpItemEntity ent )
		{
			return this.Name;
		}

		public abstract string GetIconPath( Client caller, VrpItemEntity ent );

		public abstract VrpItemInteractResult Interact( Client caller, VrpItemEntity ent, string[] args );

		public abstract void ClientInteract( Client caller, VrpItemEntity ent, string[] args );
	}
}
