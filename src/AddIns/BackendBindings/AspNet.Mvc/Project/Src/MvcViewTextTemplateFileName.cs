// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcViewTextTemplateFileName : MvcTextTemplateFileName
	{
		MvcAddViewTextTemplateFolder folder;
		
		public MvcViewTextTemplateFileName(
			string textTemplatesRootDirectory,
			MvcTextTemplateCriteria templateCriteria)
			: base(textTemplatesRootDirectory, templateCriteria)
		{
			this.folder = new MvcAddViewTextTemplateFolder(templateCriteria);
		}
		
		protected override string GetCodeTemplatesSubFolder()
		{
			return folder.Name;
		}
	}
}
