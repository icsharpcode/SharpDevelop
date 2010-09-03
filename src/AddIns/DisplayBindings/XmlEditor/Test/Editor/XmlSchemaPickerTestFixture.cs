// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class XmlSchemaPickerTestFixture
	{
		const string NoSchemaNamespaceText = "${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.None}";
		
		XmlSchemaPicker schemaPicker;
		MockSelectXmlSchemaWindow window;
		
		[SetUp]
		public void Init()
		{
			window = new MockSelectXmlSchemaWindow();
			string[] namespaceUris = new string[] { "b-namespace", "a-namespace" };
			schemaPicker = new XmlSchemaPicker(namespaceUris, window);
		}

		[Test]
		public void SchemaNamespacesAreAddedAlphabeticallyAndFirstItemIsNoNamespaceText()
		{
			string[] expectedItems = new string[] { NoSchemaNamespaceText, "a-namespace", "b-namespace" };
			Assert.AreEqual(expectedItems, window.SchemaNamespaces.ToArray());
		}
		
		[Test]
		public void CanPreselectNamespaceInSchemaList()
		{
			schemaPicker.SelectSchemaNamespace("b-namespace");
			Assert.AreEqual("b-namespace", window.SelectedItem);
		}
		
		[Test]
		public void UserSelectedNamespaceReturnedFromSchemaListWindow()
		{
			window.SelectedIndex = 1;
			Assert.AreEqual("a-namespace", schemaPicker.GetSelectedSchemaNamespace());
		}

		[Test]
		public void EmptyStringReturnedWhenSelectedIndexIsMinusOne()
		{
			window.SelectedIndex = -1;
			Assert.AreEqual(String.Empty, schemaPicker.GetSelectedSchemaNamespace());
		}
		
		[Test]
		public void EmptyStringReturnedWhenNoneSelectedInListBox()
		{
			window.SelectedIndex = 0;
			Assert.AreEqual(String.Empty, schemaPicker.GetSelectedSchemaNamespace());
		}
		
		[Test]
		public void UnknownNamespaceSelectsNoneStringInListBox()
		{
			schemaPicker.SelectSchemaNamespace("Unknown namespace");
			Assert.AreEqual(NoSchemaNamespaceText, window.SelectedItem);
		}
		
		[Test]
		public void EmptyNamespaceSelectsNoneStringInListBox()
		{
			schemaPicker.SelectSchemaNamespace(String.Empty);
			Assert.AreEqual(NoSchemaNamespaceText, window.SelectedItem);
		}
	}
}
