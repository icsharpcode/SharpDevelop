// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
