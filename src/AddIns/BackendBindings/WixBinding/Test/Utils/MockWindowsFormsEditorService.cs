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

using ICSharpCode.SharpDevelop.Widgets.DesignTimeSupport;
using ICSharpCode.WixBinding;
using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace WixBinding.Tests.Utils
{
	public class MockWindowsFormsEditorService : IWindowsFormsEditorService
	{
		Type controlType;
		string newValue;
		bool newValueSet;
		
		public MockWindowsFormsEditorService()
		{
		}
		
		public void CloseDropDown()
		{
		}
		
		public void DropDownControl(Control control)
		{
			controlType = control.GetType();
			
			if (newValueSet) {
				GuidEditorListBox guidListBox = control as GuidEditorListBox;
				DropDownEditorListBox dropDownListBox = control as DropDownEditorListBox;
				if (guidListBox != null) {
					guidListBox.Guid = newValue;
				} else if (dropDownListBox != null) {
					dropDownListBox.Value = newValue;
				}
			}
		}
		
		public DialogResult ShowDialog(Form dialog)
		{
			return DialogResult.None;
		}
		
		public Type GetDropDownControlTypeUsed()
		{
			return controlType;
		}
		
		public void SetNewValue(string value)
		{
			newValue = value;
			newValueSet = true;
		}
	}
}
