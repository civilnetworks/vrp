using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Items;
using VRP.Items.Interacts;
using VRP.Player;

namespace VRP.Weapons
{
	[Library( "weapon_keys", Title = "Keys", Spawnable = true )]
	partial class Keys : VrpBaseWeapon
	{
		private const float UNLOCK_RATE = 1f / 1.2f;
		private const float UNLOCK_DELAY = 0.63f;

		private VrpItemEntity _unlockingItem;
		private bool _unlocking;
		private float _unlockTime;
		private Sound _lockSound;

		public override float PrimaryRate => UNLOCK_RATE;

		public override float SecondaryRate => UNLOCK_RATE;

		public override string ViewModelPath => "models/civilgamers/weapons/v_weapon_keys.vmdl";

		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/civilgamers/weapons/v_weapon_keys.vmdl" );
		}

		public override void SimulateAnimator( PawnAnimator anim )
		{
			anim.SetParam( "holdtype", 0 );
			anim.SetParam( "aimat_weight", 0f );
		}

		[Event.Tick]
		public void OnTick()
		{
			if ( Host.IsClient )
			{
				return;
			}

			if ( _unlockingItem != null )
			{
				if ( _unlockTime <= Time.Now )
				{
					if ( LockUnlockInteraction.IsLocked( _unlockingItem ) == _unlocking )
					{
						if ( !_unlockingItem.Item.Interact( this.GetClientOwner(), _unlockingItem, typeof( LockUnlockInteraction ), out var error ) )
						{
							VrpSystem.SendChatMessage( To.Single( this.GetClientOwner() ), error );
							this.RpcOnAbort();
						}
					}

					_unlockingItem = null;
				}
				else if ( this.FindLockableItem() != _unlockingItem )
				{
					this.RpcOnAbort();
					_unlockingItem = null;
				}
			}
		}

		public override void CreateViewModel()
		{
			base.CreateViewModel();
		}

		public override bool CanReload()
		{
			return false;
		}

		public override bool CanPrimaryAttack()
		{
			return base.CanPrimaryAttack();
		}

		public override bool AttackPrimary()
		{
			var itemEnt = this.FindLockableItem();
			if ( itemEnt != null )
			{
				if ( !LockUnlockInteraction.IsLocked( itemEnt ) )
				{
					_unlockingItem = itemEnt;
					_unlocking = false;
					_unlockTime = Time.Now + UNLOCK_DELAY;

					this.RpcOnLock();
					this.OnLockUnlock();
					return true;
				}
				else
				{
					// TODO: Failure animation?
				}
			}

			return false;
		}

		public override bool CanSecondaryAttack()
		{
			return base.CanSecondaryAttack();
		}

		public override bool AttackSecondary()
		{
			var itemEnt = this.FindLockableItem();
			if ( itemEnt != null )
			{
				if ( LockUnlockInteraction.IsLocked( itemEnt ) )
				{
					_unlockingItem = itemEnt;
					_unlocking = true;
					_unlockTime = Time.Now + UNLOCK_DELAY;

					this.RpcOnUnlock();
					this.OnLockUnlock();
					return true;
				}
				else
				{
					// TODO: Failure animation?
				}
			}

			return false;
		}

		private void OnLockUnlock()
		{
			var test = this.Owner;
		}

		private VrpItemEntity FindLockableItem()
		{
			if ( this.Owner is VrpPlayer player )
			{
				var tr = player.GetEyeTrace();
				if ( tr.Entity is VrpItemEntity ent && ent.Item.HasInteraction( typeof( LockUnlockInteraction ) ) )
				{
					return (VrpItemEntity)tr.Entity;
				}
			}

			return null;
		}

		[ClientRpc]
		private void RpcOnLock()
		{
			Host.AssertClient();

			if ( IsLocalPawn )
			{
				_ = new Sandbox.ScreenShake.Perlin( 1f, 0.2f, 0.2f );
			}

			_lockSound = this.PlaySound( "keys_unlock" );
			// Yes the animation names are the wrong way round 
			ViewModelEntity?.SetAnimBool( "unlock", true );
		}

		[ClientRpc]
		private void RpcOnUnlock()
		{
			Host.AssertClient();

			if ( IsLocalPawn )
			{
				_ = new Sandbox.ScreenShake.Perlin( 1f, 0.2f, 0.2f );
			}

			_lockSound = this.PlaySound( "keys_unlock" );
			// Yes the animation names are the wrong way round 
			ViewModelEntity?.SetAnimBool( "lock", true );
		}

		[ClientRpc]
		private void RpcOnAbort()
		{
			Host.AssertClient();

			if ( IsLocalPawn )
			{
				_ = new Sandbox.ScreenShake.Perlin( 1f, 0.2f, 0.2f );
			}

			_lockSound.Stop();
			ViewModelEntity?.SetAnimBool( "abort", true );
		}
	}
}
