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
using ICSharpCode.PackageManagement;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakeSelectedProject : IPackageManagementSelectedProject
	{
		public FakePackageManagementProject FakeProject = new FakePackageManagementProject();
		
		public FakeSelectedProject()
		{
		}
		
		public FakeSelectedProject(string name)
			: this(name, false)
		{
		}
		
		public FakeSelectedProject(string name, bool selected)
			: this(name, selected, true)
		{
		}
		
		public FakeSelectedProject(string name, bool selected, bool enabled)
		{
			this.Name = name;
			this.IsSelected = selected;
			this.IsEnabled = enabled;
		}
		
		public string Name { get; set; }
		public bool IsSelected { get; set; }
		public bool IsEnabled { get; set; }
		
		public override string ToString()
		{
			return String.Format("Name: {0}, IsSelected: {1}, IsEnabled: {2}", Name, IsSelected, IsEnabled);
		}
		
		public IPackageManagementProject Project {
			get { return FakeProject; }
		}
		
		public FakePackageOperation AddFakeInstallPackageOperation()
		{
			return FakeProject.AddFakeInstallOperation();
		}
		
		public FakePackageOperation AddFakeUninstallPackageOperation()
		{
			return FakeProject.AddFakeUninstallOperation();
		}
	}
}
