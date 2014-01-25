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
	public class XmlNamespaceEqualsTests
	{
		XmlNamespace lhs;
		XmlNamespace rhs;
		
		[SetUp]
		public void Init()
		{
			lhs = new XmlNamespace();
			rhs = new XmlNamespace();
		}
		
		[Test]
		public void EmptyNamespacesAreEqual()
		{
			Assert.IsTrue(lhs.Equals(rhs));
		}
		
		[Test]
		public void NullNamespaceIsNotEqual()
		{
			Assert.IsFalse(lhs.Equals(null));
		}
		
		[Test]
		public void SameNamespaceAndPrefixAreEqual()
		{
			lhs.Name = "a-ns";
			lhs.Prefix = "a";
			
			rhs.Name = "a-ns";
			rhs.Prefix = "a";
			
			Assert.IsTrue(lhs.Equals(rhs));
		}
		
		[Test]
		public void DifferentNamespacesAreNotEqual()
		{
			lhs.Name = "namespace";
			lhs.Prefix = "a";
			
			rhs.Name = "different-namespace";
			rhs.Prefix = "a";
			
			Assert.IsFalse(lhs.Equals(rhs));
		}
		
		[Test]
		public void SameNamespaceButDifferentPrefixAreNotEqual()
		{
			lhs.Name = "namespace";
			lhs.Prefix = "a";
			
			rhs.Name = "namespace";
			rhs.Prefix = "b";
			
			Assert.IsFalse(lhs.Equals(rhs));
		}
	}
}
