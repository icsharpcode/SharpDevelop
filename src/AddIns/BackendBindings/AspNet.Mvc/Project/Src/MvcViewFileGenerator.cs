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
