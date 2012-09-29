// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;

namespace ICSharpCode.AspNet.Mvc
{
	public abstract class MvcFileGenerator
	{
		IMvcTextTemplateHostFactory hostFactory;
		IMvcTextTemplateHostAppDomainFactory appDomainFactory;
		IMvcFileGenerationErrorReporter errorReporter;
		
		public MvcFileGenerator(
			IMvcTextTemplateHostFactory hostFactory,
			IMvcTextTemplateHostAppDomainFactory appDomainFactory,
			IMvcFileGenerationErrorReporter errorReporter)
		{
			this.hostFactory = hostFactory;
			this.appDomainFactory = appDomainFactory;
			this.errorReporter = errorReporter;
		}
		
		public MvcTextTemplateLanguage TemplateLanguage { get; set; }
		public IMvcProject Project { get; set; }
		public CompilerErrorCollection Errors { get; private set; }
		
		public bool HasErrors {
			get { return Errors.Count > 0; }
		}
		
		public void GenerateFile(MvcFileName fileName)
		{
			using (IMvcTextTemplateHostAppDomain appDomain = CreateAppDomain()) {
				using (IMvcTextTemplateHost host = CreateHost(appDomain)) {
					GenerateFile(host, fileName);
				}
			}
		} 
		
		IMvcTextTemplateHostAppDomain CreateAppDomain()
		{
			return appDomainFactory.CreateAppDomain();
		}
		
		IMvcTextTemplateHost CreateHost(IMvcTextTemplateHostAppDomain appDomain)
		{
			return hostFactory.CreateMvcTextTemplateHost(Project, appDomain);
		}
		
		void GenerateFile(IMvcTextTemplateHost host, MvcFileName fileName)
		{
			ConfigureHost(host, fileName);
			string templateFileName = GetTextTemplateFileName();
			string outputViewFileName = fileName.GetPath();
			host.ProcessTemplate(templateFileName, outputViewFileName);
			
			Errors = host.Errors;
			if (HasErrors) {
				errorReporter.ShowErrors(Errors);
			}
		}
		
		protected abstract void ConfigureHost(IMvcTextTemplateHost host, MvcFileName fileName);
		protected abstract string GetTextTemplateFileName();
	}
}
