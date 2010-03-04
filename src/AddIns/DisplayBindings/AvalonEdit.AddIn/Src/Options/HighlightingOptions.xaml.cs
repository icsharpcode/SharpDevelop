// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn.Options
{
	public partial class HighlightingOptions : OptionPanel
	{
		public HighlightingOptions()
		{
			InitializeComponent();
			textEditor.Document.UndoStack.SizeLimit = 0;
			textEditor.Options = CodeEditorOptions.Instance;
			CodeEditorOptions.Instance.BindToTextEditor(textEditor);
		}
		
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
					from name in typeof(HighlightingManager).Assembly.GetManifestResourceNames()
					where name.StartsWith(typeof(HighlightingManager).Namespace + ".Resources.", StringComparison.OrdinalIgnoreCase)
					&& name.EndsWith(".xshd", StringComparison.OrdinalIgnoreCase)
					&& !name.EndsWith("XmlDoc.xshd", StringComparison.OrdinalIgnoreCase)
					let def = LoadBuiltinXshd(name)
					orderby def.Name
					select def
				).ToList();
				// TODO: also find syntax definitions defined in addins
			}
			customizationList = CustomizedHighlightingColor.LoadColors();
			
			languageComboBox.Items.Clear();
			languageComboBox.Items.Add(new XshdSyntaxDefinition { Name = "All languages" });
			foreach (XshdSyntaxDefinition def in allSyntaxDefinitions)
				languageComboBox.Items.Add(def);
			languageComboBox.SelectedIndex = 0;
		}
		
		void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			listBox.Items.Clear();
			if (languageComboBox.SelectedIndex == 0) {
				var namedColors =
					from def in allSyntaxDefinitions
					from color in def.Elements.OfType<XshdColor>()
					where color.ExampleText != null
					group new { DefinitionName = def.Name, Color = color } by color.Name into g
					orderby g.Key
					select g.First();
				
				foreach (var namedColor in namedColors) {
					var def = HighlightingManager.Instance.GetDefinition(namedColor.DefinitionName);
					IHighlightingItem item = new NamedColorHighlightingItem(def, namedColor.Color);
					item = new CustomizedHighlightingItem(customizationList, item, null);
					listBox.Items.Add(item);
					item.PropertyChanged += item_PropertyChanged;
				}
			} else {
				throw new NotImplementedException();
			}
			listBox.SelectedIndex = 0;
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
			((IHighlightingItem)resetButton.DataContext).Reset();
		}
		
		void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdatePreview();
		}
		
		CustomizableHighlightingColorizer colorizer;
		
		void UpdatePreview()
		{
			var item = (IHighlightingItem)listBox.SelectedItem;
			TextView textView = textEditor.TextArea.TextView;
			textView.LineTransformers.Remove(colorizer);
			if (item != null) {
				colorizer = new CustomizableHighlightingColorizer(item.ParentDefinition.MainRuleSet, customizationList);
				textView.LineTransformers.Add(colorizer);
				item.ShowExample(textEditor.TextArea);
			}
		}
	}
	
	sealed class BooleanToBoldConverter : IValueConverter
	{
		public static readonly BooleanToBoldConverter Instance = new BooleanToBoldConverter();
		
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if ((bool)value)
				return FontWeights.Bold;
			else
				return FontWeights.Normal;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
	
	sealed class BooleanToDefaultStringConverter : IValueConverter
	{
		public static readonly BooleanToDefaultStringConverter Instance = new BooleanToDefaultStringConverter();
		
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if ((bool)value)
				return "(Default)";
			else
				return null;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}