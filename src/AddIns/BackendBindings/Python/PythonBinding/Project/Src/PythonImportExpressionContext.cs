// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
