// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.AspNet.Mvc
{
	public interface IMvcTextTemplateRepository
	{
		IEnumerable<MvcControllerTextTemplate> GetMvcControllerTextTemplates(MvcTextTemplateCriteria templateCriteria);
		IEnumerable<MvcViewTextTemplate> GetMvcViewTextTemplates(MvcTextTemplateCriteria templateCriteria);
	}
}
