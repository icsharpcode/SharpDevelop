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
				new MvcTextTemplateRepository(),
				new MvcTextTemplateHostAppDomainFactory(),
				new MvcFileGenerationErrorReporter())
		{
		}
		
		public MvcViewFileGenerator(
			IMvcTextTemplateHostFactory hostFactory,
			MvcTextTemplateRepository textTemplateRepository,
			IMvcTextTemplateHostAppDomainFactory appDomainFactory,
			IMvcFileGenerationErrorReporter errorReporter)
			: base(hostFactory, appDomainFactory, errorReporter)
		{
			this.textTemplateRepository = textTemplateRepository;
			
			ModelClassName = String.Empty;
			ModelClassAssemblyLocation = String.Empty;
			MasterPageFile = String.Empty;
			PrimaryContentPlaceHolderId = String.Empty;
			Template = new MvcViewTextTemplate();
		}
		
		public string ModelClassName { get; set; }
		public string ModelClassAssemblyLocation { get; set; }
		public bool IsContentPage { get; set; }
		public string MasterPageFile { get; set; }
		public string PrimaryContentPlaceHolderId { get; set; }
		public MvcViewTextTemplate Template { get; set; }
		
		public void GenerateFile(MvcViewFileName fileName)
		{
			base.GenerateFile(fileName);
		}
		
		protected override void ConfigureHost(IMvcTextTemplateHost host, MvcFileName fileName)
		{
			host.IsContentPage = IsContentPage;
			host.MasterPageFile = MasterPageFile;
			host.PrimaryContentPlaceHolderID = PrimaryContentPlaceHolderId;
			host.ViewDataTypeName = ModelClassName;
			host.ViewDataTypeAssemblyLocation = ModelClassAssemblyLocation;
			
			var viewFileName = fileName as MvcViewFileName;
			host.IsPartialView = viewFileName.IsPartialView;
			host.ViewName = viewFileName.ViewName;
		}
		
		protected override string GetTextTemplateFileName()
		{
			return Template.FileName;
		}
	}
}
