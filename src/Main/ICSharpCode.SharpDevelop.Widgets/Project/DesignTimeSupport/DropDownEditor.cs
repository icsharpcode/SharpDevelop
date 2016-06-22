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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.SharpDevelop.Widgets.DesignTimeSupport
{
	public abstract class DropDownEditor : UITypeEditor
	{
		/// <summary>
		/// Returns the drop down style.
		/// </summary>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		
		public override bool IsDropDownResizable {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Shows the drop down editor control in the drop down so the user
		/// can change the value.
		/// </summary>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService editorService = null;
			
			if (provider != null) {
				editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			}
			
			if (editorService != null) {
				using (Control control = CreateDropDownControl(context, editorService)) {
					SetValue(control, value);
					editorService.DropDownControl(control);
					value = GetValue(control);
				}
			}
			
			return value;
		}
		
		/// <summary>
		/// Creates the drop down control.
		/// </summary>
		protected abstract Control CreateDropDownControl(ITypeDescriptorContext context, IWindowsFormsEditorService editorService);
		
		/// <summary>
		/// Sets the current value in the drop down control.
		/// </summary>
		protected virtual void SetValue(Control control, object value)
		{
			DropDownEditorListBox listBox = (DropDownEditorListBox)control;
			listBox.Value = (string)value;
		}
		
		/// <summary>
		/// Gets the current value from the drop down control.
		/// </summary>
		protected virtual object GetValue(Control control)
		{
			DropDownEditorListBox listBox = (DropDownEditorListBox)control;
			return listBox.Value;
		}
	}
}
