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
	public class WindowOpenAuswerter : IAuswerter
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
