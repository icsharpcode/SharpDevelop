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

namespace WixBinding.Tests.Utils
{
	/// <summary>
	/// Helper class for WixBinding tests.
	/// </summary>
	public class WixBindingTestsHelper
	{
		WixBindingTestsHelper()
		{
		}
		
		/// <summary>
		/// Creates a WixProject that contains no WixObject or WixLibrary items.
		/// </summary>
		public static WixProject CreateEmptyWixProject()
		{
			ProjectCreateInformation info = new ProjectCreateInformation();
			info.ProjectName = "Test";
			info.OutputProjectFileName = @"C:\Projects\Test\Test.wixproj";

			return new WixProject(info);
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
	}
}
