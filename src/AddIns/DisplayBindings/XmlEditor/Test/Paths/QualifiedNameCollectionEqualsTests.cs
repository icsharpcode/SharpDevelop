// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
