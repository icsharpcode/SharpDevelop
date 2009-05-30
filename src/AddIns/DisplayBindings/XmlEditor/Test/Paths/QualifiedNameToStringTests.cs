// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2752 $</version>
// </file>

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Paths
{
	/// <summary>
	/// Tests the QualifiedName.ToString method.
	/// </summary>
	[TestFixture]
	public class QualifiedNameToStringTests
	{
		[Test]
		public void EmptyQualifiedName()
		{
			QualifiedName name = new QualifiedName();
			Assert.AreEqual(String.Empty, name.ToString());
		}
		
		[Test]
		public void NameOnly()
		{
			QualifiedName name = new QualifiedName("root", String.Empty);
			Assert.AreEqual("root", name.ToString());
		}
		
		[Test]
		public void NameAndNamespace()
		{
			QualifiedName name = new QualifiedName("root", "urn:my-uri");
			Assert.AreEqual("root [urn:my-uri]", name.ToString());
		}

		[Test]
		public void NameNamespaceAndPrefix()
		{
			QualifiedName name = new QualifiedName("root", "urn:my-uri", "a");
			Assert.AreEqual("a:root [urn:my-uri]", name.ToString());
		}
		
		[Test]
		public void NameAndPrefixOnly()
		{
			QualifiedName name = new QualifiedName("root", String.Empty, "b");
			Assert.AreEqual("b:root", name.ToString());
		}
		
		[Test]
		public void NullPrefix()
		{
			QualifiedName name = new QualifiedName("root", String.Empty, null);
			Assert.AreEqual("root", name.ToString());
		}
		
		[Test]
		public void NullPrefixAndNonEmptyNamespace()
		{
			QualifiedName name = new QualifiedName("root", "urn:my-uri", null);
			Assert.AreEqual("root [urn:my-uri]", name.ToString());
		}
		
	}
}
