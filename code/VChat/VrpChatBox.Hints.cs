using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin;
using VAdmin.Commands;

namespace VChat
{
	public partial class VrpChatBox : Panel
	{
		private Panel _hintPanel;
		private string _prevCommandInput;
		private List<Label> _argHintLabels = new List<Label>();
		private static int _selectedHintIndex;

		public void ShowHints( bool visible )
		{
			if ( visible && _hintPanel != null )
			{
				return;
			}

			_hintPanel?.Delete();
			_hintPanel = null;

			if ( !visible )
			{
				return;
			}

			_hintPanel = Add.Panel( "hint_panel" );
		}

		public void AutoCompleteHint()
		{
			if ( Instance?.IsOpen ?? false )
			{
				var text = Instance.Input.Text;
				var showHint = VAdminSystem.EvaluateCommandEntry( text, out var evaluation, Local.Client );

				if ( showHint )
				{
					var autoCompleteText = "";

					if ( evaluation.Commands.Length > 0 && text.Length - 1 <= evaluation.Commands[0].Name.Length )
					{
						autoCompleteText = VAdminSystem.COMMAND_PREFIX + evaluation.Commands[0].Name;
					}

					if ( _argHintLabels.Count > 0 )
					{
						var lastArg = evaluation.Args.Last();
						var selectedHint = _argHintLabels[_selectedHintIndex].Text.Replace( "\"", "" );
						
						if ( !lastArg.Equals( selectedHint ) )
						{
							var argsImplode = text.Substring( 0, text.Length - evaluation.Args.Last().Length);//string.Join( ' ', evaluation.Args.Take( evaluation.Args.Length - 1 ) );
							if ( argsImplode .EndsWith('"'))
							{
								argsImplode = argsImplode.Substring( 0, argsImplode.Length - 1 );
							}

							autoCompleteText = $"{argsImplode}{_argHintLabels[_selectedHintIndex].Text}";
						}
					}

					if ( autoCompleteText.Length > 0 )
					{
						Input.SetText( autoCompleteText );
						Input.SetCaretPos( autoCompleteText.Length );
					}
				}
			}
		}

		public void NextArgHint()
		{
			if ( _hintPanel == null )
			{
				return;
			}

			_selectedHintIndex = Math.Min( _selectedHintIndex + 1, _argHintLabels.Count - 1 );

			this.ProcessArgHints();
		}

		public void PrevArgHint()
		{
			if ( _hintPanel == null )
			{
				return;
			}

			_selectedHintIndex = Math.Max( 0, _selectedHintIndex - 1 );

			this.ProcessArgHints();
		}

		private void UpdateHints( string input, CommandEvaluation evaluation )
		{
			this.ShowHints( false );
			this.ShowHints( true );

			var cmd = evaluation.Commands.FirstOrDefault();

			if ( evaluation.CurrentError != null && evaluation.CurrentError.Length > 0 && evaluation.Validation != ArgValidation.ExecutionError )
			{
				var err = _hintPanel.Add.Panel( "hint_desc_err" );
				var errContent = err.Add.Label( evaluation.CurrentError, "hint_desc_err_inner" );

				if ( evaluation.Validation == ArgValidation.Warning )
				{
					errContent.Style.FontColor = Color.Orange;
				}
			}

			foreach ( var command in evaluation.Commands )
			{
				var currentCommandPanel = _hintPanel.Add.Panel( "hint_current" );
				var currentTop = currentCommandPanel.Add.Panel( "hint_current_inner" );
				var currentBottom = currentCommandPanel.Add.Panel( "hint_current_inner" );

				var amtTyped = input.Length - 1;
				var currentName = currentTop.Add.Label( VAdminSystem.COMMAND_PREFIX + command.Name.Substring( 0, Math.Min( command.Name.Length, amtTyped ) ), "hint_emphasis" );

				if ( amtTyped < command.Name.Length )
				{
					var currentNameCompleted = currentTop.Add.Label( command.Name.Substring( amtTyped ), "hint" );
				}

				var desc = currentBottom.Add.Label( command.Description, "hint_desc" );

				if ( command != cmd || evaluation.ArgStatusList == null )
				{
					foreach ( var arg in command.Args )
					{
						var content = arg.Optional ? $" [(opt) {arg.Name}]" : $" [{arg.Name}]";
						var argLabel = currentTop.Add.Label( content, "hint" );
					}
				}
				else
				{
					foreach ( var status in evaluation.ArgStatusList )
					{
						var content = status.Arg.Optional ? $" [(opt) {status.Arg.Name}]" : $" [{status.Arg.Name}]";
						var argLabel = currentTop.Add.Label( content, status.InProgress ? "hint_emphasis" : "hint" );

						switch ( status.Validation )
						{
							case ArgValidation.Error:
								argLabel.Style.FontColor = Color.Red;
								break;
							case ArgValidation.Warning:
								argLabel.Style.FontColor = Color.Orange;
								break;
						}
					}
				}
			}

			_argHintLabels.Clear();

			if ( evaluation.Args.Length > 0 && evaluation.Args.Length <= (cmd?.Args.Length ?? 0) )
			{
				var arg = cmd.Args[evaluation.Args.Length - 1];
				var argSuggestions = VAdminSystem.GetArgSuggestions( arg, evaluation.Args.Last(), Local.Client );

				if ( argSuggestions.Length > 0 )
				{
					var argSuggestsPanel = _hintPanel.Add.Panel( "hint_arg_suggests" );

					foreach ( var suggest in argSuggestions )
					{
						var panel = argSuggestsPanel.Add.Panel( "item" );
						var text = panel.Add.Label( suggest, "content" );

						_argHintLabels.Add( text );
					}

					this.ProcessArgHints();
				}
			}
			else
			{
				_selectedHintIndex = 0;
			}
		}

		private void ProcessArgHints()
		{
			var index = 0;

			foreach ( var label in _argHintLabels )
			{
				label.SetClass( "selected", _selectedHintIndex == index );
				index++;
			}
		}

		[Event.Tick]
		private static void HintThink()
		{
			if ( Host.IsServer )
			{
				return;
			}

			if ( Instance?.IsOpen ?? false )
			{
				var text = Instance.Input.Text;
				var showHint = VAdminSystem.EvaluateCommandEntry( text, out var evaluation, Local.Client );

				if ( showHint )
				{
					Instance.ShowHints( true );

					if ( !text.Equals( Instance._prevCommandInput ) )
					{
						// Input has changed during valid command input, regenerate the hints UI elements.

						Instance._prevCommandInput = text;
						Instance.UpdateHints( text, evaluation );
					}
				}
				else
				{
					Instance.ShowHints( false );
					Instance._prevCommandInput = String.Empty;
					_selectedHintIndex = 0;
				}
			}
		}
	}
}
