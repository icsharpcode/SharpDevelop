// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class MSBuildProjectPropertiesMergeResult
	{
		List<string> propertiesAdded = new List<string>();
		List<string> propertiesUpdated = new List<string>();
		
		public IEnumerable<string> PropertiesAdded {
			get { return propertiesAdded; }
		}
		
		public IEnumerable<string> PropertiesUpdated {
			get { return propertiesUpdated; }
		}
		
		public override string ToString()
		{
			return String.Format(
				"Properties added: {0}\r\nProperties updated: {1}",
				PropertiesToString(propertiesAdded),
				PropertiesToString(propertiesUpdated));
		}
		
		static string PropertiesToString(IEnumerable<string> properties)
		{
			if (!properties.Any()) {
				return String.Empty;
			}
			
			return String.Join(",\r\n", properties.Select(property => property));
		}
		
		public void AddPropertyAdded(string propertyName)
		{
			propertiesAdded.Add(propertyName);
		}
		
		public void AddPropertyUpdated(string propertyName)
		{
			propertiesUpdated.Add(propertyName);
		}
		
		public bool AnyPropertiesChanged()
		{
			return propertiesUpdated.Any() || propertiesAdded.Any();
		}
	}
}
