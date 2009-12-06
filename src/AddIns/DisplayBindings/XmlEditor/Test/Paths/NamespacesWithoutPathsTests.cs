// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
