using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Permissions;

namespace VAdmin.Commands
{
	public abstract class VAdminCommand
	{
		public abstract string Name { get; }

		public abstract string Description { get; }

		public abstract string Category { get; }

		public TargetDefault TargetDefault { get; set; } = TargetDefault.Relative;

		public abstract EchoType Echo { get; }

		public abstract CommandArg[] Args { get; }

		public abstract bool Execute( Client caller, string[] args, out string error );

		public abstract string FormatMessage( Client caller, string[] args );

		public string PermissionName { get; set; }

		public virtual Func<string[], int> GetPermissionLevel { get; }

		public virtual void OnExecuteFailure(Client caller, string[] args, string error)
		{

		}

		/// <summary>
		/// Evaluates the required permission level to execute this command.
		/// </summary>
		/// <param name="args">The arguments from the caller. These are assumed to be valid for this commands arg requirement.</param>
		/// <returns>The required level to execute this command.</returns>
		public int EvaluateRequiredPermissionLevel( string[] args, Client caller )
		{
			// This method should only be called with VALID args.

			if ( this.GetPermissionLevel != null )
			{
				return this.GetPermissionLevel( args );
			}

			for ( var i = 0; i < this.Args.Length; i++ )
			{
				var param = this.Args[i];
				var arg = args[i];

				if ( param.Type == CommandArgType.Client )
				{
					var clients = VAdminSystem.FindClients( arg, caller );
					if ( clients .Length > 1)
					{
						throw new InvalidOperationException( "Attempted to evaluate permission level for multiple clients" );
					}

					var user = VAdminSystem.GetUserInfo( clients[0] );
					return user.GetHighestLevel();
				}
				else if ( param.Type == CommandArgType.Role )
				{
					VAdminSystem.FindRoleByName( arg, out var role );
					return role.Level;
				}
			}

			return 0;
		}
	}
}
