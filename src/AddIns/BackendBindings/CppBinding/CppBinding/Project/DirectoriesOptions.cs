// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: trecio
 * Data: 2009-07-08
 * Godzina: 12:07
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using ICSharpCode.Core;
using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CppBinding.Project
{
	/// <summary>
	/// Directory settings for c++ application.
	/// </summary>
	public class DirectoriesOptions : AbstractXmlFormsProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(GetType().Assembly.GetManifestResourceStream(
										"ICSharpCode.CppBinding.Resources.DirectoriesOptions.xfrm"));
			
			InitializeHelper();
			
			helper.BindString("excludePathTextBox", "ExcludePath", TextBoxEditMode.EditRawProperty);
			Get<Button>("excludePath").Click += StringsListEditor<TextBox>("excludePath").Event;

			helper.BindString("executablePathTextBox", "ExecutablePath", TextBoxEditMode.EditRawProperty);
			Get<Button>("executablePath").Click += StringsListEditor<TextBox>("executablePath").Event;
			
			helper.BindString("includePathTextBox", "IncludePath", TextBoxEditMode.EditRawProperty);
			Get<Button>("includePath").Click += StringsListEditor<TextBox>("includePath").Event;
			
			helper.BindString("libraryPathTextBox", "LibraryPath", TextBoxEditMode.EditRawProperty);
			Get<Button>("libraryPath").Click += StringsListEditor<TextBox>("libraryPath").Event;
#if false
			helper.BindString("sourcePathTextBox", "SourcePath", TextBoxEditMode.EditRawProperty);
			Get<Button>("sourcePath").Click += StringsListEditor<TextBox>("sourcePath").Event;

			helper.BindString("referencePathTextBox", "ReferencePath", TextBoxEditMode.EditRawProperty);
			Get<Button>("referencePath").Click += StringsListEditor<TextBox>("referencePath").Event;						
#endif
		}
		
		OpenStringsListEditor<ControlType> StringsListEditor<ControlType>(string controlId) where ControlType : Control {
			return new OpenStringsListEditor<ControlType>(this, controlId);
		}
		
		sealed class OpenStringsListEditor<ControlType> where ControlType : Control {
			public OpenStringsListEditor(DirectoriesOptions parent, string controlId) {
				this.sourceControl = parent.Get<ControlType>(controlId);
				this.listCaption = parent.Get<Label>(controlId).Text;
			}
			
			public void Event(object source, EventArgs evt) {
				using (StringListEditorDialog editor = new StringListEditorDialog()) {
					string[] strings = sourceControl.Text.Split(';');
					editor.BrowseForDirectory = true;
					editor.ListCaption = listCaption;
					editor.TitleText = editor.TitleText = StringParser.Parse("${res:Dialog.ExportProjectToHtml.FolderLabel}");
					editor.LoadList(strings);
					
					if (editor.ShowDialog() == DialogResult.OK) {					
						strings = editor.GetList();
						sourceControl.Text = string.Join(";", strings);
					}
				}
			}
			
			string listCaption;
			ControlType sourceControl;
		}
		
	}
}
