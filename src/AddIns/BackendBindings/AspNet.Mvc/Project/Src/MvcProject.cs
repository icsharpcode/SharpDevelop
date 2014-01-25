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
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcProject : IMvcProject
	{
		IMvcModelClassLocator modelClassLocator;
		
		public MvcProject(IProject project)
			: this(project, new MvcModelClassLocator())
		{
		}
		
		public MvcProject(IProject project, IMvcModelClassLocator modelClassLocator)
		{
			this.Project = project;
			this.modelClassLocator = modelClassLocator;
		}
		
		public IProject Project { get; private set; }
		
		public bool IsVisualBasic()
		{
			return GetTemplateLanguage().IsVisualBasic();
		}
		
		public string RootNamespace {
			get { return Project.RootNamespace; }
		}
		
		public string OutputAssemblyFullPath {
			get { return Project.OutputAssemblyFullPath; }
		}
		
		public void Save()
		{
			Project.Save();
		}
		
		public MvcTextTemplateLanguage GetTemplateLanguage()
		{
			string language = Project.Language;
			return MvcTextTemplateLanguageConverter.Convert(language);
		}
		
		public IEnumerable<IMvcClass> GetModelClasses()
		{
			return modelClassLocator.GetModelClasses(this);
		}
		
		public IEnumerable<MvcProjectFile> GetAspxMasterPageFiles()
		{
			foreach (ProjectItem projectItem in Project.Items) {
				MvcProjectFile file = MvcProjectMasterPageFile.CreateMvcProjectMasterPageFile(projectItem);
				if (file != null) {
					yield return file;
				}
			}
		}
		
		public IEnumerable<MvcProjectFile> GetRazorFiles()
		{
			foreach (ProjectItem projectItem in Project.Items) {
				MvcProjectFile file = MvcProjectRazorFile.CreateMvcProjectRazorFile(projectItem);
				if (file != null) {
					yield return file;
				}
			}
		}
	}
}
