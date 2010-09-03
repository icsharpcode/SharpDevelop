// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: trecio
 * Data: 2009-07-08
 * Godzina: 20:34
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.CppBinding.Project
{
	/// <summary>
	/// Description of StringsListDialog.
	/// </summary>
	public partial class StringListEditorDialog : Form
	{
		public StringListEditorDialog()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}
		
		public bool BrowseForDirectory {
			get {return stringListEditor.BrowseForDirectory;}
			set {stringListEditor.BrowseForDirectory = value;}
		}
		
		public string ListCaption {
			get {return stringListEditor.ListCaption; }
			set {stringListEditor.ListCaption = value;}
		}

		public string TitleText {
			get {return stringListEditor.TitleText;}
			set {stringListEditor.TitleText = value;}
		}
		
		public string[] GetList() {
			return stringListEditor.GetList();
		}
		
		public void LoadList(IEnumerable<string> list) {
			stringListEditor.LoadList(list);
		}		
	}
}
