using Dalamud.Data;
using Dalamud.IoC;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Plugin;
using Dalamud.Game.Command;

namespace HkSoup.Services;

internal class PluginServices {
	[PluginService] internal static Framework Framework { get; set; } = null!;
	[PluginService] internal static SigScanner SigScanner { get; set; } = null!;
	[PluginService] internal static DataManager DataManager { get; set; } = null!;
	[PluginService] internal static CommandManager CommandManager { get; set; } = null!;
	[PluginService] internal static DalamudPluginInterface Interface { get; set; } = null!;
	[PluginService] internal static ClientState ClientState { get; set; } = null!;

	public static void Init(DalamudPluginInterface dalamud)
		=> dalamud.Create<PluginServices>();
}