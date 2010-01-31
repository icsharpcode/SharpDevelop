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
		
		public override bool ShowEntry(ICompletionEntry entry)
		{
			if (HasFromAndImport) {
				return ShowEntryForImportIdentifier(entry);
			}
			return ShowEntryForImportModule(entry);
		}
		
		bool ShowEntryForImportModule(ICompletionEntry entry)
		{
			return entry is NamespaceEntry;
		}
		
		bool ShowEntryForImportIdentifier(ICompletionEntry entry)
		{
			if (entry is IMethod) {
				return true;
			} else if (entry is IField) {
				return true;
			} else if (entry is IClass) {
				return true;
			}
			return false;
		}
	}
}
