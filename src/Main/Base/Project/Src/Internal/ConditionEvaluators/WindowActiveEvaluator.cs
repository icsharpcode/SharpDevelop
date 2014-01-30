// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
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
			string activeWindow = condition.Properties["activewindow"];
			if (activeWindow == "*") {
				return SD.Workbench.ActiveWorkbenchWindow != null;
			}
			
			Type activeWindowType = condition.AddIn.FindType(activeWindow);
			if (activeWindowType == null) {
				SD.Log.WarnFormatted("WindowActiveCondition: cannot find Type {0}", activeWindow);
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
