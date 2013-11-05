// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.CSharp;

namespace TextTemplating.Tests.Helpers
{
	public class TestableProject : MSBuildBasedProject
	{
		string language = "C#";
		string rootNamespace;
		
		public TestableProject(ProjectCreateInformation info)
			: base(info)
		{
			rootNamespace = info.ProjectName;
		}
		
		public override string Language {
			get { return language; }
		}
		
		public void SetLanguage(string language)
		{
			this.language = language;
		}
		
		public override string RootNamespace {
			get { return rootNamespace; }
			set { rootNamespace = value; }
		}
		
		public CodeDomProvider CodeDomProviderToReturn = new CSharpCodeProvider();
		
		public override CodeDomProvider CreateCodeDomProvider()
		{
			return CodeDomProviderToReturn;
		}
	}
}
