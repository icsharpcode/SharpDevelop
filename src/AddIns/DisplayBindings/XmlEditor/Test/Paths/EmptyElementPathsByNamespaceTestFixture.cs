// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Paths
{
	[TestFixture]
	public class EmptyElementPathsByNamespaceTestFixture
	{
		XmlElementPathsByNamespace paths;
		
		[SetUp]
		public void Init()
		{
			XmlElementPath path = new XmlElementPath();
			paths = new XmlElementPathsByNamespace(path);
		}
		
		[Test]
		public void HasNoItemsWhenCreated()
		{
			Assert.AreEqual(0, paths.Count);
		}
	}
}
