// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System.Collections.Generic;
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
