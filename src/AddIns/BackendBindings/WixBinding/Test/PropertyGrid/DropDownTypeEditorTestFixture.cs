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
using ICSharpCode.SharpDevelop.Widgets.DesignTimeSupport;
using NUnit.Framework;
using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PropertyGrid
{
	[TestFixture]
	public class DropDownTypeEditorTestFixture
	{
		WixDropDownEditor editor;
		object newValue;
		string expectedNewValue;
		MockServiceProvider mockServiceProvider;
		MockWindowsFormsEditorService mockWindowsFormsEditorService;
		Type expectedControlType;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			editor = new WixDropDownEditor();
			
			// Edit the value.
			mockServiceProvider = new MockServiceProvider();
			mockWindowsFormsEditorService = new MockWindowsFormsEditorService();
			mockServiceProvider.SetServiceToReturn(mockWindowsFormsEditorService);
			expectedNewValue = "NewValue";
			mockWindowsFormsEditorService.SetNewValue(expectedNewValue);
						
			newValue = editor.EditValue(mockServiceProvider, "Test");
			
			expectedControlType = mockWindowsFormsEditorService.GetDropDownControlTypeUsed();
		}
		
		[Test]
		public void DropDownEditStyle()
		{
			Assert.AreEqual(UITypeEditorEditStyle.DropDown, editor.GetEditStyle());
		}
		
		[Test]
		public void IsDropDownResizable()
		{
			Assert.IsFalse(editor.IsDropDownResizable);
		}
		
		[Test]
		public void NewValue()
		{
			Assert.AreEqual(newValue, expectedNewValue);
		}
		
		[Test]
		public void WindowsFormsEditorServiceRequested()
		{
			Assert.AreEqual(typeof(IWindowsFormsEditorService), 
				mockServiceProvider.GetServiceRequested(0));
		}
		
		[Test]
		public void SameValueReturnedIfNoServiceProviderSet()
		{
			string oldValue = "Test";
			string newValue = (string)editor.EditValue(null, null, oldValue);
			Assert.IsTrue(Object.ReferenceEquals(oldValue, newValue));
		}
		
		[Test]
		public void SameValueReturnedIfWindowsFormsServiceReturned()
		{
			mockServiceProvider.SetServiceToReturn(null);
			
			string oldValue = "Test";
			string newValue = (string)editor.EditValue(null, mockServiceProvider, oldValue);
			Assert.IsTrue(Object.ReferenceEquals(oldValue, newValue));			
			Assert.AreEqual(typeof(IWindowsFormsEditorService), 
				mockServiceProvider.GetServiceRequested(1));
		}
		
		[Test]
		public void SameValueReturnedIfNotEdited()
		{
			mockWindowsFormsEditorService = new MockWindowsFormsEditorService();
			mockServiceProvider.SetServiceToReturn(mockWindowsFormsEditorService);
			
			string oldValue = "test";
			string newValue = (string)editor.EditValue(null, mockServiceProvider, oldValue);
			Assert.AreEqual(oldValue, newValue);
		}
		
		[Test]
		public void DropDownEditorControlUsedAsDropDownControl()
		{
			Assert.AreEqual("DropDownEditorListBox", expectedControlType.Name);
		}
	}
}
