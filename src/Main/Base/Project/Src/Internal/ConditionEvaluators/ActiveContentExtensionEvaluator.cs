// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	public class ActiveContentExtensionConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent == null) {
				return false;
			}
			try {
				string name = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsUntitled ?
				              WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.UntitledName : WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName;
				
				if (name == null) {
					return false;
				}
				
				string extension = Path.GetExtension(name);
				return extension.ToUpper() == condition.Properties["activeextension"].ToUpper();
			} catch (Exception) {
				return false;
			}
		}
	}
}
