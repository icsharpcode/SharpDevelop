// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests the GetResourceReader method of the PythonDesignerLoader.
	/// </summary>
	[TestFixture]
	public class GetResourcesFromDesignerLoaderTestFixture
	{
		PythonDesignerLoader loader;
		MockDesignerLoaderHost host;
		MockResourceReader reader;
		MockResourceService resourceService;
		MockResourceReader dummyReader;
		MockResourceWriter dummyWriter;
		MockResourceWriter writer;

		[SetUp]
		public void Init()
		{
			dummyReader = new MockResourceReader();
			dummyWriter = new MockResourceWriter();
			resourceService = new MockResourceService();
			resourceService.SetResourceReader(dummyReader);
			resourceService.SetResourceWriter(dummyWriter);
			host = new MockDesignerLoaderHost();
			host.AddService(typeof(IResourceService), resourceService);
			loader = new PythonDesignerLoader(new MockDesignerGenerator());
			loader.BeginLoad(host);
			
			reader = loader.GetResourceReader(CultureInfo.InvariantCulture) as MockResourceReader;
			writer = loader.GetResourceWriter(CultureInfo.InvariantCulture) as MockResourceWriter;
		}
		
		[Test]
		public void ResourceReaderInstance()
		{
			Assert.IsTrue(Object.ReferenceEquals(dummyReader, reader));
		}
		
		[Test]
		public void CultureInfoPassedToResourceServiceForReader()
		{
			Assert.AreEqual(CultureInfo.InvariantCulture, resourceService.ResourceReaderCultureInfo);
		}
		
		[Test]
		public void GetReaderWhenNoResourceService()
		{
			PythonDesignerLoader loader = new PythonDesignerLoader(new MockDesignerGenerator());
			loader.BeginLoad(new MockDesignerLoaderHost());
			Assert.IsNull(loader.GetResourceReader(CultureInfo.InvariantCulture));
		}

		[Test]
		public void GetWriterWhenNoResourceService()
		{
			PythonDesignerLoader loader = new PythonDesignerLoader(new MockDesignerGenerator());
			loader.BeginLoad(new MockDesignerLoaderHost());
			Assert.IsNull(loader.GetResourceWriter(CultureInfo.InvariantCulture));
		}
		
		[Test]
		public void ResourceWriterInstance()
		{
			Assert.IsTrue(Object.ReferenceEquals(dummyWriter, writer));
		}
		
		[Test]
		public void CultureInfoPassedToResourceServiceForWriter()
		{
			Assert.AreEqual(CultureInfo.InvariantCulture, resourceService.ResourceWriterCultureInfo);
		}		
	}
}
