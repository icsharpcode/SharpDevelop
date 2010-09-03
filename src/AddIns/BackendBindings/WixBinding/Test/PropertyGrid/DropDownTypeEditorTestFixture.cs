// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
