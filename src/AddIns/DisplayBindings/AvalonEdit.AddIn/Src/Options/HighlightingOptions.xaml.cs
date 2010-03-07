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
			// ensure all definitions from AddIns are registered so that they are available for the example view
			AvalonEditDisplayBinding.RegisterAddInHighlightingDefinitions();
			
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
					from name in typeof(HighlightingManager).Assembly.GetManifestResourceNames().AsParallel()
					where name.StartsWith(typeof(HighlightingManager).Namespace + ".Resources.", StringComparison.OrdinalIgnoreCase)
					&& name.EndsWith(".xshd", StringComparison.OrdinalIgnoreCase)
					&& !name.EndsWith("XmlDoc.xshd", StringComparison.OrdinalIgnoreCase)
					select LoadBuiltinXshd(name)
				).Concat(
					ICSharpCode.Core.AddInTree.BuildItems<AddInTreeSyntaxMode>(SyntaxModeDoozer.Path, null, false).AsParallel()
					.Select(m => m.LoadXshd())
				)
					.Where(def => def.Elements.OfType<XshdColor>().Any(c => c.ExampleText != null))
					.OrderBy(def => def.Name)
					.ToList();
			}
			customizationList = CustomizedHighlightingColor.LoadColors();
			
			languageComboBox.Items.Clear();
			//languageComboBox.Items.Add(new XshdSyntaxDefinition { Name = "All languages" });
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
				IHighlightingDefinition def = HighlightingManager.Instance.GetDefinition(xshd.Name);
				if (def == null) {
					throw new InvalidOperationException("Expected that all XSHDs are registered in default highlighting manager; but highlighting definition was not found");
				} else {
					foreach (XshdColor namedColor in xshd.Elements.OfType<XshdColor>()) {
						if (namedColor.ExampleText != null) {
							IHighlightingItem item = new NamedColorHighlightingItem(def, namedColor);
							item = new CustomizedHighlightingItem(customizationList, item, xshd.Name);
							listBox.Items.Add(item);
							item.PropertyChanged += item_PropertyChanged;
						}
					}
				}
				if (listBox.Items.Count > 0)
					listBox.SelectedIndex = 0;
			}
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