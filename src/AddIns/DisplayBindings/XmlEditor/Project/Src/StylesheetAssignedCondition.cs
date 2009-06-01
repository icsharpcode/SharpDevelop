// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
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
		public bool IsValid(object owner, Condition condition)
		{
			XmlView xmlView = XmlView.ActiveXmlView;
			if (xmlView != null)
				return xmlView.StylesheetFileName != null;
			return false;
		}
	}
}
