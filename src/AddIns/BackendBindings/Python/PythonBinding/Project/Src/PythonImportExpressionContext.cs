// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonImportExpressionContext : ExpressionContext
	{
		public bool HasFromAndImport { get; set; }
		
		public override bool ShowEntry(object o)
		{
			if (HasFromAndImport) {
				return ShowEntryForImportIdentifier(o);
			}
			return ShowEntryForImportModule(o);
		}
		
		bool ShowEntryForImportModule(object o)
		{
			return o is String;
		}
		
		bool ShowEntryForImportIdentifier(object o)
		{
			if (o is IMethod) {
				return true;
			} else if (o is IField) {
				return true;
			} else if (o is IClass) {
				return true;
			}
			return false;
		}
	}
}
