extern alias LuminaX;
using System.IO;

using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.ImGuiFileDialog;

using HkSoup.Services;

using ImGuiNET;

using LuminaX::Lumina.Data;

using Xande.Files;

namespace HkSoup.Interface.Windows.Tabs;

internal static class ImportTab {
    private static string ImportPath = string.Empty;
    private static string OriginalPath = string.Empty;
    private static FileDialogManager FileDialogManager = new();

    internal static void Draw() {
        FileDialogManager.Draw();

        ImGui.InputText("Import Path", ref ImportPath, 1024);
        ImGui.SameLine();
        if (ImGuiComponents.IconButton(FontAwesomeIcon.Folder)) {
            FileDialogManager.OpenFileDialog("Select .xml file", "Havok XML File{.xml}", (ok, result) => {
                if (!ok) return;
                ImportPath = result;
            });
        }

        ImGui.InputText("Original Path", ref OriginalPath, 1024);

        ImGui.Spacing();

        if (ImGui.Button("Import")) {
            DataService.ImportSkeleton(ImportPath, OriginalPath);
        }
    }
}
