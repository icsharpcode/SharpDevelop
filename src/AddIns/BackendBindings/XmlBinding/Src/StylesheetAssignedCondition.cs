// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: -1 $</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Determines whether the active XML document has been assigned
	/// an XSLT stylesheet.
	/// </summary>
	public class StylesheetAssignedCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			XmlView properties = XmlView.ForView(WorkbenchSingleton.Workbench.ActiveViewContent);
			if (properties != null)
				return properties.StylesheetFileName != null;
			return false;
		}
	}
}
