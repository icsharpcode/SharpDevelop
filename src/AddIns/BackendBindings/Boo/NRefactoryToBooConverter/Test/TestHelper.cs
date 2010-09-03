// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace NRefactoryToBooConverter.Tests
{
	public class TestHelper
	{
		protected virtual void ApplySettings(ConverterSettings settings)
		{
			settings.SimplifyTypeNames = false;
			settings.RemoveRedundantTypeReferences = false;
		}
		
		protected string Convert(string program)
		{
			ConverterSettings settings = new ConverterSettings("prog.cs");
			ApplySettings(settings);
			Module module = Parser.ParseModule(new CompileUnit(), new StringReader(program), settings);
			return GetStringFromModule(module, settings);
		}
		
		protected string ConvertVB(string program)
		{
			ConverterSettings settings = new ConverterSettings("prog.vb");
			ApplySettings(settings);
			Module module = Parser.ParseModule(new CompileUnit(), new StringReader(program), settings);
			return GetStringFromModule(module, settings);
		}
		
		string GetStringFromModule(Module module, ConverterSettings settings)
		{
			if (settings.Errors.Count > 0) {
				Assert.Fail(settings.Errors.Count.ToString() + " errors: " + settings.Errors[0]);
			}
			if (settings.Warnings.Count > 0) {
				Assert.Fail(settings.Warnings.Count.ToString() + " warnings: " + settings.Warnings[0]);
			}
			Assert.IsNotNull(module, "Module is null");
			string str = module.ToCodeString();
			str = str.Trim().Replace("\r", "");
			for (int i = 0; i < 5; i++) {
				str = str.Replace("\n\n", "\n");
				str = str.Replace("  ", " ");
			}
			return str;
		}
		
		protected void Test(string input, string output)
		{
			Assert.AreEqual(output.Replace("??", ConverterSettings.DefaultNameGenerationPrefix), Convert(input));
		}
		
		protected void TestVB(string input, string output)
		{
			Assert.AreEqual(output.Replace("??", ConverterSettings.DefaultNameGenerationPrefix), ConvertVB(input));
		}
		
		protected void TestInInterface(string input, string output)
		{
			Test("public interface ClassName {\n" + input + "\n}", "public interface ClassName:\n\t" + output.Replace("\n", "\n\t"));
		}
		
		protected void TestInClass(string input, string output)
		{
			Test("public class ClassName {\n" + input + "\n}", "public class ClassName:\n\t" + output.Replace("\n", "\n\t"));
		}
		
		protected void TestInClassVB(string input, string output)
		{
			TestVB("Public Class ClassName\n" + input + "\nEnd Class\n", "public class ClassName:\n\t" + output.Replace("\n", "\n\t"));
		}
		
		protected void TestStatement(string input, string output)
		{
			ConverterSettings dummySet = new ConverterSettings("dummy.cs");
			ApplySettings(dummySet);
			if (dummySet.RemoveRedundantTypeReferences) {
				TestInClass("public void Method() {\n" + input + "\n}", "public def Method():\n\t" + output.Replace("\n", "\n\t"));
			} else if (dummySet.SimplifyTypeNames) {
				TestInClass("public void Method() {\n" + input + "\n}", "public def Method() as void:\n\t" + output.Replace("\n", "\n\t"));
			} else {
				TestInClass("public void Method() {\n" + input + "\n}", "public def Method() as System.Void:\n\t" + output.Replace("\n", "\n\t"));
			}
		}
		
		protected void TestStatementVB(string input, string output)
		{
			TestInClassVB("Public Sub Method()\n" + input + "\nEnd Sub", "public def Method() as System.Void:\n\t" + output.Replace("\n", "\n\t"));
		}
		
		protected void TestExpr(string input, string output)
		{
			TestStatement("a = " + input + ";", "a = " + output);
		}
		
		protected void TestExprVB(string input, string output)
		{
			TestStatementVB("a = " + input, "a = " + output);
		}
	}
}
