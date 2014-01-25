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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ICSharpCode.Core;
using ICSharpCode.Core.Tests.Utils;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.Core.Tests.AddInTreeTests
{
	[TestFixture]
	public class RuntimeLoadAssemblyTests
	{
		[Test]
		public void AssemblyLoadCalledWhenAssemblyNameStartsWithColon()
		{
			IAddInTree addInTree = MockRepository.GenerateStrictMock<IAddInTree>();
			DerivedRuntime runtime = new DerivedRuntime(addInTree, ":ICSharpCode.SharpDevelop", String.Empty);
			runtime.Load();
			Assert.AreEqual("ICSharpCode.SharpDevelop", runtime.AssemblyNamePassedToLoadAssembly);
		}
		
		[Test]
		public void RuntimeLoadedAssemblyMatchesAssemblyReturnedFromLoadAssemblyMethod()
		{
			IAddInTree addInTree = MockRepository.GenerateStrictMock<IAddInTree>();
			DerivedRuntime runtime = new DerivedRuntime(addInTree, ":ICSharpCode.SharpDevelop", String.Empty);
			runtime.AssemblyNames.Add("ICSharpCode.SharpDevelop", typeof(string).Assembly);
			Assembly loadedAssembly = runtime.LoadedAssembly;
			
			string expectedLoadedAssemblyName = "CommonLanguageRuntimeLibrary";
			Assert.AreEqual(expectedLoadedAssemblyName, loadedAssembly.ManifestModule.ToString());
		}
		
		[Test]
		public void AssemblyLoadCalledWhenAssemblyNameDoesNotStartWithColonOrDollar()
		{
			IAddInTree addInTree = MockRepository.GenerateStrictMock<IAddInTree>();
			string hintPath = @"D:\SharpDevelop\AddIns\FormsDesigner";
			DerivedRuntime runtime = new DerivedRuntime(addInTree, "FormsDesigner.dll", hintPath);
			runtime.Load();
			
			string expectedFileName = @"D:\SharpDevelop\AddIns\FormsDesigner\FormsDesigner.dll";
			Assert.AreEqual(expectedFileName, runtime.AssemblyFileNamePassedToLoadAssemblyFrom);
		}
		
		[Test]
		public void RuntimeLoadedAssemblyMatchesAssemblyReturnedFromLoadAssemblyFromMethod()
		{
			IAddInTree addInTree = MockRepository.GenerateStrictMock<IAddInTree>();
			DerivedRuntime runtime = new DerivedRuntime(addInTree, "MyAddIn.dll", @"d:\projects");
			runtime.AssemblyFileNames.Add(@"MyAddIn.dll", typeof(string).Assembly);
			Assembly loadedAssembly = runtime.LoadedAssembly;
			
			string expectedLoadedAssemblyName = "CommonLanguageRuntimeLibrary";
			Assert.AreEqual(expectedLoadedAssemblyName, loadedAssembly.ManifestModule.ToString());
		}
		
		[Test]
		public void AssemblyLoadFromCalledWhenAssemblyNameStartsWithDollar()
		{
			IAddInTree addInTree = CreateAddInTreeWithFormsDesignerAddIn();
			DerivedRuntime runtime = new DerivedRuntime(addInTree, "$ICSharpCode.FormsDesigner/FormsDesigner.dll", String.Empty);
			runtime.Load();
			Assert.AreEqual(@"D:\SharpDevelop\AddIns\FormsDesigner\FormsDesigner.dll", runtime.AssemblyFileNamePassedToLoadAssemblyFrom);
		}
		
		IAddInTree CreateAddInTreeWithFormsDesignerAddIn()
		{
			IAddInTree addInTree = MockRepository.GenerateStrictMock<IAddInTree>();
			AddIn formsDesignerAddIn = AddIn.Load(addInTree, new StringReader(GetFormsDesignerAddInXml()));
			formsDesignerAddIn.FileName = @"D:\SharpDevelop\AddIns\FormsDesigner\FormsDesigner.addin";
			formsDesignerAddIn.Enabled = true;
			
			addInTree.Expect(a => a.AddIns).Return(new[] { formsDesignerAddIn });
			return addInTree;
		}
		
		string GetFormsDesignerAddInXml()
		{
			return
				"<AddIn name        = \"Forms Designer\"\r\n" +
				"       author      = \"Mike Krueger\"\r\n" +
				"       copyright   = \"prj:///doc/copyright.txt\"\r\n" +
				"       description = \"Windows Forms Designer\"\r\n" +
				"       addInManagerHidden = \"preinstalled\">\r\n" +
				"\r\n" +
				"    <Manifest>\r\n" +
				"        <Identity name = \"ICSharpCode.FormsDesigner\"/>\r\n" +
				"    </Manifest>\r\n" +
				"\r\n" +
				"    <Runtime>\r\n" +
				"        <Import assembly = \"FormsDesigner.dll\"/>\r\n" +
				"        <Import assembly = \":ICSharpCode.SharpDevelop\"/>\r\n" +
				"    </Runtime>\r\n" +
				"</AddIn>";
		}
		
		[Test]
		public void CoreExceptionErrorMessageShownAssemblyNameStartsWithDollarButHasNoForwardSlashCharacter()
		{
			IAddInTree addInTree = MockRepository.GenerateStrictMock<IAddInTree>();
			DerivedRuntime runtime = new DerivedRuntime(addInTree, "$ICSharpCode.FormsDesigner.dll", String.Empty);
			CoreException ex = Assert.Throws<CoreException>(delegate { runtime.Load(); });
			Assert.AreEqual("Expected '/' in path beginning with '$'!", ex.Message);
		}
		
		[Test]
		public void ErrorMessageShownWhenAddInReferenceCannotBeFound()
		{
			IAddInTree addInTree = MockRepository.GenerateStrictMock<IAddInTree>();
			addInTree.Expect(a => a.AddIns).Return(new AddIn[0]);
			DerivedRuntime runtime = new DerivedRuntime(addInTree, "$UnknownAddIn/Unknown.dll", String.Empty);
			runtime.Load();
			
			string expectedErrorMessageStart =
				"The addin '$UnknownAddIn/Unknown.dll' could not be loaded:\n" +
				"System.IO.FileNotFoundException: Could not find referenced AddIn UnknownAddIn";
			string errorMessage = runtime.ErrorMessageDisplayed;
			Assert.IsTrue(errorMessage.StartsWith(expectedErrorMessageStart), errorMessage);
		}
		
		[Test]
		public void ErrorMessageShownWhenLoadAssemblyFromThrowsFileLoadException()
		{
			IAddInTree addInTree = MockRepository.GenerateStrictMock<IAddInTree>();
			DerivedRuntime runtime = new DerivedRuntime(addInTree, "Missing.dll", String.Empty);
			FileLoadException ex = new FileLoadException("Test");
			runtime.LoadAssemblyFromExceptionToThrow = ex;
			runtime.Load();
			
			string expectedErrorMessageStart =
				"The addin 'Missing.dll' could not be loaded:\n" +
				"System.IO.FileLoadException: Test";
			string errorMessage = runtime.ErrorMessageDisplayed;
			Assert.IsTrue(errorMessage.StartsWith(expectedErrorMessageStart), errorMessage);
		}
	}
}
