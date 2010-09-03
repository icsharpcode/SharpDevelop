// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
