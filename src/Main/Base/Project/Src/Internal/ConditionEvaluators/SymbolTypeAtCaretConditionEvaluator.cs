// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Internal.ConditionEvaluators
{
	/// <summary>
	/// Condition evaluator checking the type of the symbol under the caret (if there is one).
	/// </summary>
	public class SymbolTypeAtCaretConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object parameter, Condition condition)
		{
			ResolveResult resolveResult = GetResolveResult();
			if ((resolveResult != null) && !resolveResult.IsError) {
				// Check type of symbol
				string typesList = condition.Properties["type"];
				if (typesList != null) {
					foreach (string type in typesList.Split(',')) {
						switch (type) {
							case "*":
								// Wildcard -> allow any type
								return true;
							case "member":
								// Allow members
								if (resolveResult is MemberResolveResult)
									return true;
								break;
							case "type":
								// Allow types
								if (resolveResult is TypeResolveResult)
									return true;
								break;
							case "namespace":
								// Allow namespaces
								if (resolveResult is NamespaceResolveResult)
									return true;
								break;
							case "local":
								// Allow locals
								if (resolveResult is LocalResolveResult)
									return true;
								break;
						}
					}
				}
			}
			
			return false;
		}
		
		static ResolveResult GetResolveResult()
		{
			ITextEditor currentEditor = SD.GetActiveViewContentService<ITextEditor>();
			if (currentEditor != null) {
				return SD.ParserService.Resolve(currentEditor, currentEditor.Caret.Location);
			} else {
				return ErrorResolveResult.UnknownError;
			}
		}
	}
}
