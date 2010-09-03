// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Paths
{
	[TestFixture]
	public class QualifiedNameCollectionToStringTests
	{
		QualifiedNameCollection qualifiedNames;
		
		[SetUp]
		public void Init()
		{
			qualifiedNames = new QualifiedNameCollection();
		}
		
		[Test]
		public void EmptyCollectionToStringReturnsEmptyString()
		{
			Assert.AreEqual(String.Empty, qualifiedNames.ToString());
		}
		
		[Test]
		public void OneItemInCollectionReturnsQualifiedNameInToString()
		{
			qualifiedNames.Add(new QualifiedName("root", "ns", "a"));
			Assert.AreEqual("a:root [ns]", qualifiedNames.ToString());
		}
		
		[Test]
		public void TwoItemsInCollectionReturnsTwoQualifiedNamesInToString()
		{
			qualifiedNames.Add(new QualifiedName("root1", "ns1", "a1"));
			qualifiedNames.Add(new QualifiedName("root2", "ns2", "a2"));
			Assert.AreEqual("a1:root1 [ns1] > a2:root2 [ns2]", qualifiedNames.ToString());
		}
				
	}
}
