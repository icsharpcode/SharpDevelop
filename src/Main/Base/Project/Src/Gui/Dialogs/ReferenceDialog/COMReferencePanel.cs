// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Poul Staugaard" email="poul@staugaard.dk"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

using Microsoft.Win32;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class COMReferencePanel : ListView, IReferencePanel
	{
		ISelectReferenceDialog selectDialog;
		
		public COMReferencePanel(ISelectReferenceDialog selectDialog)
		{
			this.selectDialog = selectDialog;
			
			this.Sorting = SortOrder.Ascending;
			
			
			ColumnHeader nameHeader = new ColumnHeader();
			nameHeader.Text  = ResourceService.GetString("Global.Name");
			nameHeader.Width = 240;
			Columns.Add(nameHeader);
			
			ColumnHeader directoryHeader = new ColumnHeader();
			directoryHeader.Text  = ResourceService.GetString("Global.Path");
			directoryHeader.Width =200;
			Columns.Add(directoryHeader);
			
			View = View.Details;
			Dock = DockStyle.Fill;
			FullRowSelect = true;
			
			ItemActivate += delegate { AddReference(); };
			PopulateListView();
		}
		
		public void AddReference()
		{
			foreach (ListViewItem item in SelectedItems) {
				TypeLibrary library = (TypeLibrary)item.Tag;
				selectDialog.AddReference(ReferenceType.Typelib,
				                          library.Name,
				                          library.Path,
				                          library);
			}
		}
		
		void PopulateListView()
		{
			foreach (TypeLibrary typeLib in TypeLibrary.Libraries) {
				ListViewItem newItem = new ListViewItem(new string[] { typeLib.Name, typeLib.Path });
				newItem.Tag = typeLib;
				Items.Add(newItem);
			}
		}
	}
}
