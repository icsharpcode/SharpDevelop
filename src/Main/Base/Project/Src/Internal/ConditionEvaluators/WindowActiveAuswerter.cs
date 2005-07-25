// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;


using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	public class WindowActiveAuswerter : IAuswerter
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
			Type activeType = Type.GetType(activewindow);
			if (activeType == null) {
				return false;
			}
				
			return activeType.IsAssignableFrom(currentType);
		}
	}
}
