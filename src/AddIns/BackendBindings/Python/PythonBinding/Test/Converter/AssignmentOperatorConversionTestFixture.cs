// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
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
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(GetCode(csharp, "*="));
			string expectedPython = GetCode(pythonCodeTemplate, "*=");
			
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void DivideOperator()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(GetCode(csharp, "/="));
			string expectedPython = GetCode(pythonCodeTemplate, "/=");
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void AndOperator()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(GetCode(csharp, "&="));
			string expectedPython = GetCode(pythonCodeTemplate, "&=");
			
			Assert.AreEqual(expectedPython, python);
		}	
		
		[Test]
		public void OrOperator()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(GetCode(csharp, "|="));
			string expectedPython = GetCode(pythonCodeTemplate, "|=");
			
			Assert.AreEqual(expectedPython, python);
		}	
		
		[Test]
		public void ExclusiveOrOperator()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(GetCode(csharp, "^="));
			string expectedPython = GetCode(pythonCodeTemplate, "^=");
			
			Assert.AreEqual(expectedPython, python);
		}	
		
		[Test]
		public void ShiftLeftOperator()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(GetCode(csharp, "<<="));
			string expectedPython = GetCode(pythonCodeTemplate, "<<=");
			
			Assert.AreEqual(expectedPython, python);
		}	
		
		[Test]
		public void ShiftRightOperator()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(GetCode(csharp, ">>="));
			string expectedPython = GetCode(pythonCodeTemplate, ">>=");
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void VBConcatOperator()
		{
			VBNetToPythonConverter converter = new VBNetToPythonConverter();
			string python = converter.Convert(GetCode(vb, "&="));
			string expectedPython = GetCode(pythonCodeTemplate, "+=");
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void VBDivideIntegerOperator()
		{
			VBNetToPythonConverter converter = new VBNetToPythonConverter();
			string python = converter.Convert(GetCode(vb, "\\="));
			string expectedPython = GetCode(pythonCodeTemplate, "/=");
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void VBPowerOperator()
		{
			VBNetToPythonConverter converter = new VBNetToPythonConverter();
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

