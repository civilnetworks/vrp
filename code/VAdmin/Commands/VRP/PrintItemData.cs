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
	public class PrintItemData : VAdminCommand
	{
		public override string Name => "print_item_data";

		public override string Description => "Prints data for the VrpItemEntity you are looking at.";

		public override string Category => "VRP";

		public override EchoType Echo => EchoType.Silent;

		public override CommandArg[] Args => new CommandArg[] { };

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

			VrpSystem.SendChatMessage( $"Item data for {itemEnt} [{itemEnt.Item.Data.Count}]" );

			foreach (var pair in itemEnt.Item.Data)
			{
				VrpSystem.SendChatMessage( $"{pair.Key} : {pair.Value}" );
			}

			error = null;
			return true;
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			return null;
		}
	}
}
