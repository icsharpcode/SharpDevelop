// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using System;
using System.ComponentModel;
using System.IO;

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
			info.Solution = new Solution();
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
		/// The MSBuildEngine sets the SharpDevelopBinPath so if
		/// the SharpDevelop.Base assembly is shadow copied it refers
		/// to the shadow copied assembly not the original. This
		/// causes problems for wix projects that refer to the
		/// wix.targets import via $(SharpDevelopBinPath) so here
		/// we change it so it points to the real SharpDevelop 
		/// binary.
		/// </summary>
		public static void InitMSBuildEngine()
		{
			// Remove existing SharpDevelopBinPath property.
			MSBuildEngine.MSBuildProperties.Remove("SharpDevelopBinPath");

			// Set the SharpDevelopBinPath property so it points to
			// the actual bin path where SharpDevelop was built not
			// to the shadow copy folder.
			string codeBase = typeof(MSBuildEngine).Assembly.CodeBase.Replace("file:///", String.Empty);
			string folder = Path.GetDirectoryName(codeBase);
			folder = Path.GetFullPath(Path.Combine(folder, @"..\"));
			MSBuildEngine.MSBuildProperties["SharpDevelopBinPath"] = folder;
		}
	}
}
