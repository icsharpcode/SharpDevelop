// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
