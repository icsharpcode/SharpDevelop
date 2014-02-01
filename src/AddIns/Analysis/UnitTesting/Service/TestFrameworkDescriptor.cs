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
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class TestFrameworkDescriptor
	{
		Properties properties;
		Func<string, object> objectFactory;
		ITestFramework testFramework;
		List<string> supportedProjectFileExtensions = new List<string>();
		
		public TestFrameworkDescriptor(Properties properties, Func<string, object> objectFactory)
		{
			this.properties = properties;
			this.objectFactory = objectFactory;
			
			GetSupportedProjectFileExtensions();
		}
		
		void GetSupportedProjectFileExtensions()
		{
			string extensions = properties["supportedProjects"];
			
			foreach (string extension in extensions.Split(';')) {
				supportedProjectFileExtensions.Add(extension.ToLowerInvariant().Trim());
			}
		}
		
		public string Id {
			get { return properties["id"]; }
		}
		
		public ITestFramework TestFramework {
			get {
				CreateTestFrameworkIfNotCreated();
				return testFramework;
			}
		}
		
		void CreateTestFrameworkIfNotCreated()
		{
			if (testFramework == null) {
				testFramework = (ITestFramework)objectFactory(ClassName);
			}
		}
		
		string ClassName {
			get { return properties["class"]; }
		}
		
		public bool IsSupportedProject(IProject project)
		{
			if (IsSupportedProjectFileExtension(project)) {
				return IsSupportedByTestFramework(project);
			}
			return false;
		}
		
		bool IsSupportedProjectFileExtension(IProject project)
		{
			string extension = GetProjectFileExtension(project);
			return IsSupportedProjectFileExtension(extension);
		}
		
		string GetProjectFileExtension(IProject project)
		{
			if (project != null) {
				return Path.GetExtension(project.FileName).ToLowerInvariant();
			}
			return null;
		}
		
		bool IsSupportedProjectFileExtension(string extension)
		{
			return supportedProjectFileExtensions.Contains(extension);
		}
		
		bool IsSupportedByTestFramework(IProject project)
		{
			return TestFramework.IsTestProject(project);
		}
	}		
}
