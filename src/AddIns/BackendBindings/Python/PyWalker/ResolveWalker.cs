// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using IronPython;
using IronPython.Compiler;
using IronPython.Compiler.Ast;

namespace PyWalker
{
	public interface IOutputWriter
	{
		void WriteLine(string s);		
	}

	public class ResolveWalker : PythonWalker
	{
		IOutputWriter writer;
		
		public ResolveWalker(IOutputWriter writer)
		{
			this.writer = writer;
		}
		
		public override bool Walk(AndExpression node)
		{
			writer.WriteLine("And");
			return base.Walk(node);
		}
		
		public override bool Walk(AssertStatement node)
		{
			writer.WriteLine("Assert");
			return base.Walk(node);
		}
		
		public override bool Walk(Arg node)
		{
			writer.WriteLine("Arg: " + node.Name.ToString());
			return base.Walk(node);
		}	
		
		public override bool Walk(AugmentedAssignStatement node)
		{
			writer.WriteLine("AugmentedAssignStatement");
			return base.Walk(node);
		}
		
		public override bool Walk(AssignmentStatement node)
		{
			writer.WriteLine("AssignmentStatement");
			return base.Walk(node);
		}
		
		public override bool Walk(BackQuoteExpression node)
		{
			writer.WriteLine("BackQuote");
			return base.Walk(node);
		}
		
		public override bool Walk(BinaryExpression node)
		{
			writer.WriteLine("Binary");
			return base.Walk(node);
		}
		
		public override bool Walk(BreakStatement node)
		{
			writer.WriteLine("Breaks");
			return base.Walk(node);
		}
		
		public override bool Walk(ClassDefinition node)
		{
			if (node.Bases.Count > 0) {
				writer.WriteLine("Class: " + node.Name + " BaseTypes: " + GetBaseTypes(node.Bases));
			} else {
				writer.WriteLine("Class: " + node.Name);
			}
			return base.Walk(node);
		}
		
		public override bool Walk(ConditionalExpression node)
		{
			writer.WriteLine("ConditionalExpression");
			return base.Walk(node);
		}
		
		public override bool Walk(ConstantExpression node)
		{
			writer.WriteLine("ConstantExpression");
			return base.Walk(node);
		}
		
		public override bool Walk(ContinueStatement node)
		{
			writer.WriteLine("Continue");
			return base.Walk(node);
		}
		
		public override bool Walk(PrintStatement node)
		{
			writer.WriteLine("PrintStatement");
			return base.Walk(node);
		}
		
		public override bool Walk(FunctionDefinition node)
		{
			writer.WriteLine("FunctionDefinition");
			return base.Walk(node);
		}

		public override bool Walk(CallExpression node)
		{
			writer.WriteLine("Call");
			return base.Walk(node);
		}

		public override bool Walk(DictionaryExpression node)
		{
			writer.WriteLine("Dict");
			return base.Walk(node);
		}
		
		public override bool Walk(DottedName node)
		{
			writer.WriteLine("DottedName");
			return base.Walk(node);
		}
		
		public override bool Walk(ExpressionStatement node)
		{
			writer.WriteLine("Expr");
			return base.Walk(node);
		}
		
		public override bool Walk(GlobalStatement node)
		{
			writer.WriteLine("Global");
			return base.Walk(node);
		}
		
		public override bool Walk(NameExpression node)
		{
			writer.WriteLine("Name: " + node.Name);
			return base.Walk(node);
		}
		
		public override bool Walk(MemberExpression node)
		{
			writer.WriteLine("Member: " + node.Name);
			return base.Walk(node);
		}
		
		public override bool Walk(FromImportStatement node)
		{
			writer.WriteLine("FromImport: " + node.Root.MakeString());
			return base.Walk(node);
		}
		
		public override bool Walk(ImportStatement node)
		{
			writer.WriteLine("Import: " + GetImports(node.Names));
			return base.Walk(node);
		}
		
		public override bool Walk(IndexExpression node)
		{
			writer.WriteLine("Index: " + node.Index.ToString());
			return base.Walk(node);
		}
		
		public override bool Walk(UnaryExpression node)
		{
			writer.WriteLine("Unary");
			return base.Walk(node);
		}
		
		public override bool Walk(SuiteStatement node)
		{
			writer.WriteLine("Suite");
			return base.Walk(node);
		}
		
		public override bool Walk(ErrorExpression node)
		{
			writer.WriteLine("Error");
			return base.Walk(node);
		}
		
		public override bool Walk(IfStatement node)
		{
			writer.WriteLine("If");
			return base.Walk(node);
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
