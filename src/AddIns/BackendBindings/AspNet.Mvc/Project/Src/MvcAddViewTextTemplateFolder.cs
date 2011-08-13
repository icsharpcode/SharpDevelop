// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcAddViewTextTemplateFolder
	{
		string name;
		
		public MvcAddViewTextTemplateFolder(MvcTextTemplateCriteria criteria)
		{
			name = GetFolderName(criteria);
		}
		
		string GetFolderName(MvcTextTemplateCriteria criteria)
		{
			var templateTypeFolder = new MvcTextTemplateTypeFolder(criteria);
			return "AddView\\" + templateTypeFolder.Name;
		}
		
		public string Name {
			get { return name; }
		}
	}
}
