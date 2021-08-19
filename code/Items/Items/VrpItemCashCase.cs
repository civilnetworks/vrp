using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vrp.Items;
using VRP.Finance;
using VRP.Items.Interacts;

namespace VRP.Items
{
	// Library title must match item Id
	[Library( Title = "cash_case" )]
	public class VrpItemCashCase : VrpItem
	{
		private const double CAPACITY = 100000;

		public override string Id => "cash_case";

		public override string Name => "Money Case";

		public override bool Carryable => true;

		public override bool DisplayHint => true;

		public override VrpItemHintItem[] HintItems => new VrpItemHintItem[]
		{
			new VrpItemHintItem()
			{
				DisplayTextFunc = (item) =>
				{
					if (item.GetBool("open"))
					{
						return FinanceManager.FormatMoney(item.GetDouble("money"));
					}
					else
					{
						return FinanceManager.FormatMoney("???");
					}
				},
			}
		};

		public override ItemDataTemplate[] DataTemplates => new ItemDataTemplate[] {
			new ItemDataTemplate("money", 0, true),
			new ItemDataTemplate("capacity", CAPACITY, true),
			new ItemDataTemplate("open", false, true)
		};

		public override string ModelPath => "models/civilgamers/money_case/closed_case.vmdl";

		public override Func<string> ModelPathFunc => () =>
		{
			if ( !(bool)this.GetData( "open", false ) )
			{
				return "models/civilgamers/money_case/closed_case.vmdl";
			}

			var money = (double)this.GetData( "money", 0 );
			var cap = (double)this.GetData( "capacity", CAPACITY );
			var perc = money / cap;

			if ( perc == 0 )
			{
				return "models/civilgamers/money_case/open_case.vmdl";
			}
			else if ( perc < 0.35f )
			{
				return "models/civilgamers/money_case/open_case_1.vmdl";
			}
			else if ( perc < 0.85f )
			{
				return "models/civilgamers/money_case/open_case_2.vmdl";
			}
			else
			{
				return "models/civilgamers/money_case/open_case_3.vmdl";
			}
		};

		protected override void AddInteractions()
		{
			this.AddInteraction( new OpenCloseInteraction() );
			this.AddInteraction( new LockUnlockInteraction() );
			this.AddInteraction( new ClaimUnclaimInteraction() );

			this.AddInteraction( new GenericInteraction( "take", "Take Stack", "materials/vrp/interactions/cash_stack.png", ( client, ent, args ) =>
			  {
				  var amt = double.Parse( args[0] );
				  if ( amt <= 0 )
				  {
					  return new VrpItemInteractResult( false, $"Amount cannot be zero" );
				  }

				  var money = this.GetDouble( "money" );
				  if ( amt > money )
				  {
					  return new VrpItemInteractResult( false, "Insufficient money in case" );
				  }

				  if ( !FinanceManager.TryCreateDroppedCash( amt, ent.WorldSpaceBounds.Center + Vector3.Up * 20, out var cashEnt, out var error ) )
				  {
					  return new VrpItemInteractResult( false, error );
				  }

				  ent.Item.SetData( "money", money - amt );

				  if ( amt >= VrpItemCash.STACK_REQUIREMENT )
				  {
					  cashEnt.Item.SetData( "stack", true );
				  }
				  else
				  {
					  cashEnt.Position = ent.WorldSpaceBounds.Center + Vector3.Up * 20 + ent.Transform.Rotation.Forward * 20;
				  }

				  return new VrpItemInteractResult( true );
			  }, null )
			{
				Args = new VrpItemInteractArg[]
				{
					new VrpItemInteractArg( "Amount", VrpItemInteractArgType.Number )
				},
				CanInteractFunc = ( client, ent ) =>
				{
					if ( !ent.Item.GetBool( "open" ) )
					{
						return new VrpItemInteractResult( false, "Case is closed" );
					}

					return new VrpItemInteractResult( true );
				},
			} );
		}

		public override void OnPhysicsCollision( VrpItemEntity ent, CollisionEventData eventData )
		{
			base.OnPhysicsCollision( ent, eventData );

			if ( Host.IsClient )
			{
				return;
			}

			if ( !this.GetBool( "open" ) )
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

				var amtToPut = Math.Min( otherMoney, this.GetDouble( "capacity" ) - thisMoney );

				if ( amtToPut > 0 )
				{
					// Merge dropped money
					cash.SetData( "money", otherMoney - amtToPut );
					this.SetData( "money", thisMoney + amtToPut );

					if ( cash.GetDouble( "money" ) <= 0 )
					{
						itemEnt.Delete();
					}
				}
			}
		}
	}
}
