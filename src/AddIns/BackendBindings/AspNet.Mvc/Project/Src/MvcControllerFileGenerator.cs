// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcControllerFileGenerator : MvcFileGenerator, IMvcControllerFileGenerator
	{
		MvcTextTemplateRepository textTemplateRepository;
		
		public MvcControllerFileGenerator()
			: this(
				new MvcTextTemplateHostFactory(),
				new MvcTextTemplateRepository())
		{
		}
		
		public MvcControllerFileGenerator(
			IMvcTextTemplateHostFactory hostFactory,
			MvcTextTemplateRepository textTemplateRepository)
			: base(hostFactory)
		{
			this.textTemplateRepository = textTemplateRepository;
		}
		
		public void GenerateFile(MvcControllerFileName fileName)
		{
			base.GenerateFile(fileName);
		}
		
		protected override void ConfigureHost(IMvcTextTemplateHost host, MvcFileName fileName)
		{
			var controllerFileName = fileName as MvcControllerFileName;
			host.ControllerName = controllerFileName.ControllerName;
			host.Namespace = GetNamespace();
		}
		
		string GetNamespace()
		{
			return Project.RootNamespace + ".Controllers";
		}
		
		protected override string GetTextTemplateFileName()
		{
			return textTemplateRepository.GetMvcControllerTextTemplateFileName(Language, "Controller");
		}
	}
}
