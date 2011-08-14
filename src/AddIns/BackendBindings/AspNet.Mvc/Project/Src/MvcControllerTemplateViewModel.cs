// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcControllerTemplateViewModel
	{
		public MvcControllerTemplateViewModel()
			: this(String.Empty, String.Empty)
		{
		}

		public MvcControllerTemplateViewModel(
			string name,
			string description)
		{
			this.Name = name;
			this.Description = description;
		}
		
		public string Name { get; set; }
		public string Description { get; set; }
		public bool AddActionMethods { get; set; }
	}
}
