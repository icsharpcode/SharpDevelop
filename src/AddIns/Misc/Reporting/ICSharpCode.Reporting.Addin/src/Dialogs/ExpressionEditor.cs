/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 11.05.2014
 * Time: 18:37
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
	/// Description of ExpressionEditor.
	/// </summary>
	public class ExpressionEditor:UITypeEditor
	{
		const string expressionEditor = "ExpressionEditor";
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			string s = String.Empty;;
			
			if (value != null) {
				s = value.ToString();
			}
			IStringBasedEditorDialog ed = new TextEditorDialog(s, expressionEditor);
			return ed.ShowDialog() == DialogResult.OK ? ed.TextValue : s;
		}
		
		
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return  UITypeEditorEditStyle.Modal;
		}
	}
}
