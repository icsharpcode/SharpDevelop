// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests the CSharpToPythonConverter class can convert a class with
	/// a private field that's an integer. What should happen is that
	/// since Python does not have the concept of class fields all 
	/// fields initialized in the class are moved to the start of the
	/// __init__ method. Note that this assumes there are no
	/// overloaded constructors otherwise we will get duplicated code.
	/// </summary>
	[TestFixture]
	public class IntegerClassFieldConversionTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeTypeDeclaration codeTypeDeclaration;
		CodeConstructor constructor;
		CodeAssignStatement assignStatement;
		CodePrimitiveExpression primitiveExpression;
		CodeFieldReferenceExpression fieldReferenceExpression;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tprivate int i = 0;\r\n" +
						"}";

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			converter = new CSharpToPythonConverter();
			codeCompileUnit = converter.ConvertToCodeCompileUnit(csharp);
			if (codeCompileUnit.Namespaces.Count > 0) {
				codeNamespace = codeCompileUnit.Namespaces[0];
				if (codeNamespace.Types.Count > 0) {
					codeTypeDeclaration = codeNamespace.Types[0];
					if (codeTypeDeclaration.Members.Count > 0) {
						constructor = codeTypeDeclaration.Members[0] as CodeConstructor;
						if (constructor.Statements.Count > 0) {
							assignStatement = constructor.Statements[0] as CodeAssignStatement;
							fieldReferenceExpression = assignStatement.Left as CodeFieldReferenceExpression;
							primitiveExpression = assignStatement.Right as CodePrimitiveExpression;
						}
					}
				}
			}
		}
			
		[Test]
		public void ConvertedPythonCode()
		{
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tself._i = 0";
			
			Assert.AreEqual(expectedPython, python);
		}		
		
		[Test]
		public void ClassHasConstructor()
		{
			Assert.IsNotNull(constructor);
		}
		
		[Test]
		public void ConstructorHasAssignStatement()
		{
			Assert.IsNotNull(assignStatement);
		}
		
		[Test]
		public void AssignStatementLeftIsFieldReferenceExpression()
		{
			Assert.IsNotNull(fieldReferenceExpression);
		}
		
		[Test]
		public void AssignStatementRightIsPrimitiveExpression()
		{
			Assert.IsNotNull(primitiveExpression);
		}
		
		[Test]
		public void FieldName()
		{
			Assert.AreEqual("_i", fieldReferenceExpression.FieldName);
		}
		
		[Test]
		public void FieldTargetObjectIsThisReference()
		{
			Assert.IsInstanceOfType(typeof(CodeThisReferenceExpression), fieldReferenceExpression.TargetObject);
		}
		
		[Test]
		public void PrimitiveExpressionIsIntegerZero()
		{
			Assert.AreEqual(0, primitiveExpression.Value);
		}
	}
}
