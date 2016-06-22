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
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcProject : IMvcProject
	{
		public TestableProject FakeProject = TestableProject.CreateProject();

		public IProject Project {
			get { return FakeProject; }
		}
		
		public string RootNamespace { get; set; }
		public string OutputAssemblyFullPath { get; set; }

		public bool SaveCalled;
		
		public void Save()
		{
			SaveCalled = true;
		}
		
		MvcTextTemplateLanguage TemplateLanguageToReturnFromGetTemplateLanguage;
		
		public void SetCSharpAsTemplateLanguage()
		{
			TemplateLanguageToReturnFromGetTemplateLanguage = MvcTextTemplateLanguage.CSharp;
		}
		
		public void SetVisualBasicAsTemplateLanguage()
		{
			TemplateLanguageToReturnFromGetTemplateLanguage = MvcTextTemplateLanguage.VisualBasic;
		}
		
		public bool IsVisualBasic()
		{
			return TemplateLanguageToReturnFromGetTemplateLanguage == MvcTextTemplateLanguage.VisualBasic; 
		}
		
		public MvcTextTemplateLanguage GetTemplateLanguage()
		{
			return TemplateLanguageToReturnFromGetTemplateLanguage;
		}
		
		public List<FakeMvcClass> ModelClasses = new List<FakeMvcClass>();
		
		public FakeMvcClass AddModelClassToProject(string @namespace, string name)
		{
			var fakeClass = new FakeMvcClass(@namespace, name);
			ModelClasses.Add(fakeClass);
			return fakeClass;
		}
		
		public FakeMvcClass AddModelClassToProject(string fullyQualifiedName)
		{
			var fakeClass = new FakeMvcClass(fullyQualifiedName);
			ModelClasses.Add(fakeClass);
			return fakeClass;
		}
		
		public int GetModelClassesCallCount;
		
		public IEnumerable<IMvcClass> GetModelClasses()
		{
			GetModelClassesCallCount++;
			return ModelClasses;
		}
		
		public List<MvcProjectFile> AspxMasterPageFiles = new List<MvcProjectFile>();
		
		public void AddAspxMasterPageFile(MvcProjectFile file)
		{
			AspxMasterPageFiles.Add(file);
		}
		
		public IEnumerable<MvcProjectFile> GetAspxMasterPageFiles()
		{
			return AspxMasterPageFiles;
		}
		
		public List<MvcProjectFile> RazorFiles = new List<MvcProjectFile>();
		
		public void AddRazorFile(MvcProjectFile file)
		{
			RazorFiles.Add(file);
		}
		
		public IEnumerable<MvcProjectFile> GetRazorFiles()
		{
			return RazorFiles;
		}
	}
}
