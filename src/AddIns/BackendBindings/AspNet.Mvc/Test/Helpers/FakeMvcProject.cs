// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
