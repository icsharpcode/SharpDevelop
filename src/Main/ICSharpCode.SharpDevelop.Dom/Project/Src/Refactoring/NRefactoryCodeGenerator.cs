// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.SharpDevelop.Dom.Refactoring
{
	public abstract class NRefactoryCodeGenerator : CodeGenerator
	{
		public abstract IOutputAstVisitor CreateOutputVisitor();
		
		public override string GenerateCode(AbstractNode node, string indentation)
		{
			IOutputAstVisitor visitor = CreateOutputVisitor();
			int indentCount = 0;
			foreach (char c in indentation) {
				if (c == '\t')
					indentCount += 4;
				else
					indentCount += 1;
			}
			visitor.OutputFormatter.IndentationLevel = indentCount / 4;
			if (node is Statement)
				visitor.OutputFormatter.Indent();
			node.AcceptVisitor(visitor, null);
			string text = visitor.Text;
			if (node is Statement && !text.EndsWith("\n"))
				text += Environment.NewLine;
			return text;
		}
	}
	
	public class CSharpCodeGenerator : NRefactoryCodeGenerator
	{
		public static readonly CSharpCodeGenerator Instance = new CSharpCodeGenerator();
		
		public override IOutputAstVisitor CreateOutputVisitor()
		{
			return new CSharpOutputVisitor();
		}
	}
	
	public class VBNetCodeGenerator : NRefactoryCodeGenerator
	{
		public static readonly VBNetCodeGenerator Instance = new VBNetCodeGenerator();
		
		public override IOutputAstVisitor CreateOutputVisitor()
		{
			return new VBNetOutputVisitor();
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
