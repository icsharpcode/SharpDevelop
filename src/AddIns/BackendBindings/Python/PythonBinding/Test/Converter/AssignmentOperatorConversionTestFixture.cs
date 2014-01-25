// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
