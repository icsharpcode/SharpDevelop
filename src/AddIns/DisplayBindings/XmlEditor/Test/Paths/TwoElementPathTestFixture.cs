//
// SharpDevelop Xml Editor
//
// Copyright (C) 2005 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

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
			path.Elements.Add(firstQualifiedName);
			
			secondQualifiedName = new QualifiedName("bar", "http://bar", "b");
			path.Elements.Add(secondQualifiedName);
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
			Assert.AreEqual("b", path.Elements.LastPrefix, "Incorrect last prefix.");
		}
		
		[Test]
		public void LastPrefixAfterLastItemRemoved()
		{
			path.Elements.RemoveLast();
			Assert.AreEqual("f", path.Elements.LastPrefix, "Incorrect last prefix.");
		}	
		
		[Test]
		public void Equality()
		{
			XmlElementPath newPath = new XmlElementPath();
			newPath.Elements.Add(new QualifiedName("foo", "http://foo", "f"));
			newPath.Elements.Add(new QualifiedName("bar", "http://bar", "b"));
			
			Assert.IsTrue(newPath.Equals(path), "Should be equal.");
		}
		
		[Test]
		public void NotEqual()
		{
			XmlElementPath newPath = new XmlElementPath();
			newPath.Elements.Add(new QualifiedName("aaa", "a", "a"));
			newPath.Elements.Add(new QualifiedName("bbb", "b", "b"));
			
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
			newPath.Elements.Add(new QualifiedName("bar", "http://bar", "b"));
			
			path.Compact();
			Assert.IsTrue(newPath.Equals(path), "Should be equal.");
		}
	}
}
