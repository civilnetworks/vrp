using Sandbox;
using Sandbox.UI;
using VChat;
using VRP.ui;
//using vrp.character.ui;

[Library]
public partial class Hud : HudEntity<RootPanel>
{
	public Hud()
	{
		if (!IsClient)
		{
			return;
		}
			

		RootPanel.StyleSheet.Load( "/ui/Hud.scss" );

		RootPanel.AddChild<NameTags>();
		RootPanel.AddChild<CrosshairCanvas>();
		RootPanel.AddChild<VrpChatBox>();
		RootPanel.AddChild<VoiceList>();
		RootPanel.AddChild<KillFeed>();
		RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
		RootPanel.AddChild<Health>();
		RootPanel.AddChild<InventoryBar>();
		RootPanel.AddChild<CurrentTool>();
		RootPanel.AddChild<SpawnMenu>();
		RootPanel.AddChild<CharacterSelectMenu>();
		RootPanel.AddChild<VrpHud>();
	}
}
