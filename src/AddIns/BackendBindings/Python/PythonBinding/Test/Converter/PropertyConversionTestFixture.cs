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
	public class PropertyConversionTestFixture
	{
		CodeMemberProperty property;
		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tint count = 0;\r\n" +
						"\tpublic int Count\r\n" +
						"\t{\r\n" +
						"\t\tget { return count; }\r\n" +
						"\t\tset { count = value; }\r\n" +
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
									"\t\r\n" +
									"\tdef get_Count(self):\r\n" +
									"\t\treturn self._count\r\n" +
									"\t\r\n" +
									"\tdef set_Count(self, value):\r\n" +
									"\t\tself._count = value\r\n" +
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
	}
}
