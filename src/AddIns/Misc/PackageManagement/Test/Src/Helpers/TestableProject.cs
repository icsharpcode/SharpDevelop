// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.Helpers
{
	public class TestableProject : MSBuildBasedProject
	{
		public bool IsSaved;
		public ItemType ItemTypeToReturnFromGetDefaultItemType = ItemType.Compile;
		public ReadOnlyCollection<ProjectItem> ItemsWhenSaved;
		
		public TestableProject(ProjectCreateInformation createInfo)
			: base(createInfo)
		{
		}
		
		public override void Save(string fileName)
		{
			IsSaved = true;
			ItemsWhenSaved = Items;
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			return ItemTypeToReturnFromGetDefaultItemType;
		}
	}
}
