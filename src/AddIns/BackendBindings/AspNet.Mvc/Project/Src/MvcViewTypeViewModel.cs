// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcViewEngineViewModel
	{
		public MvcViewEngineViewModel(
			string name,
			MvcTextTemplateType templateType)
		{
			this.Name = name;
			this.TemplateType = templateType;
		}
		
		public string Name { get; set; }
		public MvcTextTemplateType TemplateType { get; set; }
	}
}
