// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
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
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic void Convert(int a)\r\n" +
						"\t{\r\n" +
						"\t\ta ASSIGNMENT_OPERATOR 5;\r\n" +
						"\t}\r\n" +
						"}";
		
		string vb = "Namespace DefaultNamespace\r\n" +
					"\tPublic Class Foo\r\n" +
					"\t\tPublic Sub Convert(ByVal a as Integer)\r\n" +
					"\t\t\ta ASSIGNMENT_OPERATOR 5;\r\n" +
					"\t\tEnd Sub\r\n" +
					"\tEnd Class\r\n" +
					"End Namespace";
		
		string pythonCodeTemplate = "class Foo(object):\r\n" +
							    	"\tdef Convert(self, a):\r\n" +
									"\t\ta ASSIGNMENT_OPERATOR 5";
				
		[Test]
		public void MultiplyOperator()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(GetCode(csharp, "*="));
			string expectedPython = GetCode(pythonCodeTemplate, "*=");
			
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void DivideOperator()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(GetCode(csharp, "/="));
			string expectedPython = GetCode(pythonCodeTemplate, "/=");
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void AndOperator()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(GetCode(csharp, "&="));
			string expectedPython = GetCode(pythonCodeTemplate, "&=");
			
			Assert.AreEqual(expectedPython, python);
		}	
		
		[Test]
		public void OrOperator()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(GetCode(csharp, "|="));
			string expectedPython = GetCode(pythonCodeTemplate, "|=");
			
			Assert.AreEqual(expectedPython, python);
		}	
		
		[Test]
		public void ExclusiveOrOperator()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(GetCode(csharp, "^="));
			string expectedPython = GetCode(pythonCodeTemplate, "^=");
			
			Assert.AreEqual(expectedPython, python);
		}	
		
		[Test]
		public void ShiftLeftOperator()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(GetCode(csharp, "<<="));
			string expectedPython = GetCode(pythonCodeTemplate, "<<=");
			
			Assert.AreEqual(expectedPython, python);
		}	
		
		[Test]
		public void ShiftRightOperator()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(GetCode(csharp, ">>="));
			string expectedPython = GetCode(pythonCodeTemplate, ">>=");
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void VBConcatOperator()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.VBNet);
			string python = converter.Convert(GetCode(vb, "&="));
			string expectedPython = GetCode(pythonCodeTemplate, "+=");
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void VBDivideIntegerOperator()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.VBNet);
			string python = converter.Convert(GetCode(vb, "\\="));
			string expectedPython = GetCode(pythonCodeTemplate, "/=");
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void VBPowerOperator()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.VBNet);
			string python = converter.Convert(GetCode(vb, "^="));
			string expectedPython = GetCode(pythonCodeTemplate, "**=");
			
			Assert.AreEqual(expectedPython, python);
		}				
				
		string GetCode(string code, string assignmentOperator)
		{
			return code.Replace("ASSIGNMENT_OPERATOR", assignmentOperator);
		}
	}
}
