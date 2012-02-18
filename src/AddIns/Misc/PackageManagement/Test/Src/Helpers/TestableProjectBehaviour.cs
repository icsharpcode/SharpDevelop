// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.Helpers
{
	public class TestableProjectBehaviour : ProjectBehavior
	{
		public string FileNamePassedToGetDefaultItemType;
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			FileNamePassedToGetDefaultItemType = fileName;
			return ItemTypeToReturnFromGetDefaultItemType;
		}
		
		public ItemType ItemTypeToReturnFromGetDefaultItemType = ItemType.Compile;
	}
}
