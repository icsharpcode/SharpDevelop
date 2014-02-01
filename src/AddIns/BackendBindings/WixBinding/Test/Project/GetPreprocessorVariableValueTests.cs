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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests the WixProject.GetVariable.
	/// </summary>
	[TestFixture]
	public class GetPreprocessorVariableValueTests
	{
		[TestFixtureSetUp]
		public void SetUp()
		{
			SD.InitializeForUnitTests();
		}
		
		[Test]
		public void MissingVariableName()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			Assert.AreEqual(String.Empty, p.GetPreprocessorVariableValue("Missing"));
		}
		
		[Test]
		public void VariableNameExists()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			p.SetProperty("DefineConstants", "DATADIR=Bitmaps");
			Assert.AreEqual("Bitmaps", p.GetPreprocessorVariableValue("DATADIR"));
		}
		
		[Test]
		public void TwoDefinedVariableNames()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			p.SetProperty("DefineConstants", "TEST=test;DATADIR=Bitmaps");
			Assert.AreEqual("Bitmaps", p.GetPreprocessorVariableValue("DATADIR"));
		}
		
		[Test]
		public void VariableValueWithSpaces()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			p.SetProperty("DefineConstants", " DATADIR = Bitmaps ");
			Assert.AreEqual("Bitmaps", p.GetPreprocessorVariableValue("DATADIR"));
		}
		
		/// <summary>
		/// Tests that SharpDevelop constants (e.g. ${SharpDevelopBinPath}) 
		/// are expanded if the preprocessor variable uses one.
		/// </summary>
		[Test]
		public void VariableValueUsingSharpDevelopConstant()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			// SD.MSBuildEngine.GlobalBuildProperties is stubbed by  
			((Dictionary<string, string>)SD.MSBuildEngine.GlobalBuildProperties).Add("MyAppBinPath", @"C:\Program Files\MyApp\bin");
			p.SetProperty("DefineConstants", @" DATADIR = $(MyAppBinPath)\Bitmaps ");
			string variableValue = p.GetPreprocessorVariableValue("DATADIR");
			Assert.AreEqual(@"C:\Program Files\MyApp\bin\Bitmaps", variableValue);
		}
	}
}
