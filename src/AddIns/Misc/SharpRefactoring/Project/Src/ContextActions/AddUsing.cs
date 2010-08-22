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
			var currentLineAST = context.CurrentLineAST;
			if (currentLineAST == null)
				yield break;
			var symbol = context.CurrentSymbol;
			foreach (var contextAction in GetAddUsingContextActions(symbol, context.Editor, context.CurrentExpression.Context)) {
				yield return contextAction;
			}
		}
		
		IEnumerable<IContextAction> GetAddUsingContextActions(ResolveResult symbol, ITextEditor editor, ExpressionContext exprContext)
		{
			foreach (var addUsingAction in RefactoringService.GetAddUsingActions(symbol,editor)) {
				yield return addUsingAction;
			}
			if (exprContext == ExpressionContext.Attribute) {
				foreach (var addUsingAction in GetAddUsingAttributeActions(symbol, editor)) {
					yield return addUsingAction;
				}
			}
			foreach (var addUsingAction in GetAddUsingExtensionMethodActions(symbol, editor)) {
				yield return addUsingAction;
			}
		}
		
		IEnumerable<IContextAction> GetAddUsingExtensionMethodActions(ResolveResult symbol, ITextEditor editor)
		{
			yield break;
		}
		
		#region GetAddUsingAttributeActions
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
				string newNamespace = c.Namespace;
				yield return new DelegateAction {Title = "using " + newNamespace,
					ExecuteAction = delegate {
						NamespaceRefactoringService.AddUsingDeclaration(unit, editor.Document, newNamespace, true);
						ParserService.BeginParse(editor.FileName, editor.Document);
					}
				};
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
