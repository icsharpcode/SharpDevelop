// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.Win32;

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
			CodeEditorOptions.Instance.BindToTextEditor(textEditor);
			textEditor.Options = new TextEditorOptions(CodeEditorOptions.Instance);
			bracketHighlighter = new BracketHighlightRenderer(textEditor.TextArea.TextView);
			foldingManager = FoldingManager.Install(textEditor.TextArea);
			textMarkerService = new TextMarkerService(textEditor.Document);
			textEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
			textEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);
			textEditor.TextArea.TextView.Services.AddService(typeof(ITextMarkerService), textMarkerService);
		}
		
		BracketHighlightRenderer bracketHighlighter;
		FoldingManager foldingManager;
		TextMarkerService textMarkerService;
		ITextMarker marker;
		List<CustomizedHighlightingColor> customizationList;
		
		public const string FoldingControls = "Folding controls";
		public const string FoldingSelectedControls = "Selected folding controls";
		public const string FoldingTextMarkers = "Folding markers";
		
		static SolidColorBrush CreateFrozenBrush(Color color)
		{
			SolidColorBrush brush = new SolidColorBrush(color);
			brush.Freeze();
			return brush;
		}
		
		public static void ApplyToRendering(TextEditor editor, IEnumerable<CustomizedHighlightingColor> customisations)
		{
			bool assignedFoldingMarker = false, assignedSelectedFoldingControls = false, assignedFoldingTextMarkers = false;
			
			editor.ClearValue(FoldingMargin.FoldingMarkerBrushProperty);
			editor.ClearValue(FoldingMargin.FoldingMarkerBackgroundBrushProperty);
			editor.ClearValue(FoldingMargin.SelectedFoldingMarkerBrushProperty);
			editor.ClearValue(FoldingMargin.SelectedFoldingMarkerBackgroundBrushProperty);
			
			FoldingElementGenerator.TextBrush = FoldingElementGenerator.DefaultTextBrush;
			
			bool assignedErrorColor = false;
			bool assignedWarningColor = false;
			bool assignedMessageColor = false;

			foreach (var instance in ErrorPainter.Instances) {
				instance.ErrorColor = Colors.Red;
				instance.WarningColor = Colors.Orange;
				instance.MessageColor = Colors.Blue;
			}
			
			foreach (CustomizedHighlightingColor color in customisations) {
				switch (color.Name) {
					case FoldingControls:
						if (assignedFoldingMarker)
							continue;
						assignedFoldingMarker = true;
						if (color.Foreground != null)
							editor.SetValue(FoldingMargin.FoldingMarkerBrushProperty,
							                CreateFrozenBrush(color.Foreground.Value));
						if (color.Background != null)
							editor.SetValue(FoldingMargin.FoldingMarkerBackgroundBrushProperty,
							                CreateFrozenBrush(color.Background.Value));
						break;
					case FoldingSelectedControls:
						if (assignedSelectedFoldingControls)
							continue;
						assignedSelectedFoldingControls = true;
						if (color.Foreground != null)
							editor.SetValue(FoldingMargin.SelectedFoldingMarkerBrushProperty,
							                CreateFrozenBrush(color.Foreground.Value));
						if (color.Background != null)
							editor.SetValue(FoldingMargin.SelectedFoldingMarkerBackgroundBrushProperty,
							                CreateFrozenBrush(color.Background.Value));
						break;
					case FoldingTextMarkers:
						if (assignedFoldingTextMarkers)
							continue;
						assignedFoldingTextMarkers = true;
						if (color.Foreground != null)
							FoldingElementGenerator.TextBrush = CreateFrozenBrush(color.Foreground.Value);
						break;
					case ErrorPainter.ErrorColorName:
						if (assignedErrorColor)
							continue;
						assignedErrorColor = true;
						if (color.Foreground != null) {
							foreach (var instance in ErrorPainter.Instances) {
								instance.ErrorColor = color.Foreground.Value;
							}
						}
						break;
					case ErrorPainter.WarningColorName:
						if (assignedWarningColor)
							continue;
						assignedWarningColor = true;
						if (color.Foreground != null) {
							foreach (var instance in ErrorPainter.Instances) {
								instance.WarningColor = color.Foreground.Value;
							}
						}
						break;
					case ErrorPainter.MessageColorName:
						if (assignedMessageColor)
							continue;
						assignedMessageColor = true;
						if (color.Foreground != null) {
							foreach (var instance in ErrorPainter.Instances) {
								instance.MessageColor = color.Foreground.Value;
							}
						}
						break;
				}
			}
		}
		
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
			
			CreateDefaultEntries(null, out defaultText, defaultEntries);
			
			languageComboBox.Items.Clear();
			languageComboBox.Items.Add(new XshdSyntaxDefinition { Name = "All languages" });
			foreach (XshdSyntaxDefinition def in allSyntaxDefinitions.Where(d => !d.Name.Equals("XmlDoc", StringComparison.OrdinalIgnoreCase)))
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
				List<IHighlightingItem> list = new List<IHighlightingItem>();
				CreateDefaultEntries(languageComboBox.SelectedIndex == 0 ? null : xshd.Name, out defaultText, list);
				listBox.Items.AddRange(list);
				
				if (languageComboBox.SelectedIndex > 0) {
					// Create entries for all customizable colors in the syntax highlighting definition
					// (but don't do this for the "All languages" pseudo-entry)
					IHighlightingDefinition def = HighlightingManager.Instance.GetDefinition(xshd.Name);
					if (def == null) {
						throw new InvalidOperationException("Expected that all XSHDs are registered in default highlighting manager; but highlighting definition was not found");
					} else {
						var visitor = new ColorVisitor(allSyntaxDefinitions);
						xshd.AcceptElements(visitor);
						foreach (XshdColor namedColor in visitor.foundColors) {
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
		
		class ColorVisitor : IXshdVisitor
		{
			internal readonly List<XshdColor> foundColors = new List<XshdColor>();
			readonly HashSet<XshdSyntaxDefinition> visitedDefinitons = new HashSet<XshdSyntaxDefinition>();
			IList<XshdSyntaxDefinition> allSyntaxDefinitions;
			
			public ColorVisitor(IList<XshdSyntaxDefinition> allSyntaxDefinitions)
			{
				this.allSyntaxDefinitions = allSyntaxDefinitions;
			}
			
			public object VisitRuleSet(XshdRuleSet ruleSet)
			{
				ruleSet.AcceptElements(this);
				return null;
			}
			
			public object VisitColor(XshdColor color)
			{
				foundColors.Add(color);
				return null;
			}
			
			public object VisitKeywords(XshdKeywords keywords)
			{
				return keywords.ColorReference.AcceptVisitor(this);
			}
			
			public object VisitSpan(XshdSpan span)
			{
				if (span.RuleSetReference.InlineElement != null)
					return span.RuleSetReference.AcceptVisitor(this);
				XshdSyntaxDefinition definition = allSyntaxDefinitions.SingleOrDefault(def => def.Name == span.RuleSetReference.ReferencedDefinition);
				if (definition != null && visitedDefinitons.Add(definition))
					foundColors.AddRange(definition.Elements.OfType<XshdColor>());
				return null;
			}
			
			public object VisitImport(XshdImport import)
			{
				if (import.RuleSetReference.InlineElement != null)
					return import.RuleSetReference.AcceptVisitor(this);
				XshdSyntaxDefinition definition = allSyntaxDefinitions.SingleOrDefault(def => def.Name == import.RuleSetReference.ReferencedDefinition);
				if (definition != null && visitedDefinitons.Add(definition))
					foundColors.AddRange(definition.Elements.OfType<XshdColor>());
				return null;
			}
			
			public object VisitRule(XshdRule rule)
			{
				return rule.ColorReference.AcceptVisitor(this);
			}
		}
		
		void CreateDefaultEntries(string language, out IHighlightingItem defaultText, IList<IHighlightingItem> items)
		{
			// Create entry for "default text/background"
			defaultText = new SimpleHighlightingItem(CustomizableHighlightingColorizer.DefaultTextAndBackground, ta => ta.Document.Text = "Normal text") {
				Foreground = SystemColors.WindowTextColor,
				Background = SystemColors.WindowColor
			};
			defaultText = new CustomizedHighlightingItem(customizationList, defaultText, language, canSetFont: false);
			defaultText.PropertyChanged += item_PropertyChanged;
			items.Add(defaultText);
			
			// Create entry for "Selected text"
			IHighlightingItem selectedText = new SimpleHighlightingItem(
				CustomizableHighlightingColorizer.SelectedText,
				ta => {
					ta.Document.Text = "Selected text";
					ta.Selection = Selection.Create(ta, 0, 13);
				})
			{
				Foreground = SystemColors.HighlightTextColor,
				Background = SystemColors.HighlightColor
			};
			selectedText = new CustomizedHighlightingItem(customizationList, selectedText, language, canSetFont: false);
			selectedText.PropertyChanged += item_PropertyChanged;
			items.Add(selectedText);
			
			// Create entry for "Non-printable characters"
			IHighlightingItem nonPrintChars = new SimpleHighlightingItem(
				CustomizableHighlightingColorizer.NonPrintableCharacters,
				ta => {
					ta.Document.Text = "	    \r \r\n \n";
				})
			{
				Foreground = Colors.LightGray
			};
			nonPrintChars = new CustomizedHighlightingItem(customizationList, nonPrintChars, language, canSetFont: false, canSetBackground: false);
			nonPrintChars.PropertyChanged += item_PropertyChanged;
			items.Add(nonPrintChars);
			
			// Create entry for "Line numbers"
			IHighlightingItem lineNumbers = new SimpleHighlightingItem(
				CustomizableHighlightingColorizer.LineNumbers,
				ta => {
					ta.Document.Text = "These are just" + Environment.NewLine +
						"multiple" + Environment.NewLine +
						"lines of" + Environment.NewLine +
						"text";
				})
			{
				Foreground = Colors.Gray
			};
			lineNumbers = new CustomizedHighlightingItem(customizationList, lineNumbers, language, canSetFont: false, canSetBackground: false);
			lineNumbers.PropertyChanged += item_PropertyChanged;
			items.Add(lineNumbers);
			
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
			bracketHighlight = new CustomizedHighlightingItem(customizationList, bracketHighlight, language, canSetFont: false);
			bracketHighlight.PropertyChanged += item_PropertyChanged;
			items.Add(bracketHighlight);
			
			// Create entry for "Folding controls"
			IHighlightingItem foldingControls = new SimpleHighlightingItem(
				FoldingControls,
				ta => {
					ta.Document.Text = "This" + Environment.NewLine +
						"is a folding" + Environment.NewLine +
						"example";
					foldingManager.CreateFolding(0, 10);
				})
			{
				Foreground = Colors.Gray,
				Background = Colors.White
			};
			foldingControls = new CustomizedHighlightingItem(customizationList, foldingControls, language, canSetFont: false);
			foldingControls.PropertyChanged += item_PropertyChanged;
			items.Add(foldingControls);
			
			// Create entry for "Selected folding controls"
			IHighlightingItem selectedFoldingControls = new SimpleHighlightingItem(
				FoldingSelectedControls,
				ta => {
					ta.Document.Text = "This" + Environment.NewLine +
						"is a folding" + Environment.NewLine +
						"example";
					foldingManager.CreateFolding(0, 10);
				})
			{
				Foreground = Colors.Black,
				Background = Colors.White
			};
			selectedFoldingControls = new CustomizedHighlightingItem(customizationList, selectedFoldingControls, language, canSetFont: false);
			selectedFoldingControls.PropertyChanged += item_PropertyChanged;
			items.Add(selectedFoldingControls);
			
			// Create entry for "Folding text markers"
			IHighlightingItem foldingTextMarker = new SimpleHighlightingItem(
				FoldingTextMarkers,
				ta => {
					ta.Document.Text = "This" + Environment.NewLine +
						"is a folding" + Environment.NewLine +
						"example";
					foldingManager.CreateFolding(0, 10).IsFolded = true;
				})
			{
				Foreground = Colors.Gray
			};
			foldingTextMarker = new CustomizedHighlightingItem(customizationList, foldingTextMarker, language, canSetFont: false, canSetBackground: false);
			foldingTextMarker.PropertyChanged += item_PropertyChanged;
			items.Add(foldingTextMarker);
			
			IHighlightingItem linkText = new SimpleHighlightingItem(
				CustomizableHighlightingColorizer.LinkText,
				ta => {
					ta.Document.Text = "http://icsharpcode.net" + Environment.NewLine + "me@example.com";
				})
			{
				Foreground = Colors.Blue,
				Background = Colors.Transparent
			};
			linkText = new CustomizedHighlightingItem(customizationList, linkText, language, canSetFont: false);
			linkText.PropertyChanged += item_PropertyChanged;
			items.Add(linkText);
			
			IHighlightingItem errorMarker = new SimpleHighlightingItem(
				ErrorPainter.ErrorColorName,
				ta => {
					ta.Document.Text = "some error";
					marker = textMarkerService.Create(0, 5);
				})
			{
				Foreground = Colors.Red
			};
			errorMarker = new CustomizedHighlightingItem(customizationList, errorMarker, language, canSetFont: false, canSetBackground: false);
			errorMarker.PropertyChanged += item_PropertyChanged;
			items.Add(errorMarker);
			
			IHighlightingItem warningMarker = new SimpleHighlightingItem(
				ErrorPainter.WarningColorName,
				ta => {
					ta.Document.Text = "some warning";
					marker = textMarkerService.Create(0, 5);
				})
			{
				Foreground = Colors.Orange
			};
			warningMarker = new CustomizedHighlightingItem(customizationList, warningMarker, language, canSetFont: false, canSetBackground: false);
			warningMarker.PropertyChanged += item_PropertyChanged;
			items.Add(warningMarker);
			
			IHighlightingItem messageMarker = new SimpleHighlightingItem(
				ErrorPainter.MessageColorName,
				ta => {
					ta.Document.Text = "some message";
					marker = textMarkerService.Create(0, 5);
				})
			{
				Foreground = Colors.Blue
			};
			messageMarker = new CustomizedHighlightingItem(customizationList, messageMarker, language, canSetFont: false, canSetBackground: false);
			messageMarker.PropertyChanged += item_PropertyChanged;
			items.Add(messageMarker);
			
			IHighlightingItem breakpointMarker = new SimpleHighlightingItem(
				BreakpointBookmark.BreakpointMarker,
				ta => {
					ta.Document.Text = "some code with a breakpoint";
					marker = textMarkerService.Create(0, ta.Document.TextLength);
				})
			{
				Background = BreakpointBookmark.DefaultBackground,
				Foreground = BreakpointBookmark.DefaultForeground
			};
			breakpointMarker = new CustomizedHighlightingItem(customizationList, breakpointMarker, language, canSetFont: false);
			breakpointMarker.PropertyChanged += item_PropertyChanged;
			items.Add(breakpointMarker);
			
			IHighlightingItem currentStatementMarker = new SimpleHighlightingItem(
				CurrentLineBookmark.Name,
				ta => {
					ta.Document.Text = "current statement line";
					marker = textMarkerService.Create(0, ta.Document.TextLength);
				})
			{
				Background = CurrentLineBookmark.DefaultBackground,
				Foreground = CurrentLineBookmark.DefaultForeground
			};
			currentStatementMarker = new CustomizedHighlightingItem(customizationList, currentStatementMarker, language, canSetFont: false);
			currentStatementMarker.PropertyChanged += item_PropertyChanged;
			items.Add(currentStatementMarker);
			
			IHighlightingItem columnRuler = new SimpleHighlightingItem(
				CustomizableHighlightingColorizer.ColumnRuler,
				ta => {
					ta.Document.Text = "some line with a lot of text";
					ta.TextView.Options.ColumnRulerPosition = 15;
					ta.TextView.Options.ShowColumnRuler = true;
				})
			{
				Foreground = Colors.LightGray
			};
			columnRuler = new CustomizedHighlightingItem(customizationList, columnRuler, language, canSetFont: false, canSetBackground: false);
			columnRuler.PropertyChanged += item_PropertyChanged;
			items.Add(columnRuler);
		}

		void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
				ApplyToRendering(textEditor, customizationsForCurrentLanguage);
				var item = (IHighlightingItem)listBox.SelectedItem;
				TextView textView = textEditor.TextArea.TextView;
				foldingManager.Clear();
				textMarkerService.RemoveAll(m => true);
				marker = null;
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
					if (marker != null) {
						switch (item.Name) {
							case ErrorPainter.ErrorColorName:
							case ErrorPainter.WarningColorName:
							case ErrorPainter.MessageColorName:
								marker.MarkerType = TextMarkerType.SquigglyUnderline;
								marker.MarkerColor = item.Foreground;
								break;
							default:
								marker.MarkerType = TextMarkerType.None;
								marker.MarkerColor = Colors.Transparent;
								break;
						}
					}
				}
			}
		}
		
		void ImportButtonClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog {
				Filter = @"All known settings|*.vssettings;*.sdsettings|Visual Studio settings (*.vssettings)|*.vssettings|SharpDevelop settings (*.sdsettings)|*.sdsettings",
				CheckFileExists = true
			};
			if (dialog.ShowDialog() != true)
				return;
			switch (Path.GetExtension(dialog.FileName).ToUpperInvariant()) {
				case ".VSSETTINGS":
					LoadVSSettings(XDocument.Load(dialog.FileName));
					break;
				case ".SDSETTINGS":
					LoadSDSettings(XDocument.Load(dialog.FileName));
					break;
			}
		}
		#region VSSettings
		void LoadVSSettings(XDocument document)
		{
			XElement[] items;
			if (!CheckVersionAndFindCategory(document, out items) || items == null) {
				Core.MessageService.ShowError("${res:Dialog.HighlightingEditor.NotSupportedMessage}");
				return;
			}
			if (!MessageService.AskQuestion("${res:Dialog.HighlightingEditor.OverwriteCustomizationsMessage}"))
				return;
			ResetAllButtonClick(null, null);
			foreach (var item in items) {
				string key = item.Attribute("Name").Value;
				var entry = ParseEntry(item);
				foreach (var sdKey in mapping[key]) {
					IHighlightingItem color;
					if (FindSDColor(sdKey, out color)) {
						color.Bold = entry.Item3;
						color.UseDefaultForeground = !entry.Item1.HasValue;
						if (entry.Item1 != null)
							color.Foreground = entry.Item1.Value;
						color.UseDefaultBackground = !entry.Item2.HasValue;
						if (entry.Item2 != null)
							color.Background = entry.Item2.Value;
					}
				}
			}
		}
		
		readonly List<IHighlightingItem> defaultEntries = new List<IHighlightingItem>();
		IHighlightingItem defaultText;
		
		bool FindSDColor(string sdKey, out IHighlightingItem item)
		{
			string language = null;
			int dot = sdKey.IndexOf('.');
			if (dot > 0) {
				language = sdKey.Substring(0, dot);
				sdKey = sdKey.Substring(dot + 1);
			}
			if ((language == null && languageComboBox.SelectedIndex == 0)
			    || (language == ((XshdSyntaxDefinition)languageComboBox.SelectedItem).Name)) {
				item = listBox.Items.OfType<IHighlightingItem>().FirstOrDefault(i => i.Name == sdKey);
			} else if (language == null) {
				item = defaultEntries.FirstOrDefault(i => i.Name == sdKey);
			} else {
				var def = allSyntaxDefinitions.FirstOrDefault(d => d.Name == language);
				var highlighting = HighlightingManager.Instance.GetDefinition(language);
				item = null;
				if (def != null && highlighting != null) {
					var visitor = new ColorVisitor(allSyntaxDefinitions);
					def.AcceptElements(visitor);
					var color = visitor.foundColors.FirstOrDefault(i => i.Name == sdKey);
					if (color != null) {
						item = new NamedColorHighlightingItem(defaultText, color) { ParentDefinition = highlighting };
						item = new CustomizedHighlightingItem(customizationList, item, language);
					}
				}
			}
			return item != null;
		}
		
		// VS => SD
		static readonly MultiDictionary<string, string> mapping = new MultiDictionary<string, string>(StringComparer.Ordinal) {
			{ "Brace Matching (Rectangle)", BracketHighlightRenderer.BracketHighlight },
			{ "Collapsible Text", FoldingTextMarkers },
			{ "Comment", "VBNET.Comment" },
			{ "Comment", "C#.Comment" },
			{ "Compiler Error", ErrorPainter.ErrorColorName },
			{ "CSS Comment", "CSS.Comment" },
			{ "CSS Keyword", "" },
			{ "CSS Property Name", "CSS.Property" },
			{ "CSS Property Value", "CSS.Value" },
			{ "CSS Selector", "CSS.Selector" },
			{ "CSS String Value", "CSS.String" },
			{ "Excluded Code", "" },
			{ "HTML Attribute Value", "" },
			{ "HTML Attribute", "" },
			{ "HTML Comment", "" },
			{ "HTML Element Name", "" },
			{ "HTML Entity", "" },
			{ "HTML Operator", "" },
			{ "HTML Server-Side Script", "" },
			{ "HTML Tag Delimiter", "" },
			{ "Identifier", "C#.MethodCall" },
			{ "Inactive Selected Text", "" },
			{ "Indicator Margin", "" },
			{ "Keyword", "C#.ThisOrBaseReference" },
			{ "Keyword", "C#.NullOrValueKeywords" },
			{ "Keyword", "C#.Keywords" },
			{ "Keyword", "C#.GotoKeywords" },
			{ "Keyword", "C#.ContextKeywords" },
			{ "Keyword", "C#.ExceptionKeywords" },
			{ "Keyword", "C#.CheckedKeyword" },
			{ "Keyword", "C#.UnsafeKeywords" },
			{ "Keyword", "C#.OperatorKeywords" },
			{ "Keyword", "C#.ParameterModifiers" },
			{ "Keyword", "C#.Modifiers" },
			{ "Keyword", "C#.Visibility" },
			{ "Keyword", "C#.NamespaceKeywords" },
			{ "Keyword", "C#.GetSetAddRemove" },
			{ "Keyword", "C#.TrueFalse" },
			{ "Keyword", "C#.TypeKeywords" },
			{ "Keyword", "C#.ValueTypes" },
			{ "Keyword", "C#.ReferenceTypes" },
			{ "Keyword", "VBNET.DateLiteral" },
			{ "Keyword", "VBNET.Preprocessor" },
			{ "Keyword", "VBNET.DataTypes" },
			{ "Keyword", "VBNET.Operators" },
			{ "Keyword", "VBNET.Constants" },
			{ "Keyword", "VBNET.Keywords" },
			{ "Keyword", "VBNET.FunctionKeywords" },
			{ "Keyword", "VBNET.ContextKeywords" },
			{ "Line Numbers", CustomizableHighlightingColorizer.LineNumbers },
			{ "MarkerFormatDefinition/HighlightedReference", "" },
			{ "Number", "C#.NumberLiteral" },
			{ "Operator", "C#.Punctuation" },
			{ "outlining.collapsehintadornment", "" },
			{ "outlining.square", FoldingControls },
			{ "outlining.square", FoldingSelectedControls },
			{ "outlining.verticalrule", "" },
			{ "Plain Text", "" },
			{ "Plain Text", CustomizableHighlightingColorizer.DefaultTextAndBackground },
			{ "Preprocessor Keyword", "" },
			{ "Preprocessor Keyword", "C#.Preprocessor" },
			{ "Razor Code", "" },
			{ "Script Comment", "" },
			{ "Script Identifier", "" },
			{ "Script Keyword", "" },
			{ "Script Number", "" },
			{ "Script Operator", "" },
			{ "Script String", "" },
			{ "Selected Text", "" },
			{ "Selected Text", CustomizableHighlightingColorizer.SelectedText },
			{ "String", "VBNET.String" },
			{ "String", "C#.String" },
			{ "String(C# @ Verbatim)", "" },
			{ "Syntax Error", "" },
			{ "urlformat", CustomizableHighlightingColorizer.LinkText },
			{ "User Types", "" },
			{ "User Types(Delegates)", "" },
			{ "User Types(Enums)", "" },
			{ "User Types(Interfaces)", "" },
			{ "User Types(Value types)", "" },
			{ "Warning", ErrorPainter.WarningColorName },
			{ "XAML Attribute Quotes", "" },
			{ "XAML Attribute Value", "" },
			{ "XAML Attribute", "" },
			{ "XAML CData Section", "" },
			{ "XAML Comment", "" },
			{ "XAML Delimiter", "" },
			{ "XAML Markup Extension Class", "" },
			{ "XAML Markup Extension Parameter Name", "" },
			{ "XAML Markup Extension Parameter Value", "" },
			{ "XAML Name", "" },
			{ "XAML Text", "" },
			{ "XML Attribute Quotes", "" },
			{ "XML Attribute Value", "XML.AttributeValue" },
			{ "XML Attribute", "XML.AttributeName" },
			{ "XML CData Section", "XML.CData" },
			{ "XML Comment", "XML.Comment" },
			{ "XML Delimiter", "" },
			{ "XML Doc Comment", "C#.DocComment" },
			{ "XML Doc Tag", "C#.KnownDocTags" },
			{ "XML Doc Comment", "VBNET.DocComment" },
			{ "XML Doc Tag", "VBNET.KnownDocTags" },
			{ "XML Name", "XML.XmlTag" },
			{ "XML Name", "XML.XmlDeclaration" },
			{ "XML Name", "XML.DocType" },
			{ "XML Text", "XML." + CustomizableHighlightingColorizer.DefaultTextAndBackground },
		};
		
		Tuple<Color?, Color?, bool> ParseEntry(XElement element)
		{
			Color? fore = null;
			Color? back = null;
			bool isBold = false;
			
			var attribute = element.Attribute("Foreground");
			if (attribute != null)
				fore = ParseColor(attribute.Value);
			attribute = element.Attribute("Background");
			if (attribute != null)
				back = ParseColor(attribute.Value);
			attribute = element.Attribute("BoldFont");
			if (attribute != null)
				isBold = attribute.Value == "Yes";
			
			return Tuple.Create(fore, back, isBold);
		}
		
		Color? ParseColor(string s)
		{
			if (string.IsNullOrWhiteSpace(s))
				return null;
			if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
				s = s.Substring(2);
			if (s.Length < 8)
				return null;
			if (string.CompareOrdinal(s.Substring(0, 2), "02") == 0)
				return null;
			byte r, g, b;
			if (!byte.TryParse(s.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b))
				return Colors.Transparent;
			if (!byte.TryParse(s.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g))
				return Colors.Transparent;
			if (!byte.TryParse(s.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r))
				return Colors.Transparent;
			return Color.FromRgb(r, g, b);
		}
		
		bool CheckVersionAndFindCategory(XDocument document, out XElement[] categoryItems)
		{
			categoryItems = null;
			var node = document.Root;
			var appID = document.Root.Element("ApplicationIdentity");
			var category = document.Root.Descendants("Category").FirstOrDefault(e => e.Attribute("GUID") != null && e.Attribute("GUID").Value == "{A27B4E24-A735-4D1D-B8E7-9716E1E3D8E0}");
			if (category != null)
				categoryItems = category.Descendants("Item").ToArray();
			if (node.Name != "UserSettings" || appID == null || category == null)
				return false;
			return appID.Attribute("version") != null && appID.Attribute("version").Value == "10.0";
		}
		#endregion
		
		#region SDSettings
		void LoadSDSettings(XDocument document)
		{
			var version = document.Root.Attribute("version");
			if (version != null && version.Value != Properties.CurrentVersion.ToString()) {
				Core.MessageService.ShowError("Settings version not supported!");
				return;
			}
			var p = Properties.Load(document.CreateReader());
			customizationList = p.Get("CustomizedHighlightingRules", new List<CustomizedHighlightingColor>());
			LanguageComboBox_SelectionChanged(null, null);
		}
		#endregion

		void ExportButtonClick(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog {
				Filter = @"SharpDevelop settings (*.sdsettings)|*.sdsettings",
			};
			if (dialog.ShowDialog() != true)
				return;
			Save(dialog.FileName);
		}
		
		void Save(string fileName)
		{
			Properties p = new Properties();
			p.Set("CustomizedHighlightingRules", customizationList);
			using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				writer.WriteStartElement("Properties");
				writer.WriteAttributeString("version", Properties.CurrentVersion.ToString());
				p.WriteProperties(writer);
				writer.WriteEndElement();
			}
		}
		
		void ResetAllButtonClick(object sender, RoutedEventArgs e)
		{
			customizationList.Clear();
			LanguageComboBox_SelectionChanged(null, null);
			UpdatePreview();
		}
	}
}
