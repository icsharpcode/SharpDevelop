// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Tests the file extension of the file edited in the active window content.
	/// </summary>
	/// <attribute name="activeextension">
	/// The file extension the file should have.
	/// </attribute>
	/// <example title="Test if a C# file is being edited">
	/// &lt;Condition name = "ActiveContentExtension" activeextension=".cs"&gt;
	/// </example>
	public class ActiveContentExtensionConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null || WorkbenchSingleton.Workbench.ActiveViewContent == null) {
				return false;
			}
			try {
				string name = WorkbenchSingleton.Workbench.ActiveViewContent.PrimaryFileName;
				
				if (name == null) {
					return false;
				}
				
				string extension = Path.GetExtension(name);
				return extension.ToUpperInvariant() == condition.Properties["activeextension"].ToUpperInvariant();
			} catch (Exception) {
				return false;
			}
		}
	}
}
