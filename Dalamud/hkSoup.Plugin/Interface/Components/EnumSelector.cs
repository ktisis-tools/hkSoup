using System;

using ImGuiNET;

namespace HkSoup.Interface.Components; 

public static class EnumSelector {
	public static bool Draw<T>(string label, ref T value, string? preview = null) where T : Enum {
		bool result = false;
		
		if (ImGui.BeginCombo(label, preview ?? $"{value}")) {
			foreach (T item in Enum.GetValues(typeof(T))) {
				if (ImGui.Selectable($"{item}")) {
					result = true;
					value = item;
				}
			}
			ImGui.EndCombo();
		}
		
		return result;
	}
}