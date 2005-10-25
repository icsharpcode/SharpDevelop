// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System;

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
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window != null) {
				XmlView view = window.ActiveViewContent as XmlView;
				if (view != null) {
					return view.StylesheetFileName != null;
				}
			}
			return false;
		}
	}
}
