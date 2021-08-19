using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using vrp.Items;
using VRP.Items.Interacts;

namespace VRP.Items
{
	public delegate void VrpItemDataEvent( string key, object value );

	/// <summary>
	/// Base class for VRP Items.
	/// </summary>
	public abstract class VrpItem : NetworkComponent
	{
		public VrpItemDataEvent OnDataSet;

		private Dictionary<string, VrpItemInteraction> _interactions = new Dictionary<string, VrpItemInteraction>();

		public VrpItem()
		{
			if ( this.Carryable )
			{
				// TODO: Replace with physical carrying hands
				//this.AddInteraction( new PickupInteraction() );
			}

			this.AddInteractions();
		}

		public abstract string Id { get; }

		public abstract string Name { get; }

		public abstract bool Carryable { get; }

		public virtual bool DisplayHint { get; } = true;

		public virtual float DisplayHintDistance { get; } = 100f;

		public virtual VrpItemHintItem[] HintItems { get; } = new VrpItemHintItem[] { };

		public abstract ItemDataTemplate[] DataTemplates { get; }

		public virtual Func<string> NameFunc { get; }

		public abstract string ModelPath { get; }

		public virtual Func<string> ModelPathFunc { get; }

		/// <summary>
		/// Item data dictionary.
		/// NOTE: Must contain only Json serializable objects, since this must be saved and networked.
		/// </summary>
		public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

		public VrpItemInteraction[] GetInteractions()
		{
			return _interactions.Select( p => p.Value ).ToArray();
		}

		public VrpItemInteraction GetInteraction( string key )
		{
			if ( !this.HasInteraction( key ) )
			{
				return null;
			}

			return _interactions[key];
		}

		public VrpItemInteraction GetInteraction( Type type )
		{
			return _interactions.Select( p => p.Value ).FirstOrDefault( i => i.GetType() == type );
		}

		public string GetModelPath()
		{
			var funcVal = this.ModelPathFunc?.Invoke() ?? null;
			if ( funcVal != null )
			{
				return funcVal;
			}

			return this.ModelPath;
		}

		public string GetDisplayName()
		{
			var funcVal = this.NameFunc?.Invoke() ?? null;
			if ( funcVal != null )
			{
				return funcVal;
			}

			return this.Name;
		}

		public void SetData( string key, object value )
		{
			if ( this.Data.ContainsKey( key ) )
			{
				this.Data.Remove( key );
			}

			if ( value == null )
			{
				OnDataSet?.Invoke( key, value );
				return;
			}

			this.Data[key] = value;

			OnDataSet?.Invoke( key, value );
		}

		public object GetData( string key, object defaultValue = null )
		{
			if ( this.Data.TryGetValue( key, out var value ) )
			{
				return value;
			}

			return defaultValue;
		}

		public bool GetBool( string key, bool defaultValue = false )
		{
			var data = this.GetData( key );

			if ( bool.TryParse( data?.ToString(), out var value ) )
			{
				return value;
			}

			return defaultValue;
		}

		public double GetDouble( string key, double defaultValue = 0 )
		{
			var data = this.GetData( key );

			if ( double.TryParse( data?.ToString(), out var value ) )
			{
				return value;
			}

			return defaultValue;
		}

		public ulong GetUnsignedLong( string key, ulong defaultValue = 0 )
		{
			var data = this.GetData( key );

			if ( ulong.TryParse( data?.ToString(), out var value ) )
			{
				return value;
			}

			return defaultValue;
		}

		public bool HasData( string key )
		{
			return this.Data.ContainsKey( key );
		}

		public bool Interact( Client client, VrpItemEntity ent, Type interactionClassType, out string error )
		{
			return Interact( client, ent, null, interactionClassType, out error );
		}

		public bool Interact( Client client, VrpItemEntity ent, string[] args, Type interactionClassType, out string error )
		{
			if ( !this.HasInteraction( interactionClassType ) )
			{
				error = "Invalid interaction type";
				return false;
			}

			return Interact( client, ent, args, this.GetInteraction( interactionClassType ).Key, out error );
		}

		public bool Interact( Client client, VrpItemEntity ent, string[] args, string key, out string error )
		{
			if ( !this.HasInteraction( key ) )
			{
				error = "Invalid interaction";
				return false;
			}

			var interaction = _interactions[key];

			if ( !VrpItems.ValidateInteractionArgs( interaction, args, out error ) )
			{
				return false;
			}

			// Check custom interact func
			if ( interaction.CanInteractFunc != null )
			{
				var canResult = interaction.CanInteractFunc( client, ent );
				if ( !canResult.Success )
				{
					error = canResult.DisplayError;
					return false;
				}
			}

			// Check claim requirement
			if ( interaction.RequiresClaim && ClaimUnclaimInteraction.IsClaimable( ent ) )
			{
				var owner = ClaimUnclaimInteraction.GetOwnerSteamId( ent );

				if ( owner != client.SteamId )
				{
					error = "You don't own this.";
					return false;
				}
			}

			var argsList = new List<string>();
			for ( var i = 0; i < (interaction?.Args.Length ?? 0); i++ )
			{
				if ( args != null && args.Length > i )
				{
					argsList.Add( args[i] );
				}
				else
				{
					argsList.Add( null );
				}
			}

			var result = interaction.Interact( client, ent, argsList.ToArray() );

			error = result.DisplayError;
			return result.Success;
		}

		public void AddInteraction( VrpItemInteraction interaction )
		{
			_interactions[interaction.Key] = interaction;
		}

		public bool HasInteraction( string key )
		{
			return _interactions.ContainsKey( key );
		}

		public bool HasInteraction( Type type )
		{
			return this.GetInteraction( type ) != null;
		}

		protected virtual void AddInteractions()
		{

		}

		public virtual void Initialize( VrpItemEntity ent )
		{

		}

		public virtual void Tick( VrpItemEntity ent )
		{

		}

		public virtual void OnPhysicsCollision( VrpItemEntity ent, CollisionEventData eventData )
		{

		}


		public string SerializeData()
		{
			return JsonSerializer.Serialize( this.Data );
		}

		public Dictionary<string, object> DeserializeData( string json )
		{
			return JsonSerializer.Deserialize<Dictionary<string, object>>( json );
		}
	}
}
