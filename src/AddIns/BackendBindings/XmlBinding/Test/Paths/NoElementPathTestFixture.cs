// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2752 $</version>
// </file>

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
			Assert.AreEqual(0, path.Elements.Count, 
			                "Should not be any elements.");
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
			newPath.Elements.Add(new QualifiedName("Foo", "bar"));
			
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
		public void PathToString()
		{
			string expectedToString = String.Empty;
			Assert.AreEqual(expectedToString, path.ToString());
		}
	}
}
