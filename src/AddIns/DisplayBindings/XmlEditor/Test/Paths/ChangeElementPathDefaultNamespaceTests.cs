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
	public class ChangeElementPathDefaultNamespaceTests
	{
		[Test]
		public void ChangeNamespaceForTwoElementsWithoutNamespaceModifiesAllItems()
		{
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("root", String.Empty));
			path.Elements.Add(new QualifiedName("child", String.Empty));
			path.SetNamespaceForUnqualifiedNames("http://test");
			
			XmlElementPath expectedPath = new XmlElementPath();
			expectedPath.Elements.Add(new QualifiedName("root", "http://test"));
			expectedPath.Elements.Add(new QualifiedName("child", "http://test"));
			
			Assert.IsTrue(expectedPath.Equals(path));
		}
		
		[Test]
		public void ChangeNamespaceForTwoElementsWhereChildElementHasOwnNamespaceOnlyAffectsRootElement()
		{
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("root", String.Empty));
			path.Elements.Add(new QualifiedName("child", "has-namespace-already"));
			path.SetNamespaceForUnqualifiedNames("http://test");
			
			XmlElementPath expectedPath = new XmlElementPath();
			expectedPath.Elements.Add(new QualifiedName("root", "http://test"));
			expectedPath.Elements.Add(new QualifiedName("child", "has-namespace-already"));
			
			Assert.IsTrue(expectedPath.Equals(path));
		}		
	}
}
