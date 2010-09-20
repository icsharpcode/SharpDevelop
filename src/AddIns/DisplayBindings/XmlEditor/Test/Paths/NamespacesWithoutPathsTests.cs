// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Paths
{
	[TestFixture]
	public class NamespacesWithoutPathsTests
	{
		XmlElementPath path;
		XmlNamespace fooNamespace;
		XmlNamespace barNamespace;
		
		[SetUp]
		public void Init()
		{
			path = new XmlElementPath();
			fooNamespace = new XmlNamespace("foo", "http://foo");
			barNamespace = new XmlNamespace("bar", "http://bar");
		}
		
		[Test]
		public void TwoNamespacesInScopeAndOneNamespaceUsedInPathNamespacesWithoutPathsReturnsUnusedNamespace()
		{
			path.AddElement(new QualifiedName("foo-root", "http://foo"));
			path.NamespacesInScope.Add(fooNamespace);
			path.NamespacesInScope.Add(barNamespace);
			
			XmlElementPathsByNamespace pathsByNamespaces = new XmlElementPathsByNamespace(path);
			
			XmlNamespace[] expectedNamespaces = new XmlNamespace[] {barNamespace};
			Assert.AreEqual(expectedNamespaces, pathsByNamespaces.NamespacesWithoutPaths.ToArray());
		}
	}
}
