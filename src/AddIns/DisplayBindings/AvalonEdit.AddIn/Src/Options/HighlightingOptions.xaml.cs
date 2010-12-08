// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn.Options
{
	public partial class HighlightingOptions : OptionPanel
	{
		public HighlightingOptions()
		{
			// ensure all definitions from AddIns are registered so that they are available for the example view
			AvalonEditDisplayBinding.RegisterAddInHighlightingDefinitions();
			
			InitializeComponent();
			textEditor.Document.UndoStack.SizeLimit = 0;
			textEditor.Options = CodeEditorOptions.Instance;
			bracketHighlighter = new BracketHighlightRenderer(textEditor.TextArea.TextView);
			
			CodeEditorOptions.Instance.BindToTextEditor(textEditor);
		}
		
		BracketHighlightRenderer bracketHighlighter;
		
		List<CustomizedHighlightingColor> customizationList;
		
		XshdSyntaxDefinition LoadBuiltinXshd(string name)
		{
			using (Stream s = typeof(HighlightingManager).Assembly.GetManifestResourceStream(name)) {
				using (XmlTextReader reader = new XmlTextReader(s)) {
					return HighlightingLoader.LoadXshd(reader);
				}
			}
		}
		
		List<XshdSyntaxDefinition> allSyntaxDefinitions;
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			if (allSyntaxDefinitions == null) {
				allSyntaxDefinitions = (
					from name in typeof(HighlightingManager).Assembly.GetManifestResourceNames().AsParallel()
					where name.StartsWith(typeof(HighlightingManager).Namespace + ".Resources.", StringComparison.OrdinalIgnoreCase)
					&& name.EndsWith(".xshd", StringComparison.OrdinalIgnoreCase)
					&& !name.EndsWith("XmlDoc.xshd", StringComparison.OrdinalIgnoreCase)
					select LoadBuiltinXshd(name)
				).Concat(
					ICSharpCode.Core.AddInTree.BuildItems<AddInTreeSyntaxMode>(SyntaxModeDoozer.Path, null, false).AsParallel()
					.Select(m => m.LoadXshd())
				)
					//.Where(def => def.Elements.OfType<XshdColor>().Any(c => c.ExampleText != null))
					.OrderBy(def => def.Name)
					.ToList();
			}
			customizationList = CustomizedHighlightingColor.LoadColors();
			
			languageComboBox.Items.Clear();
			languageComboBox.Items.Add(new XshdSyntaxDefinition { Name = "All languages" });
			foreach (XshdSyntaxDefinition def in allSyntaxDefinitions)
				languageComboBox.Items.Add(def);
			if (allSyntaxDefinitions.Count > 0)
				languageComboBox.SelectedIndex = 0;
		}
		
		void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			listBox.Items.Clear();
			XshdSyntaxDefinition xshd = (XshdSyntaxDefinition)languageComboBox.SelectedItem;
			if (xshd != null) {
				IHighlightingItem defaultText;
				CreateDefaultEntries(languageComboBox.SelectedIndex == 0 ? null : xshd.Name, out defaultText);
				
				if (languageComboBox.SelectedIndex > 0) {
					// Create entries for all customizable colors in the syntax highlighting definition
					// (but don't do this for the "All languages" pseudo-entry)
					IHighlightingDefinition def = HighlightingManager.Instance.GetDefinition(xshd.Name);
					if (def == null) {
						throw new InvalidOperationException("Expected that all XSHDs are registered in default highlighting manager; but highlighting definition was not found");
					} else {
						foreach (XshdColor namedColor in xshd.Elements.OfType<XshdColor>()) {
							if (namedColor.ExampleText != null) {
								IHighlightingItem item = new NamedColorHighlightingItem(defaultText, namedColor) { ParentDefinition = def };
								item = new CustomizedHighlightingItem(customizationList, item, xshd.Name);
								listBox.Items.Add(item);
								item.PropertyChanged += item_PropertyChanged;
							}
						}
					}
				}
				if (listBox.Items.Count > 0)
					listBox.SelectedIndex = 0;
			}
		}
		
		void CreateDefaultEntries(string language, out IHighlightingItem defaultText)
		{
			// Create entry for "default text/background"
			defaultText = new SimpleHighlightingItem(CustomizableHighlightingColorizer.DefaultTextAndBackground, ta => ta.Document.Text = "Normal text") {
				Foreground = SystemColors.WindowTextColor,
				Background = SystemColors.WindowColor
			};
			defaultText = new CustomizedHighlightingItem(customizationList, defaultText, null, canSetFont: false);
			if (language != null)
				defaultText = new CustomizedHighlightingItem(customizationList, defaultText, language, canSetFont: false);
			defaultText.PropertyChanged += item_PropertyChanged;
			listBox.Items.Add(defaultText);
			
			// Create entry for "Selected text"
			IHighlightingItem selectedText = new SimpleHighlightingItem(
				CustomizableHighlightingColorizer.SelectedText,
				ta => {
					ta.Document.Text = "Selected text";
					ta.Selection = new SimpleSelection(0, 13);
				})
			{
				Foreground = SystemColors.HighlightTextColor,
				Background = SystemColors.HighlightColor
			};
			selectedText = new CustomizedHighlightingItem(customizationList, selectedText, null, canSetFont: false);
			if (language != null)
				selectedText = new CustomizedHighlightingItem(customizationList, selectedText, language, canSetFont: false);
			selectedText.PropertyChanged += item_PropertyChanged;
			listBox.Items.Add(selectedText);
			
			// Create entry for "Non-printable characters"
			IHighlightingItem nonPrintChars = new SimpleHighlightingItem(
				CustomizableHighlightingColorizer.NonPrintableCharacters,
				ta => {
					ta.Document.Text = "	    \r \r\n \n";
				})
			{
				Foreground = Colors.LightGray
			};
			nonPrintChars = new CustomizedHighlightingItem(customizationList, nonPrintChars, null, canSetFont: false, canSetBackground: false);
			if (language != null)
				nonPrintChars = new CustomizedHighlightingItem(customizationList, nonPrintChars, language, canSetFont: false);
			nonPrintChars.PropertyChanged += item_PropertyChanged;
			listBox.Items.Add(nonPrintChars);
			
			// Create entry for "Bracket highlight"
			IHighlightingItem bracketHighlight = new SimpleHighlightingItem(
				BracketHighlightRenderer.BracketHighlight,
				ta => {
					ta.Document.Text = "(simple) example";
					XshdSyntaxDefinition xshd = (XshdSyntaxDefinition)languageComboBox.SelectedItem;
					if (xshd == null)
						return;
					var customizationsForCurrentLanguage = customizationList.Where(c => c.Language == null || c.Language == xshd.Name);
					BracketHighlightRenderer.ApplyCustomizationsToRendering(bracketHighlighter, customizationsForCurrentLanguage);
					bracketHighlighter.SetHighlight(new BracketSearchResult(0, 1, 7, 1));
				})
			{
				Foreground = BracketHighlightRenderer.DefaultBorder,
				Background = BracketHighlightRenderer.DefaultBackground
			};
			bracketHighlight = new CustomizedHighlightingItem(customizationList, bracketHighlight, null, canSetFont: false);
			if (language != null)
				bracketHighlight = new CustomizedHighlightingItem(customizationList, bracketHighlight, language, canSetFont: false);
			bracketHighlight.PropertyChanged += item_PropertyChanged;
			listBox.Items.Add(bracketHighlight);
		}

		void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdatePreview();
		}
		
		public override bool SaveOptions()
		{
			CustomizedHighlightingColor.SaveColors(customizationList);
			return base.SaveOptions();
		}
		
		void ResetButtonClick(object sender, RoutedEventArgs e)
		{
			IHighlightingItem item = resetButton.DataContext as IHighlightingItem;
			if (item != null)
				item.Reset();
		}
		
		void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdatePreview();
		}
		
		CustomizableHighlightingColorizer colorizer;
		
		void UpdatePreview()
		{
			XshdSyntaxDefinition xshd = (XshdSyntaxDefinition)languageComboBox.SelectedItem;
			if (xshd != null) {
				var customizationsForCurrentLanguage = customizationList.Where(c => c.Language == null || c.Language == xshd.Name);
				CustomizableHighlightingColorizer.ApplyCustomizationsToDefaultElements(textEditor, customizationsForCurrentLanguage);
				var item = (IHighlightingItem)listBox.SelectedItem;
				TextView textView = textEditor.TextArea.TextView;
				textView.LineTransformers.Remove(colorizer);
				colorizer = null;
				if (item != null) {
					if (item.ParentDefinition != null) {
						colorizer = new CustomizableHighlightingColorizer(item.ParentDefinition.MainRuleSet, customizationsForCurrentLanguage);
						textView.LineTransformers.Add(colorizer);
					}
					textEditor.Select(0, 0);
					bracketHighlighter.SetHighlight(null);
					item.ShowExample(textEditor.TextArea);
				}
			}
		}
	}
}
