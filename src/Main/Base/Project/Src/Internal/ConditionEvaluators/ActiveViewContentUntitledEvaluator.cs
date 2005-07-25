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
	public class ActiveViewContentUntitledAuswerter : IAuswerter
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent == null) {
				return false;
			}
			
			bool activewindowuntitled = Boolean.Parse(condition.Properties["activewindowuntitled"]);
			return WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsUntitled && activewindowuntitled ||
			       !WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsUntitled && !activewindowuntitled ;
		}
	}
}
