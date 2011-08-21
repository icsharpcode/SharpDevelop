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
			ModelClassName = String.Empty;
			MasterPageFile = String.Empty;
		}
		
		public MvcTextTemplateType TemplateType { get; set; }
		public string ModelClassName { get; set; }
		public bool IsContentPage { get; set; }
		public string MasterPageFile { get; set; }
		
		public void GenerateFile(MvcViewFileName fileName)
		{
			base.GenerateFile(fileName);
		}
		
		protected override void ConfigureHost(IMvcTextTemplateHost host, MvcFileName fileName)
		{
			host.IsContentPage = IsContentPage;
			
			var viewFileName = fileName as MvcViewFileName;
			host.IsPartialView = viewFileName.IsPartialView;
			host.MasterPageFile = MasterPageFile;
			host.ViewDataTypeName = ModelClassName;
			host.ViewName = viewFileName.ViewName;
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
