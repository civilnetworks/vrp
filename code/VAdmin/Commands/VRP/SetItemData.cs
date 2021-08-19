using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Commands;
using VRP;
using VRP.Items;

namespace VAdmin.Commands
{
	class SetItemData : VAdminCommand
	{
		public override string Name => "set_item_data";

		public override string Description => "Sets item data on a VrpItemEntity";

		public override string Category => "VRP";

		public override EchoType Echo => EchoType.ToSender;

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("key", CommandArgType.String),
			new CommandArg("value", CommandArgType.String)
		};

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var tr = VrpSystem.GetEyeTrace( caller );

			if ( !tr.Hit )
			{
				error = "Trace hit nothing";
				return false;
			}

			var hit = tr.Entity;
			if ( hit == null )
			{
				error = "Trace hit null entity";
				return false;
			}

			if ( !(hit is VrpItemEntity) )
			{
				error = "Must look at a VrpItemEntity";
				return false;
			}

			var itemEnt = (VrpItemEntity)hit;

			itemEnt.Item.SetData( args[0], VrpItems.ParseItemDataString( args[1] ) );

			error = null;
			return true;
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			var tr = VrpSystem.GetEyeTrace( caller );
			var hit = (VrpItemEntity)tr.Entity;

			return $"Set data '{args[0]}' to '{args[1]}' on {hit}";
		}
	}
}
