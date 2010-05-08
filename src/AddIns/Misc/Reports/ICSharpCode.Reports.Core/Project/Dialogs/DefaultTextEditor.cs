/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier
 * Datum: 20.05.2007
 * Zeit: 18:02
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of UISettingsEditor.
	/// </summary>
	public class DefaultTextEditor:UITypeEditor
	{
		public 	DefaultTextEditor () {
		}
		
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			string s = String.Empty;;
			
			if (value != null) {
				s = value.ToString();
			}

			ITextEditorDialog ed = new TextEditorDialog (s,"ReportSettings");
			if (ed.ShowDialog() == DialogResult.OK) {
				return ed.TextValue;
			} else {
				return s;
			}
		}
		
		
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return  UITypeEditorEditStyle.Modal;
		}
	}
}
