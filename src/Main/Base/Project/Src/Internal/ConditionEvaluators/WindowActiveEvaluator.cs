// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Tests if the current workbench window is a specified type or implements an interface.
	/// </summary>
	/// <attribute name="activewindow">
	/// The fully qualified name of the type the active window should be or the
	/// interface name it should implement.
	/// "*" to test if any window is active.
	/// </attribute>
	/// <example title="Test if the current window is a text editor">
	/// &lt;Condition name="WindowActive" activewindow="ICSharpCode.SharpDevelop.Editor.ITextEditorProvider"&gt;
	/// </example>
	/// <example title="Test if any window is active">
	/// &lt;Condition name="WindowActive" activewindow="*"&gt;
	/// </example>
	public class WindowActiveConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null) {
				return false;
			}
			
			string activewindow = condition.Properties["activewindow"];
			
			if (activewindow == "*") {
				return WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null;
			}
			
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent == null) {
				return false;
			}
			
			Type currentType = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.GetType();
			if (currentType.FullName == activewindow)
				return true;
			foreach (Type interf in currentType.GetInterfaces()) {
				if (interf.FullName == activewindow)
					return true;
			}
			while ((currentType = currentType.BaseType) != null) {
				if (currentType.FullName == activewindow)
					return true;
			}
			return false;
		}
	}
}
