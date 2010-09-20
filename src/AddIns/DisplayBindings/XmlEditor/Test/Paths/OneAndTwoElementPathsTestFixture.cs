// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Paths
{
	[TestFixture]
	public class OneAndTwoElementPathsByNamespaceTestFixture
	{
		XmlElementPathsByNamespace paths;
		
		[SetUp]
		public void Init()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("a", "a-namespace"));
			path.AddElement(new QualifiedName("b", "b-namespace"));
			path.AddElement(new QualifiedName("aa", "a-namespace"));
			paths = new XmlElementPathsByNamespace(path);
		}
		
		[Test]
		public void HasTwoXmlElementPaths()
		{
			Assert.AreEqual(2, paths.Count);
		}
		
		[Test]
		public void FirstXmlElementPathHasTwoElements()
		{
			XmlElementPath expectedPath = new XmlElementPath();
			expectedPath.AddElement(new QualifiedName("a", "a-namespace"));
			expectedPath.AddElement(new QualifiedName("aa", "a-namespace"));
			
			Assert.AreEqual(expectedPath, paths[0]);
		}

		[Test]
		public void SecondXmlElementPathHasOneElement()
		{
			XmlElementPath expectedPath = new XmlElementPath();
			expectedPath.AddElement(new QualifiedName("b", "b-namespace"));
			
			Assert.AreEqual(expectedPath, paths[1]);
		}
	}
}
