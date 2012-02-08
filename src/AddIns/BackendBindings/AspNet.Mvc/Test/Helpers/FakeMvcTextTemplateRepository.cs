// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcTextTemplateRepository : IMvcTextTemplateRepository
	{
		public List<MvcControllerTextTemplate> ControllerTextTemplates = new List<MvcControllerTextTemplate>();
		public MvcTextTemplateCriteria TemplateCriteriaPassedToGetMvcControllerTextTemplates;
		
		public IEnumerable<MvcControllerTextTemplate> GetMvcControllerTextTemplates(MvcTextTemplateCriteria templateCriteria)
		{
			TemplateCriteriaPassedToGetMvcControllerTextTemplates = templateCriteria;
			return ControllerTextTemplates;
		}
		
		public List<MvcViewTextTemplate> ViewTextTemplates = new List<MvcViewTextTemplate>();
		public MvcTextTemplateCriteria TemplateCriteriaPassedToGetMvcViewTextTemplates;
		
		public IEnumerable<MvcViewTextTemplate> GetMvcViewTextTemplates(MvcTextTemplateCriteria templateCriteria)
		{
			TemplateCriteriaPassedToGetMvcViewTextTemplates = templateCriteria;
			return ViewTextTemplates;
		}
	}
}
