// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.Paths
{
	[TestFixture]
	public class SingleElementPathTestFixture
	{
		XmlElementPath path;
		QualifiedName qualifiedName;
		
		[SetUp]
		public void Init()
		{
			path = new XmlElementPath();
			qualifiedName = new QualifiedName("foo", "http://foo");
			path.AddElement(qualifiedName);
		}
		
		[Test]
		public void HasOneItem()
		{
			Assert.AreEqual(1, path.Elements.Count, 
			                "Should have 1 element.");
		}
		
		[Test]
		public void RemoveLastItem()
		{
			path.Elements.RemoveLast();
			Assert.AreEqual(0, path.Elements.Count, "Should have no items.");
		}
		
		[Test]
		public void Equality()
		{
			XmlElementPath newPath = new XmlElementPath();
			newPath.AddElement(new QualifiedName("foo", "http://foo"));
			
			Assert.IsTrue(newPath.Equals(path), "Should be equal.");
		}
		
		[Test]
		public void NotEqual()
		{
			XmlElementPath newPath = new XmlElementPath();
			newPath.AddElement(new QualifiedName("Foo", "bar"));
			
			Assert.IsFalse(newPath.Equals(path), "Should not be equal.");
		}	
		
		[Test]
		public void Compact()
		{
			path.Compact();
			Equality();
		}
		
		[Test]
		public void PathToString()
		{
			string expectedToString = "foo [http://foo]";
			Assert.AreEqual(expectedToString, path.ToString());
		}
		
		[Test]
		public void TwoElementsPathToString()
		{
			QualifiedName qualifiedName = new QualifiedName("bar", "http://foo");
			path.AddElement(qualifiedName);
			Assert.AreEqual("foo [http://foo] > bar [http://foo]", path.ToString());
		}
		
		[Test]
		public void GetRootElementNamespace()
		{
			Assert.AreEqual("http://foo", path.GetRootNamespace());
		}
	}
}
