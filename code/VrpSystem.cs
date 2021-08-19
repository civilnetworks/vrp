using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VChat;
using VRP.Character;
using VRP.Player;

namespace VRP
{
	public static partial class VrpSystem
	{
		private static bool _doneTestt;

		[Event.Frame]
		public static void TestFrame()
		{
			if ( _doneTestt )
			{
				return;
			}

			_doneTestt = true;

			var cl = Client.All[0];
			var ply = cl.Pawn as VrpPlayer;

			if (ply == null)
			{
				return;
			}

			//ply.ResetBones();

			CharacterManager.Instance.ScaleBonePositions( ply, "hand_R", "finger_ring_2_R", 0.5f );
			CharacterManager.Instance.ScaleBonePositions( ply, "hand_R", "finger_middle_2_R", 0.5f );
			CharacterManager.Instance.ScaleBonePositions( ply, "hand_R", "finger_index_2_R", 0.5f );
			CharacterManager.Instance.ScaleBonePositions( ply, "hand_R", "finger_thumb_2_R", 0.5f );
		}

		public static void SendChatMessage( string message )
		{
			SendChatMessage( To.Everyone, message );
		}

		public static void SendChatMessage( To to, string message )
		{
			VChatSystem.AddInformation( to, $"[VRP] {message}" );
		}

		public static TraceResult GetEyeTrace( Client client, float distance = 10000f )
		{
			return GetEyeTrace( client.Pawn, distance );
		}

		public static TraceResult GetEyeTrace( Entity player, float distance = 10000f )
		{
			var trace = Trace.Ray( player.EyePos, player.EyePos + player.EyeRot.Forward * distance )
				.UseHitboxes()
				.WorldAndEntities()
				.Ignore( player );

			return trace.Run();
		}

		public static string GetRPName( Client client )
		{
			var character = CharacterManager.Instance.GetActiveCharacter( client );
			if ( character != null )
			{
				return character.Name;
			}

			return client.Name;
		}

		public static void PlaceItemOnGround( Vector3 pos, ModelEntity ent, float distance = 200f )
		{
			var trace = Trace.Ray( pos + Vector3.Up, pos - Vector3.Up * distance )
				.WorldAndEntities()
				.Ignore( ent, true )
				.UseHitboxes()
				.HitLayer( CollisionLayer.All ^ CollisionLayer.Debris );

			PlaceItemOnGroundTrace( trace.Run(), ent );
		}

		public static void PlaceItemOnGroundTrace( TraceResult tr, ModelEntity ent )
		{
			ent.Position = tr.EndPos + Vector3.Up * ent.CollisionBounds.Size.z;
		}
	}

	public class Location
	{
		public Location() {
			this.SetPos( Vector3.Zero );
			this.SetAngles( Angles.Zero );
		}

		public Location( Vector3 pos )
		{
			this.SetPos( pos );
			this.SetAngles( Angles.Zero );
		}

		public Location( Vector3 pos, Angles ang )
		{
			this.SetPos( pos );
			this.SetAngles( ang );
		}

		public float PosX { get; set; }

		public float PosY { get; set; }

		public float PosZ { get; set; }

		public float Pitch { get; set; }

		public float Yaw { get; set; }

		public float Roll { get; set; }

		public Vector3 GetPos()
		{
			return new Vector3( this.PosX, this.PosY, this.PosZ );
		}

		public void SetPos( Vector3 pos )
		{
			this.PosX = pos.x;
			this.PosY = pos.y;
			this.PosZ = pos.z;
		}

		public Angles GetAngles()
		{
			return new Angles( this.Pitch, this.Yaw, this.Roll );
		}

		public void SetAngles( Angles ang )
		{
			this.Pitch = ang.pitch;
			this.Yaw = ang.yaw;
			this.Roll = ang.roll;
		}
	}
}
