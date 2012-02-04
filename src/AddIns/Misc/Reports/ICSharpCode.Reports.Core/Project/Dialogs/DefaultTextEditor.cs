// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

using ICSharpCode.Reports.Core.Dialogs;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Dialogs
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

			IStringBasedEditorDialog ed = new TextEditorDialog (s,"ReportSettings");
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
