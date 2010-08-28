// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
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
			foreach (var contextAction in GetAddUsingContextActions(context.CurrentSymbol, context.Editor, context.CurrentExpression.Context)) {
				yield return contextAction;
			}
		}
		
		IEnumerable<IContextAction> GetAddUsingContextActions(ResolveResult symbol, ITextEditor editor, ExpressionContext exprContext)
		{
			// class
			foreach (var addUsingAction in RefactoringService.GetAddUsingActions(symbol,editor)) {
				yield return addUsingAction;
			}
			// extension method
			if (exprContext != ExpressionContext.Attribute) {
				foreach (var addUsingAction in GetAddUsingExtensionMethodActions(symbol, editor)) {
					yield return addUsingAction;
				}
			}
			// attribute
			if (exprContext == ExpressionContext.Attribute) {
				foreach (var addUsingAction in GetAddUsingAttributeActions(symbol, editor)) {
					yield return addUsingAction;
				}
			}
		}
		
		#region Extension method
		IEnumerable<IContextAction> GetAddUsingExtensionMethodActions(ResolveResult symbol, ITextEditor editor)
		{
			if (!(symbol is UnknownMethodResolveResult))
				yield break;
			
			UnknownMethodResolveResult rr = symbol as UnknownMethodResolveResult;
			List<IClass> results = new List<IClass>();
			IProjectContent pc = rr.CallingClass.ProjectContent;
			
			SearchAllExtensionMethodsWithName(results, pc, rr.CallName);
			foreach (IProjectContent content in pc.ReferencedContents)
				SearchAllExtensionMethodsWithName(results, content, rr.CallName);
			if (!results.Any())
				yield break;
			
			foreach (IClass c in results) {
				yield return new RefactoringService.AddUsingAction(rr.CallingClass.CompilationUnit, editor, c.Namespace);
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
		IEnumerable<IContextAction> GetAddUsingAttributeActions(ResolveResult symbol, ITextEditor editor)
		{
			if (!(symbol is UnknownIdentifierResolveResult || symbol is UnknownMethodResolveResult))
				yield break;

			List<IClass> results = new List<IClass>();
			
			ParseInformation info = ParserService.GetParseInformation(editor.FileName);
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
				yield return new RefactoringService.AddUsingAction(unit, editor, c.Namespace);
			}
		}
		
		public string Search(UnknownMethodResolveResult rr, IProjectContent pc, List<IClass> results)
		{
			SearchAttributesWithName(results, pc, rr.CallName);
			foreach (IProjectContent content in pc.ReferencedContents)
				SearchAttributesWithName(results, content, rr.CallName);
			return rr.CallName;
		}
		
		public string Search(UnknownIdentifierResolveResult rr, IProjectContent pc, List<IClass> results)
		{
			SearchAttributesWithName(results, pc, rr.Identifier);
			foreach (IProjectContent content in pc.ReferencedContents)
				SearchAttributesWithName(results, content, rr.Identifier);
			return rr.Identifier;
		}
		
		IClass baseClass = null;
		void SearchAttributesWithName(List<IClass> results, IProjectContent pc, string name)
		{
			if (baseClass == null)
				baseClass = pc.GetClass("System.Attribute", 0);
			foreach (IClass c in pc.Classes) {
				if (c.IsTypeInInheritanceTree(baseClass) && (c.Name == name || c.Name == name + "Attribute"))
					results.Add(c);
			}
		}
		#endregion
	}
}
