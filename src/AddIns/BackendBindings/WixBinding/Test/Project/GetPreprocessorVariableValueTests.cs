// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests the WixProject.GetVariable.
	/// </summary>
	[TestFixture]
	public class GetPreprocessorVariableValueTests
	{
		[Test]
		public void MissingVariableName()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			Assert.AreEqual(String.Empty, p.GetVariable("Missing"));
		}
		
		[Test]
		public void VariableNameExists()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			p.SetProperty("DefineConstants", "DATADIR=Bitmaps");
			Assert.AreEqual("Bitmaps", p.GetVariable("DATADIR"));
		}
		
		[Test]
		public void TwoDefinedVariableNames()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			p.SetProperty("DefineConstants", "TEST=test;DATADIR=Bitmaps");
			Assert.AreEqual("Bitmaps", p.GetVariable("DATADIR"));
		}
		
		[Test]
		public void VariableValueWithSpaces()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			p.SetProperty("DefineConstants", " DATADIR = Bitmaps ");
			Assert.AreEqual("Bitmaps", p.GetVariable("DATADIR"));
		}
		
		/// <summary>
		/// Tests that SharpDevelop constants (e.g. ${SharpDevelopBinPath}) 
		/// are expanded if the preprocessor variable uses one.
		/// </summary>
		[Test]
		public void VariableValueUsingSharpDevelopConstant()
		{
			MSBuildEngine.MSBuildProperties.Add("MyAppBinPath", @"C:\Program Files\MyApp\bin");
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			p.SetProperty("DefineConstants", @" DATADIR = $(MyAppBinPath)\Bitmaps ");
			string variableValue = p.GetVariable("DATADIR");
			MSBuildEngine.MSBuildProperties.Remove("MyAppBinPath");
			Assert.AreEqual(@"C:\Program Files\MyApp\bin\Bitmaps", variableValue);
		}
	}
}
