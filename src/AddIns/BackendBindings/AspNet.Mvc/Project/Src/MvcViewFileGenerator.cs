// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcViewFileGenerator : MvcFileGenerator, IMvcViewFileGenerator
	{
		MvcTextTemplateRepository textTemplateRepository;
		
		public MvcViewFileGenerator()
			: this(
				new MvcTextTemplateHostFactory(),
				new MvcTextTemplateRepository())
		{
		}
		
		public MvcViewFileGenerator(
			IMvcTextTemplateHostFactory hostFactory,
			MvcTextTemplateRepository textTemplateRepository)
			: base(hostFactory)
		{
			this.textTemplateRepository = textTemplateRepository;
		}
		
		public MvcTextTemplateType TemplateType { get; set; }
		
		public void GenerateFile(MvcViewFileName fileName)
		{
			base.GenerateFile(fileName);
		}
		
		protected override void ConfigureHost(IMvcTextTemplateHost host, MvcFileName fileName)
		{
			var viewFileName = fileName as MvcViewFileName;
			host.ViewName = viewFileName.ViewName;
			host.IsPartialView = viewFileName.IsPartialView;
		}
		
		protected override string GetTextTemplateFileName()
		{
			MvcTextTemplateCriteria templateCriteria = GetTextTemplateCriteria();
			return textTemplateRepository.GetMvcViewTextTemplateFileName(templateCriteria);
		}
		
		MvcTextTemplateCriteria GetTextTemplateCriteria()
		{
			return new MvcTextTemplateCriteria() {
				TemplateLanguage = TemplateLanguage,
				TemplateName = "Empty",
				TemplateType = TemplateType
			};
		}
	}
}
