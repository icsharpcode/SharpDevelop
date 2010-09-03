// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.Paths
{
	[TestFixture]
	public class NoElementPathTestFixture
	{
		XmlElementPath path;
		
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			path = new XmlElementPath();
		}
		
		[Test]
		public void HasNoItems()
		{
			Assert.AreEqual(0, path.Elements.Count);
		}
		
		[Test]
		public void PathIsEmpty()
		{
			Assert.IsTrue(path.IsEmpty);
		}
		
		[Test]
		public void Equality()
		{
			XmlElementPath newPath = new XmlElementPath();
			
			Assert.IsTrue(newPath.Equals(path), "Should be equal.");
		}
		
		[Test]
		public void EqualsItself()
		{
			Assert.IsTrue(path.Equals(path), "Should be equal.");
		}
		
		[Test]
		public void NotEqual()
		{
			XmlElementPath newPath = new XmlElementPath();
			newPath.AddElement(new QualifiedName("Foo", "bar"));
			
			Assert.IsFalse(newPath.Equals(path), "Should not be equal.");
		}
		
		[Test]
		public void NotEqualToADifferentType()
		{
			Object o = new Object();
			Assert.IsFalse(path.Equals(o), "Should not be equal.");
		}	
		
		[Test]
		public void Compact()
		{
			path.Compact();
			Equality();
		}
		
		[Test]
		public void HashCode()
		{
			Assert.AreEqual(path.Elements.GetHashCode(), path.GetHashCode());
		}
		
		[Test]
		public void PathToStringIsEmpty()
		{
			string expectedToString = String.Empty;
			Assert.AreEqual(expectedToString, path.ToString());
		}
		
		[Test]
		public void RootNamespaceIsEmptyString()
		{
			Assert.AreEqual(String.Empty, path.GetRootNamespace());
		}
	}
}
