// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class SolutionSectionItem
	{
		SolutionSection section;
		string value;
		
		public SolutionSectionItem(SolutionSection section, string name)
			: this(section, name, section[name])
		{
		}
		
		SolutionSectionItem(SolutionSection section, string name, string value)
		{
			this.section = section;
			this.Name = name;
			this.value = value;
		}
		
		public SolutionSectionItem(string name, string value)
			: this(null, name, value)
		{
		}
		
		public string Name { get; set; }
		
		public string Value {
			get { return this.value; }
			set {
				this.value = value;
				if (section != null) {
					section.Remove(Name);
					section.Add(Name, value);
				}
			}
		}
	}
}
