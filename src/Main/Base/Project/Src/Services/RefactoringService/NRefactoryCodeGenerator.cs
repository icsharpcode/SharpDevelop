// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public abstract class NRefactoryCodeGenerator : CodeGenerator
	{
		public abstract IOutputASTVisitor CreateOutputVisitor();
		
		public override string GenerateCode(AbstractNode node, string indentation)
		{
			IOutputASTVisitor visitor = CreateOutputVisitor();
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
			return visitor.Text;
		}
	}
	
	public class CSharpCodeGenerator : NRefactoryCodeGenerator
	{
		public static readonly CSharpCodeGenerator Instance = new CSharpCodeGenerator();
		
		public override IOutputASTVisitor CreateOutputVisitor()
		{
			return new CSharpOutputVisitor();
		}
	}
	
	public class VBNetCodeGenerator : NRefactoryCodeGenerator
	{
		public static readonly VBNetCodeGenerator Instance = new VBNetCodeGenerator();
		
		public override IOutputASTVisitor CreateOutputVisitor()
		{
			return new VBNetOutputVisitor();
		}
	}
}
