// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using NUnit.Framework;

namespace PackageManagement.Tests.Helpers
{
	public static class SelectedProjectCollectionAssert
	{
		public static void AreEqual(
			IEnumerable<IPackageManagementSelectedProject> expected,
			IEnumerable<IPackageManagementSelectedProject> actual)
		{
			List<string> expectedAsStrings = ConvertToStrings(expected);
			List<string> actualAsStrings = ConvertToStrings(actual);
			CollectionAssert.AreEqual(expectedAsStrings, actualAsStrings);
		}
		
		static List<string> ConvertToStrings(IEnumerable<IPackageManagementSelectedProject> projects)
		{
			var projectsAsString = new List<string>();
			foreach (IPackageManagementSelectedProject project in projects) {
				string text = String.Format("Name: {0}, IsSelected: {1}, IsEnabled: {2}",
					project.Name,
					project.IsSelected,
					project.IsEnabled);
				projectsAsString.Add(text);
			}
			return projectsAsString;
		}
	}
}
