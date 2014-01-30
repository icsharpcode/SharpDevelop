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
