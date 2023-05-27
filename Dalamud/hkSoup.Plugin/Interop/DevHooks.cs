using System.Runtime.InteropServices;

using Dalamud.Hooking;
using Dalamud.Logging;

using FFXIVClientStructs.Havok;

using HkSoup.Services;

namespace HkSoup.Interop; 

public static class DevHooks {
	internal delegate nint ResolveMdlDelegate(nint a1, nint a2, nint a3, uint a4);
	internal static Hook<ResolveMdlDelegate> ResolveMdlHook = null!;
	private unsafe static nint ResolveMdlDetour(nint a1, nint a2, nint a3, uint a4) {
		//if (a4 == 10) return 0;
		//if (a4 > 12) return 0;

		//*(nint*)(a1 + 2616) = 0;

		//*(ushort*)(a1 + 0x93c) = 204;
		
		var exec = ResolveMdlHook.Original(a1, a2, a3, a4);
		//PluginLog.Information($"{a1:X} {a2:X} {a3:X} {a4} => {exec:X}");
		if (exec != 0) PluginLog.Information($"{Marshal.PtrToStringUTF8(exec)}");
		else PluginLog.Information($"<no data>");
		
			/*if (a4 > 9) {
			if (a4 == 10) {
				PluginLog.Information($"{*(ushort*)(a1 + 0x93c)}");
			}
			
			PluginLog.Information($"{a1:X}, {a2:X}, {a3}, {a4} -> {exec:X}");
			if (exec != 0) {
				var mdl = Marshal.PtrToStringUTF8(exec);
				PluginLog.Information($"{mdl}");
			}
		}*/

		return exec;
	}
	
	// 48 8B 0D ?? ?? ?? ?? 48 8D 55 80 44 8B C3
	internal static nint CharaUtil_Instance = 0;

	internal delegate nint EqpDataDelegate(nint a1, ushort a2, uint a3, ushort a4);
	//internal static EqpDataDelegate GetEqpDelegate = null!;
	internal static Hook<EqpDataDelegate> EqpDataHook = null!;
	internal static nint EqpDataDetour(nint a1, ushort a2, uint a3, ushort a4) {
		var exec = EqpDataHook.Original(a1, a2, a3, a4);
		//PluginLog.Information($"{a1:X} {a2} {a3} {a4} => {exec}");
		
		//PluginLog.Information($"{GetEqpDelegate(a1, a2, a3, a4)}");
		
		return exec;
	}

	public static void Init() {
		var util = PluginServices.SigScanner.GetStaticAddressFromSig("48 8B 0D ?? ?? ?? ?? 48 8D 55 80 44 8B C3");
		CharaUtil_Instance = util;
		
		var addr = PluginServices.SigScanner.ScanText("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC 40 45 8B D1");
		ResolveMdlHook = Hook<ResolveMdlDelegate>.FromAddress(addr, ResolveMdlDetour);
		ResolveMdlHook.Enable();

		var addr2 = PluginServices.SigScanner.ScanText("E8 ?? ?? ?? ?? 66 3B 85 ?? ?? ?? ??");
		//GetEqpDelegate = Marshal.GetDelegateForFunctionPointer<EqpDataDelegate>(addr2);
		EqpDataHook = Hook<EqpDataDelegate>.FromAddress(addr2, EqpDataDetour);
		EqpDataHook.Enable();
	}

	public static void Dispose() {
		ResolveMdlHook.Disable();
		ResolveMdlHook.Dispose();

		EqpDataHook.Disable();
		EqpDataHook.Dispose();
	}
}