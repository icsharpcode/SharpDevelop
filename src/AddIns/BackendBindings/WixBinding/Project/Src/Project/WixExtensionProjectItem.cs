// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public static class WixItemType
	{		
		public const string LibraryName = "WixLibrary";
		public static readonly ItemType Library = new ItemType(LibraryName);

		public const string ExtensionName = "WixExtension";
		public static readonly ItemType Extension = new ItemType(ExtensionName);
		
	}
	
	/// <summary>
	/// Base class for all Wix compiler extension project items.
	/// </summary>
	public class WixExtensionProjectItem : ProjectItem
	{
		public WixExtensionProjectItem(IProject project)
			: base(project, WixItemType.Extension)
		{
		}
		
		public WixExtensionProjectItem(IProject project, Microsoft.Build.BuildEngine.BuildItem item)
			: base(project, item)
		{
		}
	}
}
