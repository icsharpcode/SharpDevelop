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
using NuGet;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class TestPackageHelper
	{
		public TestPackageHelper()
		{
			Package = MockRepository.GenerateStub<IPackage>();
		}
		
		public TestPackageHelper(string id, string version)
			: this()
		{
			SetId(id);
			SetVersion(version);
		}
		
		public IPackage Package { get; set; }
		
		public void SetId(string id)
		{
			Package.Stub(p => p.Id).Return(id);
		}
		
		public void SetVersion(string version)
		{
			Package.Stub(p => p.Version).Return(new SemanticVersion(version));
		}
		
		public void IsLatestVersion()
		{
			Package.Stub(p => p.IsLatestVersion).Return(true);
		}
		
		public void Listed()
		{
			Package.Stub(p => p.Listed).Return(true);
		}
	}
}
