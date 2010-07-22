// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Windows;
using ICSharpCode.NRefactory;
using Ast = ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of ImplementInterface.
	/// </summary>
	public class ImplementInterfaceProvider : IContextActionsProvider
	{
		public IEnumerable<IContextAction> GetAvailableActions(EditorASTProvider editorAST)
		{
			// Using CurrentLineAST is basically OK, but when the "class" keyword is on different line than class name,
			// parsing only one line never tells us that we are looking at TypeDeclaration
			
			// Alternative solution could be to try to resolve also IdentifierExpression to see if it is class declaration.
			var currentLineAST = editorAST.CurrentLineAST;
			if (currentLineAST == null)
				yield break;
			var editor = editorAST.Editor;
			foreach (var declaration in currentLineAST.FindTypeDeclarations()) {
				if (declaration.Type == Ast.ClassType.Class || declaration.Type == Ast.ClassType.Struct) {
					var rr = ParserService.Resolve(new ExpressionResult(declaration.Name), editor.Caret.Line, editor.Caret.Column, editor.FileName, editor.Document.Text);
					
				}
			} 
		}
	}
	
	public class ImplementInterfaceAction : IContextAction
	{
		public string Title {
			get { return "Dummy implement interface"; }
		}
		
		public void Execute()
		{
			MessageBox.Show("Dummy implement interface");
		}
	}
}
