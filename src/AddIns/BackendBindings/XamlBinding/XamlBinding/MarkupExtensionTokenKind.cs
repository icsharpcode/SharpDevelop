// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.XamlBinding
{
	public enum MarkupExtensionTokenKind
	{
		EndOfFile,
		OpenBrace,
		CloseBrace,
		Equals,
		Comma,
		TypeName,
		MemberName,
		String
	}
}
