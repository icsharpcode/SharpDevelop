// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;

namespace ICSharpCode.WixBinding
{
	public class SetupDialogListViewItem : ListViewItem
	{
		string fileName = String.Empty;
		string id = String.Empty;
		
		/// <summary>
		/// Creates a new <see cref="SetupDialogListViewItem"/> class with 
		/// the specified filename and dialog id.
		/// </summary>
		public SetupDialogListViewItem(string fileName, string id)
		{
			this.fileName = fileName;
			Text = id;
			this.id = id;
		}
		
		/// <summary>
		/// Gets the Wix document filename that contains the dialog xml.
		/// </summary>
		public string FileName {
			get {
				return fileName;
			}
		}
		
		/// <summary>
		/// Gets the dialog id.
		/// </summary>
		public string Id {
			get {
				return id;
			}
		}
	}
}
