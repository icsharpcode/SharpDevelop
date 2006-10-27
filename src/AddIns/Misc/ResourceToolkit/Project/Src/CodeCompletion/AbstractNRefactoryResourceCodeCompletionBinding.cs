// <file>
//     <copyright see="prj:///Doc/copyright.txt"/>
//     <license see="prj:///Doc/license.txt"/>
//     <owner name="Christian Hornung" email="c-hornung@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using Hornung.ResourceToolkit.Resolver;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Provides a base class for code completion for inserting resource keys using NRefactory.
	/// </summary>
	public abstract class AbstractNRefactoryResourceCodeCompletionBinding : DefaultCodeCompletionBinding
	{
		
		public override bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch)
		{
			
			if (this.CompletionPossible(editor, ch)) {
				
				ResourceResolveResult result = ResourceResolverService.Resolve(editor);
				if (result != null) {
					if (result.ResourceFileContent != null) {
						editor.ShowCompletionWindow(new ResourceCodeCompletionDataProvider(result.ResourceFileContent, this.OutputVisitor, result.CallingClass != null ? result.CallingClass.Name+"." : null), ch);
						return true;
					}
				}
				
			}
			
			return false;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Determines if the specified character should trigger resource resolve attempt and possibly code completion at the current position.
		/// </summary>
		protected abstract bool CompletionPossible(SharpDevelopTextAreaControl editor, char ch);
		
		/// <summary>
		/// Gets an NRefactory output visitor used to generate the inserted code.
		/// </summary>
		protected abstract IOutputAstVisitor OutputVisitor {
			get;
		}
		
	}
}
