using System.Numerics;

using Dalamud.Interface;

using ImGuiNET;

namespace HkSoup.Interface.Widgets; 

internal static class Buttons {
	internal static bool IconButton(FontAwesomeIcon icon, Vector2 size = default, string id = "") {
		ImGui.PushFont(UiBuilder.IconFont);
		var clicked = ImGui.Button($"{icon.ToIconString()}##{id}", size);
		ImGui.PopFont();
		return clicked;
	}

	internal static bool IconButtonTooltip(FontAwesomeIcon icon, string tooltip, Vector2 size = default, string id = "") {
		var clicked = IconButton(icon, size, id);
		Tooltip(tooltip);
		return clicked;
	}
	
	private static void Tooltip(string text) {
		if (!ImGui.IsItemHovered()) return;

		ImGui.BeginTooltip();
		ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
		ImGui.TextUnformatted(text);
		ImGui.PopTextWrapPos();
		ImGui.EndTooltip();
	}
}