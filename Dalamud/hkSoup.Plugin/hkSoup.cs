using Dalamud.Plugin;

using HkSoup.Services;

namespace HkSoup;

// ReSharper disable once UnusedType.Global
public sealed class HkSoup : IDalamudPlugin {
	public string Name => "hkSoup";
	
	public HkSoup(DalamudPluginInterface dalamud) {
		PluginServices.Init(dalamud);
	}

	public void Dispose() {
	}
}