// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
