// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PropertyGrid
{
	[TestFixture]
	public class GuidTypeEditorTestFixture
	{
		GuidEditor editor;
		object newValue;
		string expectedNewGuid;
		MockServiceProvider mockServiceProvider;
		MockWindowsFormsEditorService mockWindowsFormsEditorService;
		Type expectedGuidControlType;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			editor = new GuidEditor();
			
			// Edit the guid value.
			mockServiceProvider = new MockServiceProvider();
			mockWindowsFormsEditorService = new MockWindowsFormsEditorService();
			mockServiceProvider.SetServiceToReturn(mockWindowsFormsEditorService);
			expectedNewGuid = Guid.NewGuid().ToString().ToUpperInvariant();
			mockWindowsFormsEditorService.SetNewValue(expectedNewGuid);
			
			Guid guid = Guid.NewGuid();
			
			newValue = editor.EditValue(mockServiceProvider, guid.ToString().ToUpperInvariant());
			
			expectedGuidControlType = mockWindowsFormsEditorService.GetDropDownControlTypeUsed();
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
		public void NewGuidReturned()
		{
			Assert.IsInstanceOf(typeof(String), newValue);
		}
		
		[Test]
		public void NewGuidValue()
		{
			Assert.AreEqual(newValue, expectedNewGuid);
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
		public void SameGuidValueReturnedIfNotEdited()
		{
			mockWindowsFormsEditorService = new MockWindowsFormsEditorService();
			mockServiceProvider.SetServiceToReturn(mockWindowsFormsEditorService);
			
			string oldValue = Guid.NewGuid().ToString().ToUpperInvariant();
			string newValue = (string)editor.EditValue(null, mockServiceProvider, oldValue);
			Assert.AreEqual(oldValue, newValue);
		}
		
		[Test]
		public void GuidEditorControlUsedAsDropDownControl()
		{
			Assert.AreEqual("GuidEditorListBox", expectedGuidControlType.Name);
		}
	}
}
