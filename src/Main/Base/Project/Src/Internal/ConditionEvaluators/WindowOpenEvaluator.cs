// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Tests if a window of a specified type or implementing an interface is open.
	/// The window does not need to be the active window.
	/// </summary>
	/// <attribute name="openwindow">
	/// The fully qualified name of the type the window should be or the
	/// interface name it should implement.
	/// "*" to test if any window is open.
	/// </attribute>
	/// <example title="Test if a text editor is opened">
	/// &lt;Condition name="WindowOpen" openwindow="ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ITextEditorControlProvider"&gt;
	/// </example>
	/// <example title="Test if any window is open">
	/// &lt;Condition name="WindowOpen" openwindow="*"&gt;
	/// </example>
	public class WindowOpenConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null) {
				return false;
			}
			
			string openwindow = condition.Properties["openwindow"];
			
			if (openwindow == "*") {
				return WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null;
			}
			
			foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
				Type currentType = view.GetType();
				if (currentType.ToString() == openwindow) {
					return true;
				}
				foreach (Type i in currentType.GetInterfaces()) {
					if (i.ToString() == openwindow) {
						return true;
					}
				}
			}
			return false;
		}
	}
}
