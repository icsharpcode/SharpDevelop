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
	}
}
