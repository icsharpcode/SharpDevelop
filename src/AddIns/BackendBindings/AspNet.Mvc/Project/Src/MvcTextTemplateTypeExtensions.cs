// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public static class MvcTextTemplateTypeExtensions
	{
		public static bool IsRazor(this MvcTextTemplateType templateType)
		{
			return templateType == MvcTextTemplateType.Razor;
		}
		
		public static bool IsAspx(this MvcTextTemplateType templateType)
		{
			return templateType == MvcTextTemplateType.Aspx;
		}
	}
}
