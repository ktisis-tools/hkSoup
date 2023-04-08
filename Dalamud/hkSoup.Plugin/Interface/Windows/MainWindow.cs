using System.Numerics;

using ImGuiNET;

using Dalamud.Interface.Windowing;

using HkSoup.Services;

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
		SkeletonsTab();
	}

	private static void SkeletonsTab() {
		ImGui.Button("soup");
	}
}