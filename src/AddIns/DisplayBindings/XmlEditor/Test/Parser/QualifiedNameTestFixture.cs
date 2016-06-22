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
using System.Xml;

namespace XmlEditor.Tests.Parser
{
	/// <summary>
	/// Tests the comparison of <see cref="QualifiedName"/> items.
	/// </summary>
	[TestFixture]
	public class QualifiedNameTestFixture
	{
		[Test]
		public void EqualsTest1()
		{
			QualifiedName name1 = new QualifiedName("foo", "http://foo.com");
			QualifiedName name2 = new QualifiedName("foo", "http://foo.com");
			
			Assert.AreEqual(name1, name2, "Should be the same.");
		}
		
		[Test]
		public void EqualsTest2()
		{
			QualifiedName name1 = new QualifiedName("foo", "http://foo.com", "f");
			QualifiedName name2 = new QualifiedName("foo", "http://foo.com", "f");
			
			Assert.AreEqual(name1, name2, "Should be the same.");
		}		
		
		[Test]
		public void EqualsTest3()
		{
			QualifiedName name1 = new QualifiedName("foo", "http://foo.com", "f");
			QualifiedName name2 = new QualifiedName("foo", "http://foo.com", "ggg");
			
			Assert.IsTrue(name1 == name2, "Should be the same.");
		}	
		
		[Test]
		public void NotEqualsTest1()
		{
			QualifiedName name1 = new QualifiedName("foo", "http://foo.com", "f");
			QualifiedName name2 = new QualifiedName("foo", "http://bar.com", "f");
			
			Assert.IsFalse(name1 == name2, "Should not be the same.");
		}		
		
		[Test]
		public void NotEqualsTest2()
		{
			QualifiedName name1 = new QualifiedName("foo", "http://foo.com", "f");
			QualifiedName name2 = null; 
			
			Assert.IsFalse(name1 == name2, "Should not be the same.");
		}		
		
		[Test]
		public void HashCodeTest()
		{
			QualifiedName name1 = new QualifiedName("foo", "http://foo.com", "f");
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName("foo", "http://foo.com");

			Assert.AreEqual(name1.GetHashCode(), xmlQualifiedName.GetHashCode());
		}
	}
}
