// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2229 $</version>
// </file>

using System;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.SharpDevelop.Dom.Refactoring
{
	public class VBNetCodeGenerator : NRefactoryCodeGenerator
	{
		internal static readonly VBNetCodeGenerator Instance = new VBNetCodeGenerator();
		
		public override IOutputAstVisitor CreateOutputVisitor()
		{
			VBNetOutputVisitor v = new VBNetOutputVisitor();
			VBNetPrettyPrintOptions pOpt = v.Options;
			
			pOpt.IndentationChar = this.Options.IndentString[0];
			pOpt.IndentSize = this.Options.IndentString.Length;
			pOpt.TabSize = this.Options.IndentString.Length;
			
			return v;
		}
		
		public override PropertyDeclaration CreateProperty(IField field, bool createGetter, bool createSetter)
		{
			string propertyName = GetPropertyName(field.Name);
			if (string.Equals(propertyName, field.Name, StringComparison.InvariantCultureIgnoreCase)) {
				if (HostCallback.RenameMember(field, "m_" + field.Name)) {
					field = new DefaultField(field.ReturnType, "m_" + field.Name,
					                         field.Modifiers, field.Region, field.DeclaringType);
				}
			}
			return base.CreateProperty(field, createGetter, createSetter);
		}
	}
}
