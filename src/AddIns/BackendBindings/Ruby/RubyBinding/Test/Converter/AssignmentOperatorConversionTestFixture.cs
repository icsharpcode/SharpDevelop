// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests ArithmeticOperatorType conversions:
	/// 
	/// 	None,
	///		Assign,
	///		
	///		Add, Done
	///		Subtract,
	///		Multiply,
	///		Divide,
	///		Modulus,
	///		
	///		Power,         // (VB only)
	///		DivideInteger, // (VB only)
	///		ConcatString,  // (VB only)
	///		
	///		ShiftLeft,
	///		ShiftRight,
	///		
	///		BitwiseAnd,
	///		BitwiseOr,
	///		ExclusiveOr
	/// </summary>
	[TestFixture]
	public class AssignmentOperatorConversionTestFixture
	{		
		string csharp =
			"class Foo\r\n" +
			"{\r\n" +
			"    public void Convert(int a)\r\n" +
			"    {\r\n" +
			"        a ASSIGNMENT_OPERATOR 5;\r\n" +
			"    }\r\n" +
			"}";
		
		string vb =
			"Public Class Foo\r\n" +
			"    Public Sub Convert(ByVal a as Integer)\r\n" +
			"        a ASSIGNMENT_OPERATOR 5;\r\n" +
			"    End Sub\r\n" +
			"End Class";
		
		string RubyCodeTemplate = 
			"class Foo\r\n" +
			"    def Convert(a)\r\n" +
			"        a ASSIGNMENT_OPERATOR 5\r\n" +
			"    end\r\n" +
			"end";
				
		[Test]
		public void MultiplyOperator()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(GetCode(csharp, "*="));
			string expectedRuby = GetCode(RubyCodeTemplate, "*=");
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
		
		[Test]
		public void DivideOperator()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(GetCode(csharp, "/="));
			string expectedRuby = GetCode(RubyCodeTemplate, "/=");
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
		
		[Test]
		public void ModulusOperator()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(GetCode(csharp, "%="));
			string expectedRuby = GetCode(RubyCodeTemplate, "%=");
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
		
		[Test]
		public void AndOperator()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(GetCode(csharp, "&="));
			string expectedRuby = GetCode(RubyCodeTemplate, "&=");
			
			Assert.AreEqual(expectedRuby, Ruby);
		}	
		
		[Test]
		public void OrOperator()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(GetCode(csharp, "|="));
			string expectedRuby = GetCode(RubyCodeTemplate, "|=");
			
			Assert.AreEqual(expectedRuby, Ruby);
		}	
		
		[Test]
		public void ExclusiveOrOperator()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(GetCode(csharp, "^="));
			string expectedRuby = GetCode(RubyCodeTemplate, "^=");
			
			Assert.AreEqual(expectedRuby, Ruby);
		}	
		
		[Test]
		public void ShiftLeftOperator()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(GetCode(csharp, "<<="));
			string expectedRuby = GetCode(RubyCodeTemplate, "<<=");
			
			Assert.AreEqual(expectedRuby, Ruby);
		}	
		
		[Test]
		public void ShiftRightOperator()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(GetCode(csharp, ">>="));
			string expectedRuby = GetCode(RubyCodeTemplate, ">>=");
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
		
		[Test]
		public void VBConcatOperator()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.VBNet);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(GetCode(vb, "&="));
			string expectedRuby = GetCode(RubyCodeTemplate, "+=");
			
			Assert.AreEqual(expectedRuby, Ruby);
		}		
		
		[Test]
		public void VBDivideIntegerOperator()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.VBNet);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(GetCode(vb, "\\="));
			string expectedRuby = GetCode(RubyCodeTemplate, "/=");
			
			Assert.AreEqual(expectedRuby, Ruby);
		}		
		
		[Test]
		public void VBPowerOperator()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.VBNet);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(GetCode(vb, "^="));
			string expectedRuby = GetCode(RubyCodeTemplate, "**=");
			
			Assert.AreEqual(expectedRuby, Ruby);
		}				
				
		string GetCode(string code, string assignmentOperator)
		{
			return code.Replace("ASSIGNMENT_OPERATOR", assignmentOperator);
		}
	}
}
