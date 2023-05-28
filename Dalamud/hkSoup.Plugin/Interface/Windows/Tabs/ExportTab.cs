using System;
using System.Linq;

using Dalamud.Logging;

using ImGuiNET;

using HkSoup.Enums;
using HkSoup.Interface.Components;
using HkSoup.Interface.Widgets;
using HkSoup.Services;

namespace HkSoup.Interface.Windows.Tabs; 

internal static class ExportTab {
	private static ObjectType ObjectType = ObjectType.Character;

	internal static void Draw() {
		EnumSelector.Draw("Object Type", ref ObjectType);

		switch (ObjectType) {
			case ObjectType.Character:
				DrawChara();
				break;
			default:
				ImGui.Text("(Not Implemented)");
				break;
		}
	}
	
	// Character

	private static CharaType CharaType = CharaType.Human;

	private static ushort BodyType = 0101;

	private static ushort SkeleType = BodyType;
	private static bool SkeleAuto = true;

	private static ushort FaceId = 0;
	private static ushort HairId = 0;
	private static ushort TailEarsId = 0;

	private static void DrawChara() {
		EnumSelector.Draw("Character Type", ref CharaType);

		ImGui.Spacing();
		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
		ImGui.Spacing();

		if (ImGui.CollapsingHeader("Body")) {
			ImGui.Spacing();
			
			DataService.BodyTypes.TryGetValue(BodyType, out var preview);
			if (ImGui.BeginCombo("Body Type", preview ?? "Custom")) {
				foreach (var body in DataService.BodyTypes) {
					if (ImGui.Selectable(body.Value))
						BodyType = body.Key;
				}
				ImGui.EndCombo();
			}

			var id = (int)BodyType;
			if (ImGui.InputInt("##BodyId", ref id))
				BodyType = (ushort)id;

			ImGui.Spacing();
		}

		ImGui.Spacing();
		
		// Slots

		if (ImGui.CollapsingHeader("Model Slots")) {
			ImGui.Spacing();
			
			var style = ImGui.GetStyle();

			var width = (ImGui.GetContentRegionAvail().X / 3) - (style.ItemSpacing.X / 1.5f);
			ImGui.PushItemWidth(width);

			var cursorY = ImGui.GetCursorPosY() - style.ItemSpacing.Y; // ???

			ImGui.BeginGroup();
			ImGui.Text("Face Id");
			InputType.Int16("##FaceId", ref FaceId);
			ImGui.EndGroup();

			ImGui.SameLine();
			ImGui.SetCursorPosY(cursorY); // ???

			ImGui.BeginGroup();
			ImGui.Text("Hair Id");
			InputType.Int16("##HairId", ref HairId);
			ImGui.EndGroup();

			ImGui.SameLine();
			ImGui.SetCursorPosY(cursorY);

			ImGui.BeginGroup();
			ImGui.Text("Tail Id");
			InputType.Int16("##TailId", ref TailEarsId);
			ImGui.EndGroup();

			ImGui.PopItemWidth();
			
			ImGui.Spacing();
		}

		ImGui.Spacing();

		// Skeleton
		
		if (ImGui.CollapsingHeader("Skeleton")) {
			ImGui.Spacing();
			
			ImGui.Checkbox("Auto resolve skeleton", ref SkeleAuto);

			ImGui.Spacing();

			if (SkeleAuto && SkeleType != BodyType)
				SkeleType = BodyType;
			
			ImGui.BeginDisabled(SkeleAuto);
			
			DataService.BodyTypes.TryGetValue(SkeleType, out var preview);
			if (ImGui.BeginCombo("Skeleton", preview ?? "Custom")) {
				foreach (var body in DataService.BodyTypes)
					if (ImGui.Selectable(body.Value))
						SkeleType = body.Key;
			}

			var id = (int)SkeleType;
			if (ImGui.InputInt("##SkeleId", ref id))
				SkeleType = (ushort)id;
			
			ImGui.EndDisabled();
			
			ImGui.Spacing();
		}

		ImGui.Spacing();

		// Buttons

		ImGui.Spacing();
		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
		ImGui.Spacing();

		/*if (ImGui.Button("Export .glTF")) {
			var models = new[] {
				DataService.MdlResolver.Resolve(BodyType, ModelSlot.Top, 1)!,
				DataService.MdlResolver.Resolve(BodyType, ModelSlot.Gloves, 0)!,
				DataService.MdlResolver.Resolve(BodyType, ModelSlot.Legs, 1)!,
				DataService.MdlResolver.Resolve(BodyType, ModelSlot.Shoes, 0)!,
				DataService.MdlResolver.Resolve(BodyType, ModelSlot.Face, FaceId)!,
				DataService.MdlResolver.Resolve(BodyType, ModelSlot.Hair, HairId)!,
				DataService.MdlResolver.Resolve(BodyType, ModelSlot.TailEars, TailEarsId)!
			};

			var skele = new[] {
				DataService.SklbResolver.GetHumanBasePath(BodyType)!
			}.Concat(DataService.SklbResolver.ResolveAll(models)).ToArray();

			DataService.Export(models, skele, BodyType, true);
		}*/
		
		ImGui.SameLine();
		ImGui.Button("Export .XML"); // TODO
		ImGui.SameLine();
	}
}