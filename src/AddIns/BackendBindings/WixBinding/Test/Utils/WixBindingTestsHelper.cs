// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.IO;
using System.Resources;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;

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
			
			public override bool ReadOnly {
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
			ProjectCreateInformation info = new ProjectCreateInformation();
			info.Solution = new Solution(new MockProjectChangeWatcher());
			info.ProjectName = "Test";
			info.OutputProjectFileName = @"C:\Projects\Test\Test.wixproj";

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
			MSBuildEngine.MSBuildProperties["WixTargetsPath"] = fileName;
		}
		
		/// <summary>
		/// Registers embedded strings with SharpDevelop's resource Manager 
		/// </summary>
		public static void RegisterResourceStringsWithSharpDevelopResourceManager()
		{
			ResourceManager rm = new ResourceManager("WixBinding.Tests.Strings", typeof(WixBindingTestsHelper).Assembly);
			ResourceService.RegisterNeutralStrings(rm);
		}
	}
}
