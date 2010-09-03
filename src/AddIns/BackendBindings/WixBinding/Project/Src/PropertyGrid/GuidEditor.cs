// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.SharpDevelop.Widgets.DesignTimeSupport;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Property grid editor for Guids
	/// </summary>
	public class GuidEditor : DropDownEditor
	{
		public GuidEditor()
		{
		}
		
		protected override Control CreateDropDownControl(ITypeDescriptorContext context, IWindowsFormsEditorService editorService)
		{
			return new GuidEditorListBox(editorService);
		}
		
		protected override void SetValue(Control control, object value)
		{
			GuidEditorListBox listBox = (GuidEditorListBox)control;
			listBox.Guid = (string)value;
		}
		
		protected override object GetValue(Control control)
		{
			GuidEditorListBox listBox = (GuidEditorListBox)control;
			return listBox.Guid;
		}
	}
}
