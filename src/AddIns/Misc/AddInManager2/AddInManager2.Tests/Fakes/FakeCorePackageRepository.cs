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
using System.Linq;
using NuGet;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
	public class FakeCorePackageRepository : IPackageRepository
	{
		public FakeCorePackageRepository()
		{
		}
		
		public string Source
		{
			get;
			set;
		}
		
		public bool SupportsPrereleasePackages
		{
			get;
			set;
		}
		
		public IQueryable<IPackage> ReturnedPackages
		{
			get;
			set;
		}
		
		public System.Linq.IQueryable<IPackage> GetPackages()
		{
			return ReturnedPackages;
		}
		
		public void AddPackage(IPackage package)
		{
			if (AddPackageCallback != null)
			{
				AddPackageCallback(package);
			}
		}
		
		public Action<IPackage> AddPackageCallback
		{
			get;
			set;
		}
		
		public void RemovePackage(IPackage package)
		{
			if (RemovePackageCallback != null)
			{
				RemovePackageCallback(package);
			}
		}
		
		public Action<IPackage> RemovePackageCallback
		{
			get;
			set;
		}

		public PackageSaveModes PackageSaveMode { get; set; }
	}
}
