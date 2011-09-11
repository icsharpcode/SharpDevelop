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
		
		public MvcTextTemplateLanguage GetTemplateLanguage()
		{
			return TemplateLanguageToReturnFromGetTemplateLanguage;
		}
		
		public List<FakeMvcClass> ModelClasses = new List<FakeMvcClass>();
		
		public void AddModelClassToProject(string @namespace, string name)
		{
			var fakeClass = new FakeMvcClass(@namespace, name);
			ModelClasses.Add(fakeClass);
		}
		
		public void AddModelClassToProject(string fullyQualifiedName)
		{
			var fakeClass = new FakeMvcClass(fullyQualifiedName);
			ModelClasses.Add(fakeClass);
		}
		
		public int GetModelClassesCallCount;
		
		public IEnumerable<IMvcClass> GetModelClasses()
		{
			GetModelClassesCallCount++;
			return ModelClasses;
		}
		
		public List<MvcMasterPageFileName> MasterPageFileNames = new List<MvcMasterPageFileName>();
		
		public void AddMasterPageFile(MvcMasterPageFileName fileName)
		{
			MasterPageFileNames.Add(fileName);
		}
		
		public IEnumerable<MvcMasterPageFileName> GetAspxMasterPageFileNames()
		{
			return MasterPageFileNames;
		}
	}
}
