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
