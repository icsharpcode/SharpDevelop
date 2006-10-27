// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Provides code completion for inserting resource keys in C#.
	/// </summary>
	public class CSharpResourceCodeCompletionBinding : AbstractNRefactoryResourceCodeCompletionBinding
	{
		
		/// <summary>
		/// Determines if the specified character should trigger resource resolve attempt and possibly code completion at the current position.
		/// </summary>
		protected override bool CompletionPossible(SharpDevelopTextAreaControl editor, char ch)
		{
			return ch == '(' || ch == '[';
		}
		
		/// <summary>
		/// Gets a CSharpOutputVisitor used to generate the inserted code.
		/// </summary>
		protected override IOutputAstVisitor OutputVisitor {
			get {
				return new CSharpOutputVisitor();
			}
		}
		
	}
}
