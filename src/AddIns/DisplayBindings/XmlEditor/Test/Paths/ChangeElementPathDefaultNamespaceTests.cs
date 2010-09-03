// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Paths
{
	[TestFixture]
	public class ChangeElementPathDefaultNamespaceTests
	{
		[Test]
		public void ChangeNamespaceForTwoElementsWithoutNamespaceModifiesAllItems()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("root", String.Empty));
			path.AddElement(new QualifiedName("child", String.Empty));
			path.SetNamespaceForUnqualifiedNames("http://test");
			
			XmlElementPath expectedPath = new XmlElementPath();
			expectedPath.AddElement(new QualifiedName("root", "http://test"));
			expectedPath.AddElement(new QualifiedName("child", "http://test"));
			
			Assert.IsTrue(expectedPath.Equals(path));
		}
		
		[Test]
		public void ChangeNamespaceForTwoElementsWhereChildElementHasOwnNamespaceOnlyAffectsRootElement()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("root", String.Empty));
			path.AddElement(new QualifiedName("child", "has-namespace-already"));
			path.SetNamespaceForUnqualifiedNames("http://test");
			
			XmlElementPath expectedPath = new XmlElementPath();
			expectedPath.AddElement(new QualifiedName("root", "http://test"));
			expectedPath.AddElement(new QualifiedName("child", "has-namespace-already"));
			
			Assert.IsTrue(expectedPath.Equals(path));
		}		
	}
}
