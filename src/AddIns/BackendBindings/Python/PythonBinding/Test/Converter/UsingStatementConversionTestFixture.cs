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
	/// Tests the using statement is converted into a Python import.
	/// Note that a statement such as "from System import *" will never
	/// be generated since the code dom does not support this. If the 
	/// following code is run through the PythonProvider to create
	/// a CodeCompileUnit then only one CodeNamespaceImport is generated
	/// for the System namespace:
	/// 
	/// import System
	/// from System import *
	/// 
	/// class Foo: pass
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class UsingStatementConversionTestFixture
	{
		CSharpToPythonConverter converter;
		CodeCompileUnit codeCompileUnit;
		CodeNamespace codeNamespace;
		CodeNamespaceImport import;
		
		string csharp = "using System\r\n" +
						"class Foo\r\n" +
						"{\r\n" +
						"}";

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			converter = new CSharpToPythonConverter();
			codeCompileUnit = converter.ConvertToCodeCompileUnit(csharp);
			if (codeCompileUnit.Namespaces.Count > 0) {
				codeNamespace = codeCompileUnit.Namespaces[0];
				if (codeNamespace.Imports.Count > 0) {
					import = codeNamespace.Imports[0];
				}
			}
		}
			
		[Test]
		public void GeneratedPythonCode()
		{
			string python = converter.Convert(csharp);
			string expectedPython = "import System\r\n" +
									"\r\n" +
									"class Foo(object): pass";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void OneImport()
		{
			Assert.AreEqual(1, codeNamespace.Imports.Count);
		}
		
		[Test]
		public void OneClassInRootNamespace()
		{
			Assert.AreEqual(1, codeNamespace.Types.Count);
		}		
		
		[Test]
		public void ImportNamespace()
		{
			Assert.AreEqual("System", import.Namespace);
		}
	}
}
