// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;


using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	public class WindowActiveAuswerter : IAuswerter
	{
		Type prevType      = null;
		bool prevValidFlag = false;
		
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
			
			
			if (currentType.Equals(prevType)) {
				return prevValidFlag;
			} else {
				prevType = currentType;
				if (currentType.ToString() == activewindow) {
					prevValidFlag = true;
					return true;
				}
				foreach (Type i in currentType.GetInterfaces()) {
					if (i.ToString() == activewindow) {
						prevValidFlag = true;
						return true;
					}
				}
			}
			prevValidFlag = false;
			return false;			
		}
	}
}
