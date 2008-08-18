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
	/// Tests that an array is converted from C# to Python.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class ArrayConversionTestFixture
	{		
		CodeArrayCreateExpression arrayCreate;
		CodeArrayIndexerExpression arrayIndexer;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic int[] Run()\r\n" +
						"\t{" +
						"\t\tint[] i = new int[] {1, 2, 3, 4};\r\n" +
						"\t\ti[0] = 5;\r\n" +
						"\t\treturn i;\r\n" +
						"\t}\r\n" +
						"}";
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			CodeCompileUnit unit = converter.ConvertToCodeCompileUnit(csharp);
			if (unit.Namespaces.Count > 0) {
				CodeNamespace ns = unit.Namespaces[0];
				if (ns.Types.Count > 0) {
					CodeTypeDeclaration type = ns.Types[0];
					if (type.Members.Count > 0) {
						CodeMemberMethod method = type.Members[0] as CodeMemberMethod;
						if (method != null && method.Statements.Count > 1) {
							CodeVariableDeclarationStatement variableDeclaration = method.Statements[0] as CodeVariableDeclarationStatement;
							if (variableDeclaration != null) {
								arrayCreate = variableDeclaration.InitExpression as CodeArrayCreateExpression;
							}
							CodeAssignStatement assignStatement = method.Statements[1] as CodeAssignStatement;
							arrayIndexer = assignStatement.Left as CodeArrayIndexerExpression;
						}
					}
				}
			}
		}
		
		[Test]
		public void ConvertedPythonCode()
		{
			string expectedCode = "class Foo(object):\r\n" +
									"\tdef Run(self):\r\n" +
									"\t\ti = System.Array[int]((1, 2, 3, 4, ))\r\n" +
									"\t\ti[0] = 5\r\n" +
									"\t\treturn i";
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code);
		}	
		
		[Test]
		public void ArrayCreateStatementExists()
		{
			Assert.IsNotNull(arrayCreate);
		}
		
		[Test]
		public void ArrayType()
		{
			Assert.AreEqual("int", arrayCreate.CreateType.BaseType);
		}
		
		[Test]
		public void ArrayHasInitialValues()
		{
			Assert.AreEqual(4, arrayCreate.Initializers.Count);
		}
		
		[Test]
		public void ArrayHasPrimitiveExpressionValues()
		{
			Assert.IsInstanceOfType(typeof(CodePrimitiveExpression), arrayCreate.Initializers[0]);
		}
		
		[Test]
		public void FirstArrayValueIsOne()
		{
			CodePrimitiveExpression primitiveExpression = arrayCreate.Initializers[0] as CodePrimitiveExpression;
			Assert.AreEqual(1, primitiveExpression.Value);
		}
		
		[Test]
		public void ArrayCapacityIsFour()
		{
			Assert.AreEqual(4, arrayCreate.Initializers.Capacity);
		}
		
		[Test]
		public void ArrayIndexerExists()
		{
			Assert.IsNotNull(arrayIndexer);
		}
		
		[Test]
		public void ArrayIndexerHasOneIndex()
		{
			Assert.AreEqual(1, arrayIndexer.Indices.Count);
		}
		
		[Test]
		public void ArrayIndexerIndexValue()
		{
			CodePrimitiveExpression primitiveExpression = arrayIndexer.Indices[0] as CodePrimitiveExpression;
			Assert.AreEqual(0, primitiveExpression.Value);
		}
		
		[Test]
		public void ArrayIndexerTargetObject()
		{
			CodeVariableReferenceExpression variableRef = arrayIndexer.TargetObject as CodeVariableReferenceExpression;
			Assert.AreEqual("i", variableRef.VariableName);
		}
	}
}
