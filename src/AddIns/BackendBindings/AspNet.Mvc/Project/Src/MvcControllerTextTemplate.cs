// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcControllerTextTemplate
	{
		public MvcControllerTextTemplate()
			: this(String.Empty, String.Empty, String.Empty)
		{
		}

		public MvcControllerTextTemplate(
			string name,
			string description,
			string fileName)
		{
			this.Name = name;
			this.Description = description;
			this.FileName = fileName;
		}
		
		public string Name { get; set; }
		public string Description { get; set; }
		public string FileName { get; set; }
		public bool AddActionMethods { get; set; }
	}
}
