// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
