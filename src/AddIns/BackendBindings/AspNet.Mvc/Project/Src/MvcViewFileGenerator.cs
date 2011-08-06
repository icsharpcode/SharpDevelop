// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcViewFileGenerator : IMvcViewFileGenerator
	{
		IMvcTextTemplateHostFactory hostFactory;
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
		{
			this.hostFactory = hostFactory;
			this.textTemplateRepository = textTemplateRepository;
		}
		
		public MvcTextTemplateLanguage Language { get; set; }
		public IProject Project { get; set; }
		
		public void GenerateView(MvcViewFileName fileName)
		{
			using (IMvcTextTemplateHost host = CreateHost()) {
				GenerateView(host, fileName);
			}
		} 
		
		IMvcTextTemplateHost CreateHost()
		{
			return hostFactory.CreateMvcTextTemplateHost(Project);
		}
		
		void GenerateView(IMvcTextTemplateHost host, MvcViewFileName fileName)
		{
			ConfigureHost(host, fileName);
			string templateFileName = GetTextTemplateFileName();
			string outputViewFileName = fileName.GetPath();
			host.ProcessTemplate(templateFileName, outputViewFileName);
		}
		
		void ConfigureHost(IMvcTextTemplateHost host, MvcViewFileName fileName)
		{
			host.ViewName = fileName.ViewName;
		}
		
		string GetTextTemplateFileName()
		{
			return textTemplateRepository.GetMvcViewTextTemplateFileName(Language, "Empty");
		}
	}
}
