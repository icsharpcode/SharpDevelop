// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace NoGoop.ObjBrowser.Dialogs
{

	internal class ConstructorDialog : Dialog
	{
		internal ConstructorDialog() : base()
		{
		}

		internal static ConstructorInfo GetConstructor(ConstructorInfo[] ci)
		{
			ConstructorDialog d = new ConstructorDialog();
			d.Text = StringParser.Parse("${res:ComponentInspector.ConstructorDialog.Title}");

			TextBox tb = new TextBox();
			tb.Dock = DockStyle.Fill;
			tb.Text = StringParser.Parse("${res:ComponentInspector.ConstructorDialog.Information}");
			tb.BorderStyle = BorderStyle.None;
			tb.Multiline = true;
			tb.AutoSize = true;
			tb.ReadOnly = true;
			tb.WordWrap = true;
			tb.BackColor = d.BackColor;
			d.Controls.Add(tb);

			Label l = new Label();
			l.Dock = DockStyle.Top;
			l.Height = 10;
			d.Controls.Add(l);

			ComboBox cb = new ComboBox();
			cb.Dock = DockStyle.Top;
			foreach (ConstructorInfo c in ci)
				cb.Items.Add(c.ToString());
			cb.SelectedIndex = 0;
			d.Controls.Add(cb);

			DialogResult result = d.ShowDialog();
			if (result != DialogResult.OK)
				return null;

			return ci[cb.SelectedIndex];
		}

	}
}
