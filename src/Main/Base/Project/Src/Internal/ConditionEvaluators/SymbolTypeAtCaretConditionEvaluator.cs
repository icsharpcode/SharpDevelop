// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Commands;

namespace ICSharpCode.SharpDevelop.Internal.ConditionEvaluators
{
	/// <summary>
	/// Condition evaluator checking the type of the symbol under the caret (if there is one).
	/// </summary>
	public class SymbolTypeAtCaretConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object parameter, Condition condition)
		{
			ResolveResult resolveResult = GetResolveResult(parameter);
			if ((resolveResult != null) && !resolveResult.IsError) {
				// Check if project-only entities should be valid
				var defRegion = resolveResult.GetDefinitionRegion();
				if ((condition.Properties["projectonly"] == "true") && (resolveResult.GetDefinitionRegion().IsEmpty))
					return false;
				
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
		
		static ResolveResult GetResolveResult(object parameter)
		{
			if (parameter is ITextEditor)
				return GetResolveResultFromCurrentEditor((ITextEditor) parameter);
			
			if (parameter is ResolveResult)
				return (ResolveResult) parameter;
			
			if (parameter is IEntityModel)
				return GetResolveResultFromEntityModel((IEntityModel) parameter);
			
			return GetResolveResultFromCurrentEditor();
		}
		
		static ResolveResult GetResolveResultFromEntityModel(IEntityModel entityModel)
		{
			IEntity entity = entityModel.Resolve();
			if (entity is IMember)
				return new MemberResolveResult(null, (IMember) entity);
			if (entity is ITypeDefinition)
				return new TypeResolveResult((ITypeDefinition) entity);
			return ErrorResolveResult.UnknownError;
		}
		
		static ResolveResult GetResolveResultFromCurrentEditor()
		{
			ITextEditor currentEditor = SD.GetActiveViewContentService<ITextEditor>();
			if (currentEditor != null)
				return GetResolveResultFromCurrentEditor(currentEditor);
			
			return null;
		}
		
		static ResolveResult GetResolveResultFromCurrentEditor(ITextEditor currentEditor)
		{
			if (currentEditor != null) {
				return SD.ParserService.Resolve(currentEditor, currentEditor.Caret.Location);
			}
			
			return ErrorResolveResult.UnknownError;
		}
	}
}
