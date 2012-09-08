// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		ITestFrameworkFactory factory;
		ITestFramework testFramework;
		List<string> supportedProjectFileExtensions = new List<string>();
		
		public TestFrameworkDescriptor(Properties properties, ITestFrameworkFactory factory)
		{
			this.properties = properties;
			this.factory = factory;
			
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
				testFramework = factory.Create(ClassName);
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
