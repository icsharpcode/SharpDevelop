// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.MSTest
{
	public static class ProjectItemExtensions
	{
		public static bool IsMSTestAssemblyReference(this ProjectItem item)
		{
			var referenceItem = item as ReferenceProjectItem;
			if (referenceItem == null)
				return false;
			
			return IsMSTestAssemblyReference(referenceItem);
		}
		
		public static bool IsMSTestAssemblyReference(this ReferenceProjectItem item)
		{
			return String.Equals(
				item.ShortName,
				"Microsoft.VisualStudio.QualityTools.UnitTestFramework",
				StringComparison.OrdinalIgnoreCase);
		}
	}
}
