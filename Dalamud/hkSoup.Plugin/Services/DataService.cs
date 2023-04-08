using System;
using System.Linq;
using System.Threading.Tasks;

using Dalamud.Logging;

using Lumina.Excel.GeneratedSheets;

using Xande;
using Xande.Havok;

// https://elysion.ktisis.tools/sklb_paths.txt

namespace HkSoup.Services; 

public static class DataService {
	public readonly static HavokConverter HkConverter = new();
	public readonly static SklbResolver SklbResolver = new();
	
	// Clans

	private static bool _getClans = true;
	private static string[] Clans = Array.Empty<string>();
	
	public static string[] GetClans() {
		if (_getClans) {
			_getClans = false;
			new Task(() => {
				var tribes = PluginServices.DataManager.GetExcelSheet<Tribe>()!;
				Clans = tribes.ToList().Select(row => row.Feminine.RawString).Where(name => name != "").ToArray();
				foreach (var c in Clans) PluginLog.Information(c);
			}).Start();
		}
		return Clans;
	}
}