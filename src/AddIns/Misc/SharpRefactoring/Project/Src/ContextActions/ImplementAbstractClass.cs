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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of ImplementAbstractClass.
	/// </summary>
	public class ImplementAbstractClassProvider : IContextActionsProvider
	{
		public IEnumerable<IContextAction> GetAvailableActions(EditorContext editorContext)
		{
			var ambience = AmbienceService.GetCurrentAmbience();
			
			foreach (var targetClass in editorContext.GetClassDeclarationsOnCurrentLine().Where(c => c.ClassType == ClassType.Class)) {
				
				foreach (var implementAction in RefactoringService.GetImplementAbstractClassActions(targetClass)) {
					var implementActionCopy = implementAction;
					yield return new DelegateAction {
						Title = string.Format("Implement abstract class {0}", ambience.Convert(implementActionCopy.ClassToImplement)),
						ExecuteAction = implementActionCopy.Execute
					};
				}
			}
		}
	}
}
