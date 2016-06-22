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
using System.IO;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Helpers.Tests
{
	[TestFixture]
	public class PackageReferenceRepositoryTests
	{
		FakeSharedPackageRepository sourceRepository;
		PackageReferenceRepository repository;
		FakeFileSystem fakeFileSystem;
		PackageReferenceRepositoryHelper helper;
		
		void CreatePackageReferenceRepository(string path)
		{
			helper = new PackageReferenceRepositoryHelper();
			fakeFileSystem = helper.FakeProjectSystem;
			fakeFileSystem.PathToReturnFromGetFullPath = path;
			sourceRepository = helper.FakeSharedSourceRepository;
			repository = new PackageReferenceRepository(fakeFileSystem, helper.FakeProjectSystem.ProjectName, sourceRepository);
		}
		
		[Test]
		public void RegisteredIfRequired_PackageIdInConfigFileAndInSourceRepository_MethodCalledWithPathToConfigFile()
		{
			string expectedPath = @"d:\temp";
			CreatePackageReferenceRepository(expectedPath);
			
			repository.RegisterIfNecessary();
			
			Assert.AreEqual(expectedPath, sourceRepository.PathPassedToRegisterRepository);
		}
	}
}
