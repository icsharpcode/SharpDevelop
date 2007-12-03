// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using IronPython;
using IronPython.CodeDom;
using IronPython.Compiler;
using IronPython.Compiler.Ast;

namespace PyWalker
{
	public interface IOutputWriter
	{
		void WriteLine(string s);		
	}

	public class ResolveWalker : AstWalker
	{
		IOutputWriter writer;
		
		public ResolveWalker(IOutputWriter writer)
		{
			this.writer = writer;
		}
		
		public override bool Walk(AndExpression node)
		{
			writer.WriteLine("And");
			return true;
		}
		
		public override bool Walk(AssertStatement node)
		{
			writer.WriteLine("Assert");
			return true;
		}
		
		public override bool Walk(AugAssignStatement node)
		{
			writer.WriteLine("AugAssert");
			return true;
		}
		
		public override bool Walk(Arg node)
		{
			writer.WriteLine("Arg: " + node.Name.ToString());
			return true;
		}	
		
		public override bool Walk(AssignStatement node)
		{
			writer.WriteLine("Assign");
			return true;
		}
		
		public override bool Walk(BackQuoteExpression node)
		{
			writer.WriteLine("BackQuote");
			return true;
		}
		
		public override bool Walk(BinaryExpression node)
		{
			writer.WriteLine("Binary");
			return true;
		}
		
		public override bool Walk(BreakStatement node)
		{
			writer.WriteLine("Breaks");
			return true;
		}
		
		public override bool Walk(ClassDefinition node)
		{
			if (node.Bases.Count > 0) {
				writer.WriteLine("Class: " + node.Name.ToString() + " BaseTypes: " + GetBaseTypes(node.Bases));
			} else {
				writer.WriteLine("Class: " + node.Name.ToString());
			}
			return true;
		}
		
		public override bool Walk(ConditionalExpression node)
		{
			writer.WriteLine("ConditionalExpression");
			return true;
		}
		
		public override bool Walk(ConstantExpression node)
		{
			writer.WriteLine("ConstantExpression");
			return true;
		}
		
		public override bool Walk(ContinueStatement node)
		{
			writer.WriteLine("Continue");
			return true;
		}
		
		public override bool Walk(PrintStatement node)
		{
			writer.WriteLine("PrintStatement");
			return true;
		}
		
		public override bool Walk(FunctionDefinition node)
		{
			writer.WriteLine("FunctionDefinition");
			return true;
		}

		public override bool Walk(CallExpression node)
		{
			writer.WriteLine("Call");
			return true;
		}

		public override bool Walk(DictionaryExpression node)
		{
			writer.WriteLine("Dict");
			return true;
		}
		
		public override bool Walk(DottedName node)
		{
			writer.WriteLine("DottedName");
			return true;
		}
		
		public override bool Walk(ExpressionStatement node)
		{
			writer.WriteLine("Expr");
			return true;
		}
		
		public override bool Walk(FieldExpression node)
		{
			writer.WriteLine("Field: " + node.Name.ToString());
			return true;
		}
		
		public override bool Walk(GlobalStatement node)
		{
			writer.WriteLine("Global");
			return true;
		}
		
		public override bool Walk(NameExpression node)
		{
			writer.WriteLine("Name: " + node.Name);
			return true;
		}
		
		public override bool Walk(FromImportStatement node)
		{
			writer.WriteLine("FromImport: " + node.Root.MakeString());
			return true;
		}
		
		public override bool Walk(ImportStatement node)
		{
			writer.WriteLine("Import: " + GetImports(node.Names));
			return true;
		}
		
		public override bool Walk(UnaryExpression node)
		{
			writer.WriteLine("Unary");
			return true;
		}
		
		public override bool Walk(SuiteStatement node)
		{
			writer.WriteLine("Suite");
			return true;
		}
		
		public override bool Walk(GlobalSuite node)
		{
			writer.WriteLine("GlobalSuite");
			return true;
		}
		
		public override bool Walk(ErrorExpression node)
		{
			writer.WriteLine("Error");
			return true;
		}
		
		public override bool Walk(IfStatement node)
		{
			writer.WriteLine("If");
			return true;
		}
				
		string GetImports(IList<DottedName> names)
		{
			StringBuilder s = new StringBuilder();
			foreach (DottedName name in names) {
				s.Append(name.MakeString());
				s.Append(',');
			}
			return s.ToString();
		}
		
		string GetBaseTypes(IList<Expression> types)
		{
			StringBuilder s = new StringBuilder();
			foreach (Expression expression in types) {
				NameExpression nameExpression = expression as NameExpression;
				if (nameExpression != null) {
					s.Append(nameExpression.Name.ToString());
					s.Append(',');
				}
			}
			return s.ToString();
		}

	}
}
