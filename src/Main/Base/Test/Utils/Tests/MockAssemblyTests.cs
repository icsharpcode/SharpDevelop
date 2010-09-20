// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.Utils.Tests
{
	[TestFixture]
	public class MockAssemblyTests
	{
		MockAssembly assembly;
		Stream predefinedResourceStream;
		
		[SetUp]
		public void Init()
		{
			assembly = new MockAssembly();
			predefinedResourceStream = new MemoryStream();
			assembly.AddManifestResourceStream("ICSharpCode.Test.Xml.xshd", predefinedResourceStream);
		}
		
		[TearDown]
		public void TearDown()
		{
			predefinedResourceStream.Dispose();
		}
		
		[Test]
		public void GetManifestResourceStreamReturnsPredefinedStreamForKnownResource()
		{
			Assert.AreSame(predefinedResourceStream, assembly.GetManifestResourceStream("ICSharpCode.Test.Xml.xshd"));
		}
		
		[Test]
		public void GetManifestResourceStreamReturnsNullForUnknownResource()
		{
			Assert.IsNull(assembly.GetManifestResourceStream("UnknownResource.xshd"));
		}
		
		[Test]
		public void AssemblyExportedTypesReturnsAnEmptyArray()
		{
			Assert.AreEqual(0, assembly.GetExportedTypes().Length);
		}
	}
}
