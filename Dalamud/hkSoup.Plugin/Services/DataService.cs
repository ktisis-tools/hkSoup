extern alias LuminaX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using LuminaX::Lumina.Data;

using Lumina.Excel.GeneratedSheets;

using Xande;
using Xande.Files;
using Xande.Havok;

// https://elysion.ktisis.tools/sklb_paths.txt

namespace HkSoup.Services;

internal static class DataService {
    internal readonly static LuminaManager Lumina = new();
    internal readonly static HavokConverter HkConverter;
    internal readonly static SklbResolver SklbResolver;
    internal readonly static MdlResolver MdlResolver;
    internal readonly static ModelConverter ModelConverter;

    // Init

    static DataService() {
        Lumina = new LuminaManager();
        HkConverter = new();
        SklbResolver = new();
        MdlResolver = new();
        ModelConverter = new(Lumina);
    }

    internal static void Init()
        => new Task(InitTask).Start();

    private static void InitTask() {
        GetSheetData();
    }

    // Exports

    /* TODO: Allow model exports to run as an async Task to reduce hitching.
     * Xande is currently required to run on the framework thread in order to read Havok data.
     * However, the heavier operations that put the model together also run on this thread, hanging the game until completed.
    */

    /*internal static void Export(string[] models, string[] skeletons)
        => new Task(() => ExportTask(models, skeletons)).Start();*/

    private static string GetTempDir(string prefix) {
        var tempDir = Path.Combine(Path.GetTempPath(), "hkSoup");
        Directory.CreateDirectory(tempDir);

        var tempPath = Path.Combine(tempDir, $"{prefix}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}");
        Directory.CreateDirectory(tempPath);

        return tempPath;
    }

    internal static void Export(string[] models, string[] skeletonPaths, ushort? deform = null, bool openPath = false) {
        // Temporarily using NotNite's code for this -
        // https://github.com/xivdev/Xande/blob/8fd69851b8f1069e4eecdbfd99ad5d8a81ef2816/Xande.TestPlugin/Windows/MainWindow.cs#L49
        var tempPath = GetTempDir("export");
        PluginServices.Framework.RunOnTick(() => {
            var skeletons = skeletonPaths.Select(path => {
                var file = Lumina.GetFile<FileResource>(path)!;
                var sklb = SklbFile.FromStream(file.Reader.BaseStream);
                var xml = HkConverter.HkxToXml(sklb.HkxData);
                return new HavokXml(xml);
            }).ToArray();
            ModelConverter.ExportModel(tempPath, models, skeletons, deform);
            if (openPath) Process.Start("explorer.exe", tempPath);
        });
    }

    internal static void Export(string[] models, string baseSkel, ushort? deform = null, bool openPath = false) {
        var skeletons = SklbResolver.ResolveAll(models)
            .Prepend(baseSkel)
            .ToArray();

        Export(models, skeletons, deform, openPath);
    }

    internal static void ExportSkeleton(string skel) {
        var file = Lumina.GetFile<FileResource>(skel)!;
        var sklb = SklbFile.FromStream(file.Reader.BaseStream);
        var xmlStr = HkConverter.HkxToXml(sklb.HkxData);
        var xml = new HavokXml(xmlStr);

        var tempPath = GetTempDir("skeleton-export");

        var filename = Path.GetFileNameWithoutExtension(skel);
        var xmlPath = Path.Combine(tempPath, $"{filename}.xml");
        File.WriteAllText(xmlPath, xmlStr);

        Process.Start("explorer.exe", tempPath);
    }

    internal static void ImportSkeleton(string xmlPath, string origPath) {
        var sklbFile = Lumina.GetFile<FileResource>(origPath);
        using var ms = new MemoryStream(sklbFile.Data);
        var sklb = SklbFile.FromStream(ms);

        var xml = File.ReadAllText(xmlPath);
        var hkx = HkConverter.XmlToHkx(xml);

        sklb.ReplaceHkxData(hkx);

        var tempPath = GetTempDir("skeleton-import");
        var filename = Path.GetFileNameWithoutExtension(origPath);
        var newPath = Path.Combine(tempPath, $"{filename}.sklb");

        using var ms2 = new MemoryStream();
        sklb.Write(ms2);
        File.WriteAllBytes(newPath, ms2.ToArray());
        
        Process.Start("explorer.exe", tempPath);
    }

    // Sheets

    internal readonly static Dictionary<ushort, string> BodyTypes = new();

    private static void GetSheetData() {
        var data = PluginServices.DataManager;

        var races = data.GetExcelSheet<Race>()!;
        var clans = data.GetExcelSheet<Tribe>()!;
        foreach (var race in races) {
            var id = race.RowId;
            if (id == 0) continue;

            var rTribes = new List<Tribe>();
            for (uint c = 0; c < 2; c++)
                rTribes.Add(clans.GetRow(id * 2 - 1 + c)!);

            for (byte g = 0; g < 2; g++) {
                var adj = g == 0 ? "Male" : "Female";
                var rName = g == 0 ? race.Masculine : race.Feminine;
                if (id == 1) {
                    foreach (var c in rTribes) {
                        var cName = g == 0 ? c.Masculine : c.Feminine;
                        var bType = SklbResolver.GetHumanId((byte)c.RowId, g);
                        BodyTypes.Add(bType, $"{adj} {cName}");
                    }
                } else {
                    var bType = SklbResolver.GetHumanId((byte)rTribes[0].RowId, g);
                    BodyTypes.Add(bType, $"{adj} {rName}");
                }
            }
        }
    }
}
