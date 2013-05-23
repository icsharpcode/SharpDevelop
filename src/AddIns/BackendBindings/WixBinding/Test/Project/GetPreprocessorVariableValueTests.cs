// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
