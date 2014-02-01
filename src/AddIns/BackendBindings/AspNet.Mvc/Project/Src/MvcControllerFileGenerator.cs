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
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcControllerFileGenerator : MvcFileGenerator, IMvcControllerFileGenerator
	{
		public MvcControllerFileGenerator()
			: this(
				new MvcTextTemplateHostFactory(),
				new MvcTextTemplateHostAppDomainFactory(),
				new MvcFileGenerationErrorReporter())
		{
		}
		
		public MvcControllerFileGenerator(
			IMvcTextTemplateHostFactory hostFactory,
			IMvcTextTemplateHostAppDomainFactory appDomainFactory,
			IMvcFileGenerationErrorReporter errorReporter)
			: base(hostFactory, appDomainFactory, errorReporter)
		{
			this.Template = new MvcControllerTextTemplate();
		}
		
		public MvcControllerTextTemplate Template { get; set; }
		
		public void GenerateFile(MvcControllerFileName fileName)
		{
			base.GenerateFile(fileName);
		}
		
		protected override void ConfigureHost(IMvcTextTemplateHost host, MvcFileName fileName)
		{
			var controllerFileName = fileName as MvcControllerFileName;
			host.ControllerName = controllerFileName.ControllerName;
			host.AddActionMethods = Template.AddActionMethods;
			host.Namespace = GetNamespace();
		}
		
		string GetNamespace()
		{
			return Project.RootNamespace + ".Controllers";
		}
		
		protected override string GetTextTemplateFileName()
		{
			return Template.FileName;
		}
	}
}
