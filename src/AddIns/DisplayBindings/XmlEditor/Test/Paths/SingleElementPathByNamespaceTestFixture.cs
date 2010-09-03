// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Paths
{
	[TestFixture]
	public class SingleElementPathByNamespaceTestFixture
	{
		XmlElementPathsByNamespace paths;
		
		[SetUp]
		public void Init()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("a", "a-namespace"));
			paths = new XmlElementPathsByNamespace(path);
		}
		
		[Test]
		public void HasSingleXmlElementPath()
		{
			Assert.AreEqual(1, paths.Count);
		}
		
		[Test]
		public void XmlElementPathHasOneElement()
		{
			XmlElementPath expectedPath = new XmlElementPath();
			expectedPath.AddElement(new QualifiedName("a", "a-namespace"));
			
			Assert.AreEqual(expectedPath, paths[0]);
		}
	}
}
