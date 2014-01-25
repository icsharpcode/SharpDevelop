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

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Windows.Forms;

namespace WixBinding.Tests.DialogXmlGeneration
{
	/// <summary>
	/// Tests the WixDialog.GetControlTypeName method.
	/// </summary>
	[TestFixture]
	public class GetControlTypesTests
	{
		[Test]
		public void ButtonType()
		{
			Assert.AreEqual("PushButton", WixDialog.GetControlTypeName(typeof(Button)));
		}
		
		[Test]
		public void TextBoxType()
		{
			Assert.AreEqual("Edit", WixDialog.GetControlTypeName(typeof(TextBox)));
		}
		
		[Test]
		public void LabelType()
		{
			Assert.AreEqual("Text", WixDialog.GetControlTypeName(typeof(Label)));
		}
				
		[Test]
		public void CheckBoxType()
		{
			Assert.AreEqual("CheckBox", WixDialog.GetControlTypeName(typeof(CheckBox)));
		}
		
		[Test]
		public void RichTextBoxType()
		{
			Assert.AreEqual("ScrollableText", WixDialog.GetControlTypeName(typeof(RichTextBox)));
		}
		
		[Test]
		public void ComboBoxType()
		{
			Assert.AreEqual("ComboBox", WixDialog.GetControlTypeName(typeof(ComboBox)));
		}
		
		[Test]
		public void GroupBoxType()
		{
			Assert.AreEqual("GroupBox", WixDialog.GetControlTypeName(typeof(GroupBox)));
		}
		
		[Test]
		public void ListBoxType()
		{
			Assert.AreEqual("ListBox", WixDialog.GetControlTypeName(typeof(ListBox)));
		}

		[Test]
		public void ListViewType()
		{
			Assert.AreEqual("ListView", WixDialog.GetControlTypeName(typeof(ListView)));
		}
		
		[Test]
		public void ProgressBarType()
		{
			Assert.AreEqual("ProgressBar", WixDialog.GetControlTypeName(typeof(ProgressBar)));
		}
		
		[Test]
		public void MaskedTextBoxType()
		{
			Assert.AreEqual("MaskedEdit", WixDialog.GetControlTypeName(typeof(MaskedTextBox)));
		}
		
		[Test]
		public void TreeViewType()
		{
			Assert.AreEqual("SelectionTree", WixDialog.GetControlTypeName(typeof(TreeView)));
		}
		
		[Test]
		public void PictureBoxType()
		{
			Assert.AreEqual("Bitmap", WixDialog.GetControlTypeName(typeof(PictureBox)));
		}
		
		[Test]
		public void RadioButtonGroupType()
		{
			Assert.AreEqual("RadioButtonGroup", WixDialog.GetControlTypeName(typeof(RadioButtonGroupBox)));
		}
	}
}
