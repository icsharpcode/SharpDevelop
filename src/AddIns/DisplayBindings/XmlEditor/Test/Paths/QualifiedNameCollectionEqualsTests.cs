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
	public class QualifiedNameCollectionEqualsTests
	{
		QualifiedNameCollection lhs;
		QualifiedNameCollection rhs;
		
		[SetUp]
		public void Init()
		{
			lhs = new QualifiedNameCollection();
			rhs = new QualifiedNameCollection();			
		}
		
		[Test]
		public void EmptyCollectionsAreEqual()
		{
			Assert.IsTrue(lhs.Equals(rhs));
		}
		
		[Test]
		public void NullIsNotEqualToCollection()
		{
			Assert.IsFalse(lhs.Equals(null));
		}
		
		[Test]
		public void EqualReturnsFalseWhenOneCollectionHasAnItemAndTheOtherDoesNot()
		{
			lhs.Add(new QualifiedName("root", "ns", "a"));
			Assert.IsFalse(lhs.Equals(rhs));
		}
		
		[Test]
		public void EqualsReturnsFalseWhenQualifiedNamesAreDifferent()
		{
			lhs.Add(new QualifiedName("name1", "ns1", "prefix1"));
			rhs.Add(new QualifiedName("name2", "ns2", "prefix2"));
			Assert.IsFalse(lhs.Equals(rhs));
		}
		
		[Test]
		public void EqualsReturnsTrueWhenQualifiedNamesHaveSameNameAndNamespaceButDifferentPrefix()
		{
			lhs.Add(new QualifiedName("name", "ns", "prefix1"));
			rhs.Add(new QualifiedName("name", "ns", "prefix2"));
			Assert.IsTrue(lhs.Equals(rhs));
		}	
		
		[Test]
		public void EqualsReturnsFalseWhenQualifiedNamesHaveSameNameButDifferentNamespace()
		{
			lhs.Add(new QualifiedName("name", "ns1", "prefix"));
			rhs.Add(new QualifiedName("name", "ns2", "prefix"));
			Assert.IsFalse(lhs.Equals(rhs));
		}			
	}
}
