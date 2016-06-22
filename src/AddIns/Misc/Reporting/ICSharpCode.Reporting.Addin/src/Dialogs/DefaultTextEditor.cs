/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 11.05.2014
 * Time: 18:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using ICSharpCode.Reports.Core.Dialogs;

namespace ICSharpCode.Reporting.Addin.Dialogs
{
	/// <summary>
	/// Description of DefaultTextEditor.
	/// </summary>
	public class DefaultTextEditor:UITypeEditor
	{
		const string textEditor = "TextEditor";

		
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			string s = String.Empty;;
			
			if (value != null) {
				s = value.ToString();
			}

			IStringBasedEditorDialog ed = new TextEditorDialog(s, textEditor);
			return ed.ShowDialog() == DialogResult.OK ? ed.TextValue : s;
		}
		
		
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return  UITypeEditorEditStyle.Modal;
		}
	}
}
