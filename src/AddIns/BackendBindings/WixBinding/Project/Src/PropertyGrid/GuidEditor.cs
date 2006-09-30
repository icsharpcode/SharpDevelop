// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Property grid editor for Guids
	/// </summary>
	public class GuidEditor : UITypeEditor
	{
		public GuidEditor()
		{
		}
		
		/// <summary>
		/// Returns the drop down style.
		/// </summary>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		
		/// <summary>
		/// Shows the Guid editor control in the drop down so the user
		/// can change the Guid.
		/// </summary>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService editorService = null;
			
			if (provider != null) {
				editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			}
			
			if (editorService != null) {
				using (GuidEditorListBox listBox = new GuidEditorListBox(editorService)) {
					listBox.Guid = (string)value;
					editorService.DropDownControl(listBox);
					value = listBox.Guid;
				}
			}
			
			return value;
		}
	}
}
