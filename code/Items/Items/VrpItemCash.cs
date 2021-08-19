using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vrp.Items;
using VRP.Finance;
using VRP.Finance.Mediums;
using VRP.Items.Interacts;

namespace VRP.Items
{
	// Library title must match item Id
	[Library( Title = "cash" )]
	class VrpItemCash : VrpItem
	{
		public const double CAPACITY = 5000;
		public const double STACK_REQUIREMENT = 100;

		public override string Id => "cash";

		public override string Name => "Cash";

		public override bool Carryable => true;

		public override VrpItemHintItem[] HintItems => new VrpItemHintItem[]
		{
			new VrpItemHintItem()
			{
				DisplayTextFunc = (item) =>
				{
					return FinanceManager.FormatMoney(item.GetDouble("money"));
				},
			}
		};

		public override ItemDataTemplate[] DataTemplates => new ItemDataTemplate[]
		{
			new ItemDataTemplate("stack", false, true),
			new ItemDataTemplate("money", 0, true),
			new ItemDataTemplate("capacity", CAPACITY),
		};

		public override Func<string> NameFunc => () =>
		{
			if ( this.GetBool( "stack" ) )
			{
				return "Stack of Cash";
			}

			return "Cash";
		};

		public override string ModelPath => "models/thruster/thrusterprojector.vmdl";

		public override Func<string> ModelPathFunc => () =>
		{
			if ( this.GetBool( "stack" ) )
			{
				return "models/civilgamers/cash/dollarstack.vmdl";
			}

			return "models/civilgamers/cash/moneyroll.vmdl";
		};

		public override void Initialize( VrpItemEntity ent )
		{
			base.Initialize( ent );

			//ent.CollisionGroup = CollisionGroup.Debris;
		}

		public override void Tick( VrpItemEntity ent )
		{
			base.Tick( ent );

			if ( this.GetDouble( "money" ) <= 0 )
			{
				ent.Delete();
			}
		}

		public override void OnPhysicsCollision( VrpItemEntity ent, CollisionEventData eventData )
		{
			base.OnPhysicsCollision( ent, eventData );

			if ( Host.IsClient )
			{
				return;
			}

			if ( this.GetBool( "stack" ) )
			{
				return;
			}

			if ( !eventData.Entity?.IsValid() ?? false )
			{
				return;
			}

			if ( eventData.Entity is VrpItemEntity itemEnt && itemEnt.Item is VrpItemCash cash )
			{
				if ( cash.GetBool( "stack" ) )
				{
					return;
				}

				var thisMoney = this.GetDouble( "money" );
				var otherMoney = cash.GetDouble( "money" );
				if ( otherMoney <= 0 )
				{
					return;
				}

				if ( thisMoney + otherMoney <= cash.GetDouble( "capacity" ) )
				{
					// Merge dropped money
					cash.SetData( "money", otherMoney + thisMoney );
					this.SetData( "money", 0 );
					ent.Delete();
				}
			}
		}

		protected override void AddInteractions()
		{
			base.AddInteractions();

			this.AddInteraction( new GenericInteraction( "take", "Take", "materials/vrp/interactions/pickup.png", ( client, ent, args ) =>
			  {
				  var amt = this.GetDouble( "money" );

				  if ( !FinanceManager.GetBalance( client, typeof( WalletMedium ), out var balance, out var balanceErr ) )
				  {
					  return new VrpItemInteractResult( false, balanceErr );
				  }

				  var medium = FinanceManager.GetMedium( typeof( WalletMedium ) );
				  var toTake = Math.Min( amt, medium.MaximumBalance - balance );

				  if ( toTake <= 0 )
				  {
					  return new VrpItemInteractResult( false, "No space in wallet" );
				  }

				  if ( FinanceManager.Deposit( client, typeof( WalletMedium ), toTake, out var error ) )
				  {
					  this.SetData( "money", amt - toTake );

					  if ( this.GetDouble( "money" ) <= 0 )
					  {
						  ent.Delete();
					  }
					  else
					  {
						  VrpSystem.SendChatMessage( To.Single( client ), $"You only had space to pickup {FinanceManager.FormatMoney( toTake )}" );
					  }

					  return new VrpItemInteractResult( true );
				  }
				  else
				  {
					  return new VrpItemInteractResult( false, error );
				  }
			  }, null ) );

			// Stack/Unstack
			this.AddInteraction( new ToggleInteraction( "stack", "Stack", "Unstack", "Stack" )
			{
				DataToggleKey = "stack",
				OnIconPath = "materials/vrp/interactions/cash_stack.png",
				OffIconPath = "materials/vrp/interactions/cash_unstack.png",
				InteractFunc = ( caller, ent, newValue ) =>
				{
					if ( newValue && this.GetDouble( "money" ) < STACK_REQUIREMENT )
					{
						return new VrpItemInteractResult( false, $"Cannot stack less than {FinanceManager.FormatMoney( STACK_REQUIREMENT )}" );
					}

					return null;
				}
			} );
		}
	}
}
