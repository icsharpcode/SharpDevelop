// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
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
			if (WorkbenchSingleton.Workbench == null) {
				return false;
			}
			
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (viewContent == null || viewContent.PrimaryFile == null) {
				return false;
			}
			
			if (!condition.Properties.Contains("activewindowuntitled"))
				return viewContent.PrimaryFile.IsUntitled;
			bool activewindowuntitled = Boolean.Parse(condition.Properties["activewindowuntitled"]);
			return viewContent.PrimaryFile.IsUntitled == activewindowuntitled;
		}
	}
}
