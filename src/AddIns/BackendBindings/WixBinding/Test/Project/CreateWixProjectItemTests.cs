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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests the WixProject.CreateProjectItem method.
	/// </summary>
	[TestFixture]
	public class CreateWixProjectItemTests
	{
		class BuildItem : IProjectItemBackendStore
		{
			public BuildItem(string type, string include)
			{
				this.ItemType = new ItemType(type);
				this.UnevaluatedInclude = include;
				this.EvaluatedInclude = include;
			}
			
			/// Gets the owning project.
			public IProject Project { get { throw new NotImplementedException(); } }
			
			public string UnevaluatedInclude { get; set; }
			public string EvaluatedInclude { get; set; }
			public ItemType ItemType { get; set; }
			
			public string GetEvaluatedMetadata(string name)
			{
				throw new NotImplementedException();
			}
			public string GetMetadata(string name)
			{
				throw new NotImplementedException();
			}
			public bool HasMetadata(string name)
			{
				throw new NotImplementedException();
			}
			public void RemoveMetadata(string name)
			{
				throw new NotImplementedException();
			}
			public void SetEvaluatedMetadata(string name, string value)
			{
				throw new NotImplementedException();
			}
			public void SetMetadata(string name, string value)
			{
				throw new NotImplementedException();
			}
			public IEnumerable<string> MetadataNames {
				get {
					throw new NotImplementedException();
				}
			}
		}
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			SD.InitializeForUnitTests();
		}
		
		[Test]
		public void CreateReferenceProjectItem()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			ProjectItem item = p.CreateProjectItem(new BuildItem("Reference", "DummyInclude"));
			Assert.IsInstanceOf(typeof(ReferenceProjectItem), item);
		}
		
		[Test]
		public void CreateWixLibraryProjectItem()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			ProjectItem item = p.CreateProjectItem(new BuildItem("WixLibrary", "DummyInclude"));
			Assert.IsInstanceOf(typeof(WixLibraryProjectItem), item);
		}
		
		[Test]
		public void CreateWixExtensionProjectItem()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			ProjectItem item = p.CreateProjectItem(new BuildItem("WixExtension", "DummyInclude"));
			Assert.IsInstanceOf(typeof(WixExtensionProjectItem), item);
		}
	}
}
