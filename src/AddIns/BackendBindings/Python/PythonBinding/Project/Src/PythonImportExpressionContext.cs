// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			} else if (entry is NamespaceEntry) {
				return IsImportAll(entry);
			}
			return false;
		}
		
		bool IsImportAll(ICompletionEntry entry)
		{
			return PythonImportCompletion.ImportAll.Equals(entry);
		}
	}
}
