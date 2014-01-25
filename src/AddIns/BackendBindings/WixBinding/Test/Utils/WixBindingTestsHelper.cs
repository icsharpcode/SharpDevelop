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
using System.ComponentModel;
using System.IO;
using System.Resources;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using Microsoft.Build.Evaluation;
using Rhino.Mocks;

namespace WixBinding.Tests.Utils
{
	/// <summary>
	/// Helper class for WixBinding tests.
	/// </summary>
	public class WixBindingTestsHelper
	{
		class DummyWixProject : WixProject
		{
			public DummyWixProject(ProjectCreateInformation info) : base (info)
			{
			}
			
			public override bool IsReadOnly {
				get { return false; }
			}
		}
		
		WixBindingTestsHelper()
		{
		}
		
		/// <summary>
		/// Creates a WixProject that contains no WixObject or WixLibrary items.
		/// </summary>
		public static WixProject CreateEmptyWixProject()
		{
			// Make sure the MSBuildEngine is initialised correctly.
			InitMSBuildEngine();
			
			// create the project.
			ProjectCreateInformation info = new ProjectCreateInformation(MockRepository.GenerateStub<ISolution>(), new FileName(@"C:\Projects\Test\Test.wixproj"));
			info.Solution.Stub(s => s.MSBuildProjectCollection).Return(new ProjectCollection());
			info.ProjectName = "Test";

			return new DummyWixProject(info);
		}
		
		/// <summary>
		/// Returns the EditorAttribute in the AttributeCollection.
		/// </summary>
		public static EditorAttribute GetEditorAttribute(AttributeCollection attributes)
		{
			foreach (Attribute attribute in attributes) {
				EditorAttribute editorAttribute = attribute as EditorAttribute;
				if (editorAttribute != null) {
					return editorAttribute;
				}
			}
			return null;
		}
		
		/// <summary>
		/// The MSBuildEngine sets the WixTargetsPath so if
		/// the WixBinding.Tests assembly is shadow copied it refers
		/// to the shadow copied assembly not the original. This
		/// causes problems for wix projects that refer to the
		/// wix.targets import via $(WixTargetsPath) so here
		/// we change it so it points to where the test assembly is built.
		/// </summary>
		public static void InitMSBuildEngine()
		{
			// Set the WixTargetsPath property so it points to
			// the actual bin path where SharpDevelop was built not
			// to the shadow copy folder.
			string codeBase = typeof(WixBindingTestsHelper).Assembly.CodeBase.Replace("file:///", String.Empty);
			string folder = Path.GetDirectoryName(codeBase);
			string fileName = Path.Combine(folder, "wix2010.targets");
			SD.Services.RemoveService(typeof(IMSBuildEngine));
			SD.Services.AddService(typeof(IMSBuildEngine), MockRepository.GenerateStrictMock<IMSBuildEngine>());
			var globalBuildProperties = new Dictionary<string, string> { { "WixTargetsPath", fileName } };
			SD.MSBuildEngine.Stub(e => e.GlobalBuildProperties).Return(globalBuildProperties);
		}
		
		/// <summary>
		/// Registers embedded strings with SharpDevelop's resource Manager 
		/// </summary>
		public static void RegisterResourceStringsWithSharpDevelopResourceManager()
		{
			ResourceServiceHelper.InitializeForUnitTests();
			ResourceManager rm = new ResourceManager("WixBinding.Tests.Strings", typeof(WixBindingTestsHelper).Assembly);
			SD.ResourceService.RegisterNeutralStrings(rm);
		}
	}
}
