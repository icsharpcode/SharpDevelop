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
using ICSharpCode.AddInManager2.Model;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
	public class FakeAddInManagerServices : IAddInManagerServices
	{
		public FakeAddInManagerServices()
		{
		}
		
		public FakePackageRepositories FakeRepositories
		{
			get;
			set;
		}
		
		public FakeAddInSetup FakeSetup 
		{
			get;
			set;
		}
		
		public FakeNuGetPackageManager FakeNuGet
		{
			get;
			set;
		}
		
		public FakeAddInManagerSettings FakeSettings
		{
			get;
			set;
		}
		
		public FakeSDAddInManagement FakeSDAddInManagement
		{
			get;
			set;
		}
		
		public IAddInManagerEvents Events
		{
			get;
			set;
		}
		
		public IPackageRepositories Repositories
		{
			get
			{
				return FakeRepositories;
			}
		}
		
		public IAddInSetup Setup 
		{
			get
			{
				return FakeSetup;
			}
		}
		
		public INuGetPackageManager NuGet
		{
			get
			{
				return FakeNuGet;
			}
		}
		
		public IAddInManagerSettings Settings
		{
			get
			{
				return FakeSettings;
			}
		}
		
		public ISDAddInManagement SDAddInManagement
		{
			get
			{
				return FakeSDAddInManagement;
			}
		}
	}
}
