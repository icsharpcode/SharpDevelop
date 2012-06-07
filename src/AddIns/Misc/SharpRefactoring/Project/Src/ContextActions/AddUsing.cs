// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;
using System.Linq;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of AddUsing.
	/// </summary>
	public class AddUsingProvider : ContextActionsProvider
	{
		public override IEnumerable<IContextAction> GetAvailableActions(EditorContext context)
		{
			// class
			foreach (var addUsingAction in RefactoringService.GetAddUsingActions(context.CurrentSymbol, context.Editor)) {
				yield return addUsingAction;
			}
			// extension method
			if (context.CurrentExpression.Context != ExpressionContext.Attribute) {
				foreach (var addUsingAction in GetAddUsingExtensionMethodActions(context)) {
					yield return addUsingAction;
				}
			}
			// attribute
			if (context.CurrentExpression.Context == ExpressionContext.Attribute) {
				foreach (var addUsingAction in GetAddUsingAttributeActions(context)) {
					yield return addUsingAction;
				}
			}
		}
		
		#region Extension method
		IEnumerable<IContextAction> GetAddUsingExtensionMethodActions(EditorContext context)
		{
			UnknownMethodResolveResult rr = context.CurrentSymbol as UnknownMethodResolveResult;
			if (rr == null)
				yield break;
			
			List<IClass> results = new List<IClass>();
			IProjectContent pc = context.ProjectContent;
			
			SearchAllExtensionMethodsWithName(results, pc, rr.CallName);
			foreach (IProjectContent content in pc.ThreadSafeGetReferencedContents())
				SearchAllExtensionMethodsWithName(results, content, rr.CallName);
			
			foreach (IClass c in results) {
				yield return new RefactoringService.AddUsingAction(context.CurrentParseInformation.CompilationUnit, context.Editor, c.Namespace);
			}
		}
		
		void SearchAllExtensionMethodsWithName(List<IClass> searchResults, IProjectContent pc, string name)
		{
			foreach (IClass c in pc.Classes) {
				if (c.HasExtensionMethods && !searchResults.Any(cl => cl.Namespace == c.Namespace) &&
				    c.Methods.Any(m => m.IsExtensionMethod && m.Name == name))
					searchResults.Add(c);
			}
		}
		#endregion
		
		#region Attribute
		IEnumerable<IContextAction> GetAddUsingAttributeActions(EditorContext context)
		{
			ResolveResult symbol = context.CurrentSymbol;
			if (!(symbol is UnknownIdentifierResolveResult || symbol is UnknownMethodResolveResult))
				yield break;

			List<IClass> results = new List<IClass>();
			
			ParseInformation info = context.CurrentParseInformation;
			if (info == null || info.CompilationUnit == null || info.CompilationUnit.ProjectContent == null)
				yield break;
			ICompilationUnit unit = info.CompilationUnit;
			IProjectContent pc = info.CompilationUnit.ProjectContent;
			
			string name = null;
			if (symbol is UnknownMethodResolveResult) {
				name = Search((UnknownMethodResolveResult)symbol, pc, results);
			}
			if (symbol is UnknownIdentifierResolveResult) {
				name = Search((UnknownIdentifierResolveResult)symbol, pc, results);
			} else {
				yield break;
			}
			
			foreach (IClass c in results) {
				yield return new RefactoringService.AddUsingAction(unit, context.Editor, c.Namespace);
			}
		}
		
		public string Search(UnknownMethodResolveResult rr, IProjectContent pc, List<IClass> results)
		{
			SearchAttributesWithName(results, pc, rr.CallName);
			foreach (IProjectContent content in pc.ThreadSafeGetReferencedContents())
				SearchAttributesWithName(results, content, rr.CallName);
			return rr.CallName;
		}
		
		public string Search(UnknownIdentifierResolveResult rr, IProjectContent pc, List<IClass> results)
		{
			SearchAttributesWithName(results, pc, rr.Identifier);
			foreach (IProjectContent content in pc.ThreadSafeGetReferencedContents())
				SearchAttributesWithName(results, content, rr.Identifier);
			return rr.Identifier;
		}
		
		IClass baseClass = null;
		void SearchAttributesWithName(List<IClass> results, IProjectContent pc, string name)
		{
			if (baseClass == null)
				baseClass = pc.GetClass("System.Attribute", 0);
			foreach (IClass c in pc.Classes) {
				if ((c.Name == name || c.Name == name + "Attribute") && c.IsTypeInInheritanceTree(baseClass))
					results.Add(c);
			}
		}
		#endregion
	}
}
