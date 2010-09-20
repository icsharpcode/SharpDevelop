// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class MockSelectXmlSchemaWindowTests
	{
		MockSelectXmlSchemaWindow schemaWindow;
		
		[SetUp]
		public void Init()
		{
			schemaWindow = new MockSelectXmlSchemaWindow();
			schemaWindow.AddSchemaNamespace("a");
			schemaWindow.AddSchemaNamespace("b");
		}
		
		[Test]
		public void SchemaAddedToWindowIsAddedToSchemaNamespacesList()
		{			
			string[] expectedNamespaces = new string[] { "a", "b" };
			
			Assert.AreEqual(expectedNamespaces, schemaWindow.SchemaNamespaces.ToArray());
		}
		
		[Test]
		public void SelectedIndexIsMinusOneAtStart()
		{
			Assert.AreEqual(-1, schemaWindow.SelectedIndex);
		}
		
		[Test]
		public void SelectedIndexSetReturnsCorrectNamespaceAsSelectedItem()
		{
			schemaWindow.SelectedIndex = 1;
			Assert.AreEqual("b", schemaWindow.SelectedItem);
		}
	
		[Test]
		public void ItemIndexOfReturnsCorrectIndex()
		{
			Assert.AreEqual(1, schemaWindow.IndexOfItem("b"));
		}
	}
}
