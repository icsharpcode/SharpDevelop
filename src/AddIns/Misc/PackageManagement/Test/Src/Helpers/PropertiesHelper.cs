// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using EnvDTE;

namespace PackageManagement.Tests.Helpers
{
	public static class PropertiesHelper
	{
		public static Property FindProperty(Properties properties, string name)
		{
			return ToList(properties).Find(p => p.Name == name);
		}
		
		public static List<Property> ToList(Properties properties)
		{
			var itemsList = new List<Property>();
			foreach (Property property in properties) {
				itemsList.Add(property);
			}
			return itemsList;
		}
	}
}
