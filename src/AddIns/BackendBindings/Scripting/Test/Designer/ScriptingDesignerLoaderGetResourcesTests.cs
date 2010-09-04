// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Designer
{
	[TestFixture]
	public class ScriptingDesignerLoaderGetResourcesTests
	{
		ScriptingDesignerLoader loader;
		MockDesignerLoaderHost host;
		MockResourceService resourceService;
		MockResourceReader fakeReader;
		MockResourceWriter fakeWriter;
		MockResourceWriter writer;
		MockResourceReader reader;

		[Test]
		public void GetResourceReader_PassedInvariantCulture_ReturnsReaderFromResourceService()
		{
			BeginLoad();
			GetResourceReader();
			Assert.AreEqual(fakeReader, reader);
		}
		
		void BeginLoad()
		{
			CreateResourceService();
			CreateDesignerLoaderHost();
			
			host.AddService(typeof(IResourceService), resourceService);
			
			BeginLoad(host);	
		}
				
		void CreateResourceService()
		{
			fakeReader = new MockResourceReader();
			fakeWriter = new MockResourceWriter();
			resourceService = new MockResourceService();
			resourceService.SetResourceReader(fakeReader);
			resourceService.SetResourceWriter(fakeWriter);
		}
		
		void CreateDesignerLoaderHost()
		{
			host = new MockDesignerLoaderHost();
		}
		
		void BeginLoad(IDesignerLoaderHost host)
		{
			loader = new ScriptingDesignerLoader(new MockDesignerGenerator());			
			loader.BeginLoad(host);
		}
		
		void BeginLoadWithoutResourceService()
		{
			CreateDesignerLoaderHost();
			BeginLoad(host);
		}
		
		void GetResourceReader()
		{
			reader = loader.GetResourceReader(CultureInfo.InvariantCulture) as MockResourceReader;
		}
		
		void GetResourceWriter()
		{
			writer = loader.GetResourceWriter(CultureInfo.InvariantCulture) as MockResourceWriter;
		}
		
		[Test]
		public void GetResourceReader_PassedInvariantCulture_CultureInfoPassedToResourceServiceForReader()
		{
			BeginLoad();
			GetResourceReader();
			CultureInfo culture = resourceService.CultureInfoPassedToGetResourceReader;
			CultureInfo expectedCulture = CultureInfo.InvariantCulture;
			Assert.AreEqual(expectedCulture, culture);
		}
		
		[Test]
		public void GetResourceReader_NoResourceService_ReturnsNull()
		{
			BeginLoadWithoutResourceService();
			GetResourceReader();
			Assert.IsNull(reader);
		}
		
		[Test]
		public void GetResourceWriter_PassedInvariantCulture_ReturnsWriterFromResourceService()
		{
			BeginLoad();
			GetResourceWriter();
			Assert.AreEqual(fakeWriter, writer);
		}
		
		[Test]
		public void GetResourceWriter_PassedInvariantCulture_CultureInfoPassedToResourceServiceForWriter()
		{
			BeginLoad();
			GetResourceWriter();
			CultureInfo culture = resourceService.CultureInfoPassedToGetResourceWriter;
			CultureInfo expectedCulture = CultureInfo.InvariantCulture;
			Assert.AreEqual(expectedCulture, culture);
		}
		
		[Test]
		public void GetResourceWriter_NoResourceService_ReturnsNull()
		{
			BeginLoadWithoutResourceService();
			GetResourceWriter();
			Assert.IsNull(writer);
		}
	}
}
