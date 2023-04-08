using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Dalamud.Interface.Windowing;

namespace HkSoup.Interface; 

public static class Gui {
	public static void Draw() => Windows.Draw();

	// Windows
	
	private readonly static WindowSystem Windows = new("hkSoup");
	
	private readonly static FieldInfo WindowsField = typeof(WindowSystem).GetField("windows", BindingFlags.Instance | BindingFlags.NonPublic)!;
	private static IEnumerable<Window> WindowsList => (List<Window>?)WindowsField.GetValue(Windows)!;

	public static Window GetWindow<T>(object[]? args = null) where T : Window {
		var exists = WindowsList.OfType<T>().FirstOrDefault();
		if (exists != null) return exists;

		var window = (Window)Activator.CreateInstance(typeof(T), args)!;
		Windows.AddWindow(window);
		return window;
	}

	public static void RemoveWindow<T>() where T : Window {
		foreach (var w in WindowsList.OfType<T>())
			Windows.RemoveWindow(w);
	}
}