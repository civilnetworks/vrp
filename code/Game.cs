using Sandbox;
using VAdmin;
using VRP.Character;
using VVehicle;

[Library( "sandbox", Title = "Sandbox" )]
partial class SandboxGame : Game
{
	private Hud _hud;
	private CharacterManager _characterManager;

	public SandboxGame()
	{
		if ( IsServer )
		{
			// Create the HUD
			_hud = new Hud();

			this.InitializeVrpManagers();
		}
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );

		if ( IsServer )
		{
			// TODO: Make this an event
			_characterManager.InitializeClient( cl );
			VAdminSystem.NetworkRoles( To.Single( cl ) );
			VAdminSystem.NetworkOnlineUserInfo( To.Single( cl ) );
			VVehicleSystem.NetworkAllVehicleData( To.Single( cl ) );
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	[ServerCmd( "spawn" )]
	public static void Spawn( string modelname )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null )
			return;

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();

		var ent = new Prop();
		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
		ent.SetModel( modelname );

		// Drop to floor
		if ( ent.PhysicsBody != null && ent.PhysicsGroup.BodyCount == 1 )
		{
			var p = ent.PhysicsBody.FindClosestPoint( tr.EndPos );

			var delta = p - tr.EndPos;
			ent.PhysicsBody.Position -= delta;
			//DebugOverlay.Line( p, tr.EndPos, 10, false );
		}

	}

	[ServerCmd( "spawn_entity" )]
	public static void SpawnEntity( string entName )
	{
		var owner = ConsoleSystem.Caller.Pawn;

		if ( owner == null )
			return;

		var attribute = Library.GetAttribute( entName );

		if ( attribute == null || !attribute.Spawnable )
			return;

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 200 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();

		var ent = Library.Create<Entity>( entName );
		if ( ent is BaseCarriable && owner.Inventory != null )
		{
			if ( owner.Inventory.Add( ent, true ) )
				return;
		}

		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) );

		//Log.Info( $"ent: {ent}" );
	}

	public override void DoPlayerNoclip( Client player )
	{
		if ( !VAdminSystem.TestPermission( player, "vrp_noclip" ) )
		{
			return;
		}

		if ( player.Pawn is Player basePlayer )
		{
			if ( basePlayer.DevController is NoclipController )
			{
				Log.Info( "Noclip Mode Off" );
				basePlayer.DevController = null;
			}
			else
			{
				Log.Info( "Noclip Mode On" );
				basePlayer.DevController = new NoclipController();
			}
		}
	}

	public void InitializeVrpManagers()
	{
		_characterManager?.Delete();
		_characterManager = new CharacterManager();
	}

	[Event.Hotload]
	public void OnReloaded()
	{
		if ( IsServer )
		{
			_hud?.Delete();
			_hud = new Hud();
		}
	}
}
