// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
