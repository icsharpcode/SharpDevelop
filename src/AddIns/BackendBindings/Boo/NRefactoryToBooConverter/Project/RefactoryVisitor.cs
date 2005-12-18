// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace NRefactoryToBooConverter
{
	/// <summary>
	/// Visitor that prepares the conversion by converting source-language specific constructs
	/// into "better suited" constructs.
	/// </summary>
	/// <example>
	/// ForStatements of the form "for(int i = Start; i &lt; End; i += Step)" are
	/// converted to "For i As Integer = Start To End Step Step" (VB-Style) which has the better-matching
	/// Boo representation of "for i as int in range(Start, End, Step):"
	/// </example>
	public class RefactoryVisitor : CSharpToVBNetConvertVisitor
	{
		public RefactoryVisitor()
		{
			base.RenameConflictingFieldNames = false; // do not rename fields to VB-style
		}
	}
}
