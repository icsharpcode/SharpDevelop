// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcControllerTextTemplateFileName : MvcTextTemplateFileName
	{
		public MvcControllerTextTemplateFileName(
			string textTemplatesRootFolder,
			MvcTextTemplateCriteria templateCriteria)
			: base(textTemplatesRootFolder, templateCriteria)
		{
		}
		
		protected override string GetCodeTemplatesSubFolder()
		{
			return "AddController";
		}
	}
}
