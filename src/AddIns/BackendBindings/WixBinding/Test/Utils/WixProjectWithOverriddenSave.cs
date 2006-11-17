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

namespace WixBinding.Tests.Utils
{
	/// <summary>
	/// Test helper class which overrides the WixProject.Save method so it does
	/// nothing. Allows us to call Save in test fixtures without actually writing
	/// anything to disk.
	/// </summary>
	public class WixProjectWithOverriddenSave : WixProject
	{
		public WixProjectWithOverriddenSave(ProjectCreateInformation info) : base(info)
		{
		}
		
		/// <summary>
		/// Save method that does nothing.
		/// </summary>
		public override void Save(string fileName)
		{
			// Do nothing.
		}
	}
}
