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
		
		public MvcFileGenerator(
			IMvcTextTemplateHostFactory hostFactory,
			IMvcTextTemplateHostAppDomainFactory appDomainFactory)
		{
			this.hostFactory = hostFactory;
			this.appDomainFactory = appDomainFactory;
		}
		
		public MvcTextTemplateLanguage TemplateLanguage { get; set; }
		public IMvcProject Project { get; set; }
		
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
			
			if (host.Errors.Count > 0) {
				CompilerError error = host.Errors[0];
				Console.WriteLine("ProcessTemplate error: " + error.ErrorText);
				Console.WriteLine("ProcessTemplate error: Line: " + error.Line);
			}
		}
		
		protected abstract void ConfigureHost(IMvcTextTemplateHost host, MvcFileName fileName);
		protected abstract string GetTextTemplateFileName();
	}
}
