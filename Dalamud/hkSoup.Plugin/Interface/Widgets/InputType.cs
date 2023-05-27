using ImGuiNET;

namespace HkSoup.Interface.Widgets; 

public static class InputType {
	public static bool Int16(string label, ref short val) {
		bool result;
		
		var iVal = (int)val;
		if (result = ImGui.InputInt(label, ref iVal))
			val = (short)iVal;
		
		return result;
	}

	public static bool Int16(string label, ref ushort val) {
		bool result;
		
		var sVal = (short)val;
		if (result = Int16(label, ref sVal))
			val = (ushort)sVal;
		
		return result;
	}
}