// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Linq;
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
		public IEnumerable<IContextAction> GetAvailableActions(EditorContext editorContext)
		{
			// Using CurrentLineAST is basically OK, but when the "class" keyword is on different line than class name,
			// parsing only one line never tells us that we are looking at TypeDeclaration
			// Alternative solution could be to try to resolve also IdentifierExpression to see if it is class declaration.
			var ambience = AmbienceService.GetCurrentAmbience();
			
			foreach (var targetClass in editorContext.GetClassDeclarationsOnCurrentLine().
			         Where(c => c.ClassType == ClassType.Class || c.ClassType == ClassType.Interface)) {
				
				foreach (var implementAction in RefactoringService.GetImplementInterfaceActions(targetClass, false)) {
					var implementActionCopy = implementAction;
					yield return new DelegateAction {
						Title = string.Format("Implement interface {0}", ambience.Convert(implementActionCopy.ClassToImplement)),
						ExecuteAction = implementActionCopy.Execute
					};
				}
			}
		}
	}
}
