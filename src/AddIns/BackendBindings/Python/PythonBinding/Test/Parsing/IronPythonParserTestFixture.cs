// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Scripting.Runtime;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using IronPython;
using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Runtime;
using NUnit.Framework;

namespace PythonBinding.Tests.Parsing
{
	/// <summary>
	/// Tests the IronPython's parser.
	/// </summary>
	[TestFixture]
	public class IronPythonParserTestFixture
	{
		/// <summary>
		/// Cannot create PrintStatement for code com.
		/// </summary>
//		[Test]s
//		public void Test()
//		{
//			string pythonScript = "print 'Hello'";
//			PythonProvider provider = new PythonProvider();
//			CodeCompileUnit unit = provider.Parse(new StringReader(pythonScript));
//		}
		
		[Test]
		public void Test2()
		{
//			string pythonScript = "print 'Hello'";
//			CompilerContext context = new CompilerContext();
//			Parser parser = Parser.FromString(null, context, pythonScript);
//			Statement statement = parser.ParseFileInput();
		}
		
		[Test]
		public void Test3()
		{
//			string pythonScript = "Console.WriteLine";
//			CompilerContext context = new CompilerContext();
//			Parser parser = Parser.FromString(null, context, pythonScript);
//			Statement statement = parser.ParseFileInput();
//			ResolveVisitor visitor = new ResolveVisitor();
//			statement.Walk(visitor);
//			Console.WriteLine(statement.GetType().FullName);
		}
	}
	
	public class ResolveVisitor : PythonWalker
	{
		public override bool Walk(NameExpression node)
		{
			System.Console.WriteLine("NameExpression: " + node.Name);
			return true;
		}
	}
}
