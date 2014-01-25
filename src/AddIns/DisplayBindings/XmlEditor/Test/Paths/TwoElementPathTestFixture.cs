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

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Paths
{
	[TestFixture]
	public class TwoElementPathTestFixture
	{
		XmlElementPath path;
		QualifiedName firstQualifiedName;
		QualifiedName secondQualifiedName;
		
		[SetUp]
		public void Init()
		{
			path = new XmlElementPath();
			firstQualifiedName = new QualifiedName("foo", "http://foo", "f");
			path.AddElement(firstQualifiedName);
			
			secondQualifiedName = new QualifiedName("bar", "http://bar", "b");
			path.AddElement(secondQualifiedName);
		}
		
		[Test]
		public void HasTwoItems()
		{
			Assert.AreEqual(2, path.Elements.Count, 
			                "Should have 2 elements.");
		}
		
		[Test]
		public void RemoveLastItem()
		{
			path.Elements.RemoveLast();
			Assert.AreEqual(firstQualifiedName, path.Elements[0],
			                "Wrong item removed.");
		}	
		
		[Test]
		public void LastPrefix()
		{
			Assert.AreEqual("b", path.Elements.GetLastPrefix(), "Incorrect last prefix.");
		}
		
		[Test]
		public void LastPrefixAfterLastItemRemoved()
		{
			path.Elements.RemoveLast();
			Assert.AreEqual("f", path.Elements.GetLastPrefix(), "Incorrect last prefix.");
		}	
		
		[Test]
		public void Equality()
		{
			XmlElementPath newPath = new XmlElementPath();
			newPath.AddElement(new QualifiedName("foo", "http://foo", "f"));
			newPath.AddElement(new QualifiedName("bar", "http://bar", "b"));
			
			Assert.IsTrue(newPath.Equals(path), "Should be equal.");
		}
		
		[Test]
		public void NotEqual()
		{
			XmlElementPath newPath = new XmlElementPath();
			newPath.AddElement(new QualifiedName("aaa", "a", "a"));
			newPath.AddElement(new QualifiedName("bbb", "b", "b"));
			
			Assert.IsFalse(newPath.Equals(path), "Should not be equal.");
		}
		
		[Test]
		public void CompactedPathItemCount()
		{
			path.Compact();
			Assert.AreEqual(1, path.Elements.Count, "Should only be one item.");
		}
		
		[Test]
		public void CompactPathItem()
		{
			XmlElementPath newPath = new XmlElementPath();
			newPath.AddElement(new QualifiedName("bar", "http://bar", "b"));
			
			path.Compact();
			Assert.IsTrue(newPath.Equals(path), "Should be equal.");
		}

		[Test]
		public void PathToString()
		{
			string expectedToString = "f:foo [http://foo] > b:bar [http://bar]";
			Assert.AreEqual(expectedToString, path.ToString());
		}
		
		[Test]
		public void ElementPathsByNamespaceFoo()
		{
			XmlElementPathsByNamespace paths = new XmlElementPathsByNamespace(path);
			XmlElementPath fooPath = paths[0];
			
			XmlElementPath expectedPath = new XmlElementPath();
			expectedPath.AddElement(new QualifiedName("foo", "http://foo", "f"));
			
			Assert.IsTrue(expectedPath.Equals(fooPath));
		}
		
		[Test]
		public void TwoElementPathsByNamespace()
		{
			XmlElementPathsByNamespace paths = new XmlElementPathsByNamespace(path);
			Assert.AreEqual(2, paths.Count);
		}
		
		[Test]
		public void ElementPathsByNamespaceBar()
		{
			XmlElementPathsByNamespace paths = new XmlElementPathsByNamespace(path);
			XmlElementPath barPath = paths[1];
			
			XmlElementPath expectedPath = new XmlElementPath();
			expectedPath.AddElement(new QualifiedName("bar", "http://bar", "b"));
			
			Assert.IsTrue(expectedPath.Equals(barPath));
		}
	}
}
