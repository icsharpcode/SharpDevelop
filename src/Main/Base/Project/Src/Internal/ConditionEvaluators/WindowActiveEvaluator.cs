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
	/// <attribute name="activeWindow">
	/// The fully qualified name of the type the active window should be or the
	/// interface name it should implement.
	/// "*" to test if any window is active.
	/// </attribute>
	/// <example title="Test if the current window is a text editor">
	/// &lt;Condition name="WindowActive" activeWindow="ICSharpCode.SharpDevelop.Editor.ITextEditor"&gt;
	/// </example>
	/// <example title="Test if any window is active">
	/// &lt;Condition name="WindowActive" activeWindow="*"&gt;
	/// </example>
	public class WindowActiveConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (SD.Workbench == null) {
				return false;
			}
			
			string activeWindow = condition.Properties["activewindow"];
			if (activeWindow == "*") {
				return SD.Workbench.ActiveWorkbenchWindow != null;
			}
			
			Type activeWindowType = Type.GetType(activeWindow, false);
			if (activeWindowType == null) {
				//SD.Log.WarnFormatted("WindowActiveCondition: cannot find Type {0}", activeWindow);
				return false;
			}
			
			if (SD.GetActiveViewContentService(activeWindowType) != null)
				return true;
			
			if (SD.Workbench.ActiveWorkbenchWindow == null
			    || SD.Workbench.ActiveWorkbenchWindow.ActiveViewContent == null)
				return false;
			
			Type currentType = SD.Workbench.ActiveWorkbenchWindow.ActiveViewContent.GetType();
			if (currentType.FullName == activeWindow)
				return true;
			foreach (Type interf in currentType.GetInterfaces()) {
				if (interf.FullName == activeWindow)
					return true;
			}
			while ((currentType = currentType.BaseType) != null) {
				if (currentType.FullName == activeWindow)
					return true;
			}
			return false;
		}
	}
}
