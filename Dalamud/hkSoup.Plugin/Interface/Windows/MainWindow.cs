using System;
using System.Numerics;

using ImGuiNET;

using Dalamud.Interface.Windowing;

using HkSoup.Interface.Windows.Tabs;

namespace HkSoup.Interface.Windows; 

public class MainWindow : Window {
	// woo!! yeah!! woo!
	
	public MainWindow() : base("hkSoup Toolkit") {
		SizeConstraints = new WindowSizeConstraints {
			MinimumSize = new Vector2(200, 200),
			MaximumSize = ImGui.GetIO().DisplaySize
		};
	}

	public override void Draw() {
		if (ImGui.BeginTabBar("hkSoup Editor")) {
			DrawTab("Imports", () => {});
			DrawTab("Exports", ExportTab.Draw);
		}
	}

	private void DrawTab(string label, Action callback) {
		if (ImGui.BeginTabItem(label)) {
			ImGui.Spacing();
			callback.Invoke();
			ImGui.EndTabItem();
		}
	}
}