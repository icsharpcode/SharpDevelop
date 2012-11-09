// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class SolutionPropertyFactory : IPropertyFactory
	{
		Solution solution;
		
		public SolutionPropertyFactory(Solution solution)
		{
			this.solution = solution;
		}
		
		public Property CreateProperty(string name)
		{
			return new SolutionProperty(solution, name);
		}
		
		public IEnumerator<Property> GetEnumerator()
		{
			List<Property> properties = GetProperties().ToList();
			return properties.GetEnumerator();
		}
		
		IEnumerable<Property> GetProperties()
		{
			foreach (string propertyName in solution.GetAllPropertyNames()) {
				yield return CreateProperty(propertyName);
			}
		}
	}
}
