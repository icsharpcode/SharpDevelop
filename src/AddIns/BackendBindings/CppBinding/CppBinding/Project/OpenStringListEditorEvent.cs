// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: trecio
 * Data: 2009-07-09
 * Godzina: 10:46
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using ICSharpCode.Core;
using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.CppBinding.Project
{
	sealed class OpenStringListEditorEvent<ControlType> where ControlType : Control {
		public OpenStringListEditorEvent(XmlUserControl parent, string controlId) {
			this.sourceControl = parent.Get<ControlType>(controlId);
			this.listCaption = parent.Get<Label>(controlId).Text;
            this.ShowBrowseButton = false;
		}

        public bool ShowBrowseButton { get; set; }
        public string TitleText { get; set; }
		
		public void Event(object source, EventArgs evt) {
			using (StringListEditorDialog editor = new StringListEditorDialog()) {
				string[] strings = sourceControl.Text.Split(';');
				editor.BrowseForDirectory = ShowBrowseButton;
				editor.ListCaption = listCaption;
				if (TitleText != null)
					editor.TitleText = TitleText;
				editor.LoadList(strings);
				
				if (editor.ShowDialog() == DialogResult.OK) {					
					strings = editor.GetList();
					sourceControl.Text = string.Join(";", strings);
				}
			}
		}

		public static OpenStringListEditorEvent<ControlType> DirectoriesEditor(XmlUserControl parent, string controlId)
		{
			OpenStringListEditorEvent<ControlType> editor = new OpenStringListEditorEvent<ControlType>(parent, controlId);
			editor.ShowBrowseButton = true;
			editor.TitleText = StringParser.Parse("${res:Global.Folder}:");
			return editor;
		}

		public static OpenStringListEditorEvent<ControlType> SymbolsEditor(XmlUserControl parent, string controlId)
		{
			OpenStringListEditorEvent<ControlType> editor = new OpenStringListEditorEvent<ControlType>(parent, controlId);
			editor.ShowBrowseButton = false;
			editor.TitleText = StringParser.Parse("${res:ICSharpCode.CppBinding.ProjectOptions.SymbolLabel}:");
			return editor;
		}

		string listCaption;
		ControlType sourceControl;
	}
}
