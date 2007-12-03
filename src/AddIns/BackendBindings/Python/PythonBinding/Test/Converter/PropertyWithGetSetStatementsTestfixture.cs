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
	/// Tests the CSharpToPythonConverter class can convert a C# property to
	/// two get and set methods in Python.
	/// </summary>
	[TestFixture]
	public class PropertyWithGetSetStatementsTestFixture
	{
		CodeMemberProperty property;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tint count = 0;\r\n" +
						"\tint i = 0;\r\n" +
						"\tpublic int Count\r\n" +
						"\t{\r\n" +
						"\t\tget {\r\n" +
						"\t\t\tif (i == 0) {\r\n" +
						"\t\t\treturn 10;\r\n" +
						"\t\t} else {\r\n" +
						"\t\t\treturn count;\r\n" +
						"\t\t}\r\n" +
						"\t}\r\n" +
						"\t\tset {\r\n" +
						"\t\t\tif (i == 1) {\r\n" +
						"\t\t\tcount = value;\r\n" +
						"\t\t} else {\r\n" +
						"\t\t\tcount = value + 5;\r\n" +
						"\t\t}\r\n" +
						"\t}\r\n" +
						"}";

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			CodeCompileUnit codeCompileUnit = converter.ConvertToCodeCompileUnit(csharp);
			if (codeCompileUnit.Namespaces.Count > 0) {
				CodeNamespace codeNamespace = codeCompileUnit.Namespaces[0];
				if (codeNamespace.Types.Count > 0) {
					CodeTypeDeclaration codeTypeDeclaration = codeNamespace.Types[0];
					foreach (CodeTypeMember member in codeTypeDeclaration.Members) {
						if (member is CodeMemberProperty) {
							property = (CodeMemberProperty)member;
							break;
						}
					}
				}
			}
		}
			
		[Test]
		public void ConvertedPythonCode()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tself._count = 0\r\n" +
									"\t\tself._i = 0\r\n" +
									"\t\r\n" +
									"\tdef get_Count(self):\r\n" +
									"\t\tif self._i == 0:\r\n" +
									"\t\t\treturn 10\r\n" +
									"\t\telse:\r\n" +
									"\t\t\treturn self._count\r\n" +
									"\t\r\n" +
									"\tdef set_Count(self, value):\r\n" +
									"\t\tif self._i == 1:\r\n" +
									"\t\t\tself._count = value\r\n" +
									"\t\telse:\r\n" +
									"\t\t\tself._count = value + 5\r\n" +
									"\t\r\n" +
									"\tCount = property(fget=get_Count,fset=set_Count)";
			
			Assert.AreEqual(expectedPython, python);
		}		

		[Test]
		public void PropertyExists()
		{
			Assert.IsNotNull(property);
		}
		
		[Test]
		public void PropertyName()
		{
			Assert.AreEqual("Count", property.Name);
		}
		
		[Test]
		public void PropertyIsPublic()
		{
			Assert.AreEqual(MemberAttributes.Public, property.Attributes);
		}		
		
		[Test]
		public void PropertyHasGetStatements()
		{
			Assert.IsTrue(property.GetStatements.Count > 0);
		}
		
		[Test]
		public void FirstGetStatementIsIfTest()
		{
			Assert.IsInstanceOfType(typeof(CodeConditionStatement), property.GetStatements[0]);
		}

		[Test]
		public void FirstSetStatementIsIfTest()
		{
			Assert.IsInstanceOfType(typeof(CodeConditionStatement), property.SetStatements[0]);
		}
		
		[Test]
		public void PropertyHasSetStatements()
		{
			Assert.IsTrue(property.SetStatements.Count > 0);
		}		
		
		[Test]
		public void PropertyGetHasUserDataCalledHasAccepts()
		{
			Assert.IsTrue(property.UserData.Contains("HasAccepts"));			
		}
		
		[Test]
		public void PropertyGetUserDataHasAcceptsIsFalse()
		{
			Assert.IsFalse((bool)property.UserData["HasAccepts"]);
		}		
		
		[Test]
		public void PropertyGetHasUserDataCalledHasReturns()
		{
			Assert.IsTrue(property.UserData.Contains("HasReturns"));			
		}
		
		[Test]
		public void PropertyGetUserDataHasReturnsIsFalse()
		{
			Assert.IsFalse((bool)property.UserData["HasReturns"]);
		}				
	}
}
