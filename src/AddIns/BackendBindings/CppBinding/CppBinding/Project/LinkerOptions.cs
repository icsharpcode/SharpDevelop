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
	using ListEditor = ICSharpCode.CppBinding.Project.OpenStringListEditorEvent<TextBox>;
	
	/// <summary>
	/// Directory settings for c++ application.
	/// </summary>
	public class LinkerOptions : AbstractXmlFormsProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(GetType().Assembly.GetManifestResourceStream(
										"ICSharpCode.CppBinding.Resources.LinkerOptions.xfrm"));
			
			InitializeHelper();
			
			helper.BindString("libraryPathTextBox", "LibraryPath", TextBoxEditMode.EditRawProperty);
			Get<Button>("libraryPath").Click += ListEditor.DirectoriesEditor(this, "libraryPath").Event;
			
			TextBox additionalLibsTextBox = Get<TextBox>("additionalLibs");
			helper.AddBinding(null, new ItemDefinitionGroupBinding<TextBox>(additionalLibsTextBox, "Link", "AdditionalDependencies"));
			Get<Button>("additionalLibs").Click += ListEditor.SymbolsEditor(this, "additionalLibs").Event;
			
			TextBox addModuleTextBox = Get<TextBox>("addModule");
			helper.AddBinding(null, new ItemDefinitionGroupBinding<TextBox>(addModuleTextBox, "Link", "AddModuleNamesToAssembly"));
			Get<Button>("addModule").Click += ListEditor.SymbolsEditor(this, "addModule").Event;

			CheckBox debugInfoCheckBox = Get<CheckBox>("debugInfo");
			helper.AddBinding(null, new CheckBoxItemDefinitionGroupBinding(debugInfoCheckBox, "Link", "GenerateDebugInformation"));			

			TextBox resourceFileTextBox = Get<TextBox>("resourceFile");
			helper.AddBinding(null, new ItemDefinitionGroupBinding<TextBox>(resourceFileTextBox, "Link", "EmbedManagedResourceFile"));
			Get<Button>("resourceFile").Click += ListEditor.SymbolsEditor(this, "resourceFile").Event;
			
			TextBox additionalOptionsTextBox = Get<TextBox>("additionalOptions");
			helper.AddBinding(null, new ItemDefinitionGroupBinding<TextBox>(additionalOptionsTextBox, "Link", "AdditionalOptions"));
		}
	}
}
