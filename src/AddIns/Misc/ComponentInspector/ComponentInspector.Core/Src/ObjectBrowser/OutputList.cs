// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace NoGoop.ObjBrowser
{

	internal class OutputList : ListView
	{
		protected const int             PADDING = 10;
		
		internal OutputList() : base()
		{
			MultiSelect = false;
			FullRowSelect = true;
			HideSelection = false;
			ContextMenu = new ContextMenu();

			MenuItem clear = new MenuItem();
			clear.Text = StringParser.Parse("${res:ComponentInspector.EventLogList.ClearMenuItem}");
			clear.Click += new EventHandler(ClearClick);
			ContextMenu.MenuItems.Add(clear);

			ColumnHeader ch;
			ch = new ColumnHeader();
			ch.Text = StringParser.Parse("${res:ComponentInspector.OutputList.OutputColumnHeader}");
			ch.TextAlign = HorizontalAlignment.Left;
			Columns.Add(ch);

			View = View.Details;

		}

		protected void ClearClick(object sender, EventArgs e)
		{
		}


	}
}
