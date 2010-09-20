// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
