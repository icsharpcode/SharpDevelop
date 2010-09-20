// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public WixExtensionProjectItem(IProject project, IProjectItemBackendStore item)
			: base(project, item)
		{
		}
	}
}
