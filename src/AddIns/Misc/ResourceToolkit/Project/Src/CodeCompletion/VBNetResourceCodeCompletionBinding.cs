// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop.Editor;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Provides code completion for inserting resource keys in VB.
	/// </summary>
	public class VBNetResourceCodeCompletionBinding : AbstractNRefactoryResourceCodeCompletionBinding
	{
		
		/// <summary>
		/// Determines if the specified character should trigger resource resolve attempt and possibly code completion at the current position.
		/// </summary>
		protected override bool CompletionPossible(ITextEditor editor, char ch)
		{
			return ch == '(';
		}
		
		/// <summary>
		/// Gets a VBNetOutputVisitor used to generate the inserted code.
		/// </summary>
		protected override IOutputAstVisitor OutputVisitor {
			get {
				return new VBNetOutputVisitor();
			}
		}
		
	}
}
