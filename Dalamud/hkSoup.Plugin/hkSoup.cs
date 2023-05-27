using Dalamud.Game.Command;
using Dalamud.Plugin;

using HkSoup.Interface;
using HkSoup.Interface.Windows;
using HkSoup.Interop;
using HkSoup.Services;

namespace HkSoup;

// ReSharper disable once UnusedType.Global
public sealed class HkSoup : IDalamudPlugin {
	// Plugin info
	
	public string Name => "hkSoup";
	private const string CommandName = "/hksoup";

	// Init & Dispose
	
	public HkSoup(DalamudPluginInterface dalamud) {
		PluginServices.Init(dalamud);

		DataService.Init();

		PluginServices.Interface.UiBuilder.DisableGposeUiHide = true;
		PluginServices.Interface.UiBuilder.Draw += Gui.Draw;

		PluginServices.Interface.UiBuilder.OpenConfigUi += ToggleMainWindow;
		
		PluginServices.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
			HelpMessage = "Opens the main hkSoup window."
		});

		DevHooks.Init();
		
		#if DEBUG
		ToggleMainWindow();
		#endif
	}

	public void Dispose() {
		DevHooks.Dispose();
		
		PluginServices.Interface.UiBuilder.Draw -= Gui.Draw;
		
		PluginServices.CommandManager.RemoveHandler(CommandName);
	}
	
	// Interface

	private void ToggleMainWindow()
		=> Gui.GetWindow<MainWindow>().Toggle();

	private void OnCommand(string cmd, string args)
		=> ToggleMainWindow();
}