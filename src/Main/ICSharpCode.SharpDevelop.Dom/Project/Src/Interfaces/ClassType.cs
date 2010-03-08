// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

namespace ICSharpCode.SharpDevelop.Dom
{
	public enum ClassType {
		Class = ICSharpCode.NRefactory.Ast.ClassType.Class,
		Enum = ICSharpCode.NRefactory.Ast.ClassType.Enum,
		Interface = ICSharpCode.NRefactory.Ast.ClassType.Interface,
		Struct = ICSharpCode.NRefactory.Ast.ClassType.Struct,
		Delegate = 0x5,
		Module = ICSharpCode.NRefactory.Ast.ClassType.Module
	}
}
