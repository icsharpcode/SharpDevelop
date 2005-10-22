/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 22.10.2005
 * Time: 14:41
 */

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;
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
