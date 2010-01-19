// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseFromMathImportCosAndTanExitTestFixture
	{
		ICompilationUnit compilationUnit;
		PythonImport import;
		
		[SetUp]
		public void Init()
		{
			string python = "from math import cos, tan";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);
			import = compilationUnit.UsingScope.Usings[0] as PythonImport;
		}
		
		[Test]
		public void UsingAsPythonImportHasCosIdentifier()
		{
			Assert.IsTrue(import.HasIdentifier("cos"));
		}
		
		[Test]
		public void UsingAsPythonImportContainsMathModuleName()
		{
			Assert.AreEqual("math", import.Module);
		}

		[Test]
		public void UsingAsPythonImportHasTanIdentifier()
		{
			Assert.IsTrue(import.HasIdentifier("tan"));
		}

		[Test]
		public void UsingAsPythonImportDoesNotHaveACosIdentifier()
		{
			Assert.IsFalse(import.HasIdentifier("acos"));
		}
	}
}
