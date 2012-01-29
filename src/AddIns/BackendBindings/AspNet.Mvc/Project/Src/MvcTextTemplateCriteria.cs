// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcTextTemplateCriteria
	{
		public MvcTextTemplateCriteria()
		{
			TemplateName = String.Empty;
		}
		
		public MvcTextTemplateLanguage TemplateLanguage { get; set; }
		public string TemplateName { get; set; }
		public MvcTextTemplateType TemplateType { get; set; }
		
		public bool IsRazor {
			get { return TemplateType == MvcTextTemplateType.Razor; }
		}
		
		public bool IsAspx {
			get { return TemplateType == MvcTextTemplateType.Aspx; }
		}
		
		public bool IsVisualBasic {
			get { return TemplateLanguage.IsVisualBasic(); }
		}
		
		public bool IsCSharp {
			get { return TemplateLanguage.IsCSharp(); }
		}
	}
}
