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
	public class PreprocessorOptions : AbstractXmlFormsProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(GetType().Assembly.GetManifestResourceStream(
										"ICSharpCode.CppBinding.Resources.PreprocessorOptions.xfrm"));
			
			InitializeHelper();
			
			TextBox defineTextBox = Get<TextBox>("define");
			helper.AddBinding(null, new ItemDefinitionGroupBinding<TextBox>(defineTextBox, "ClCompile", "PreprocessorDefinitions"));
			Get<Button>("define").Click += ListEditor.SymbolsEditor(this, "define").Event;
			
			helper.BindString("includePathTextBox", "IncludePath", TextBoxEditMode.EditRawProperty);
			Get<Button>("includePath").Click += ListEditor.DirectoriesEditor(this, "includePath").Event;

			TextBox undefineTextBox = Get<TextBox>("undefine");
			helper.AddBinding(null, new ItemDefinitionGroupBinding<TextBox>(undefineTextBox, "ClCompile", "UndefinePreprocessorDefinitions"));
			Get<Button>("undefine").Click += ListEditor.SymbolsEditor(this, "undefine").Event;
			
			CheckBox undefineAllCheckBox = Get<CheckBox>("undefineAll");
			helper.AddBinding(null, new CheckBoxItemDefinitionGroupBinding(undefineAllCheckBox, "ClCompile", "UndefineAllPreprocessorDefinitions"));
		}
		
	}
}
