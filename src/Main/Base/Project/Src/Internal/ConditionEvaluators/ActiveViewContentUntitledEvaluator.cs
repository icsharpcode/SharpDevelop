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
	/// <summary>
	/// Tests if the active view content is untitled.
	/// </summary>
	/// <attributes name="activewindowuntitled">Boolean value to test against.</attributes>
	/// <example title="Test if the active view content is untitled">
	/// &lt;Condition name = "ActiveViewContentUntitled" activewindowuntitled="True"&gt;
	/// - or -
	/// &lt;Condition name = "ActiveViewContentUntitled"&gt;
	/// </example>
	/// <example title="Test if the active view content has a title">
	/// &lt;Condition name = "ActiveViewContentUntitled" activewindowuntitled="False"&gt;
	/// </example>
	public class ActiveViewContentUntitledConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent == null) {
				return false;
			}
			
			if (!condition.Properties.Contains("activewindowuntitled"))
				return WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsUntitled;
			bool activewindowuntitled = Boolean.Parse(condition.Properties["activewindowuntitled"]);
			return WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsUntitled == activewindowuntitled;
		}
	}
}
