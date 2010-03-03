// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Xml;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn.Options
{
	public partial class HighlightingOptions : OptionPanel
	{
		public HighlightingOptions()
		{
			InitializeComponent();
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
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			var syntaxDefinitions =
				from name in typeof(HighlightingManager).Assembly.GetManifestResourceNames()
				where name.StartsWith(typeof(HighlightingManager).Namespace + ".Resources.", StringComparison.OrdinalIgnoreCase)
				where name.EndsWith(".xshd", StringComparison.OrdinalIgnoreCase)
				select LoadBuiltinXshd(name);
			var namedColors =
				from def in syntaxDefinitions
				from color in def.Elements.OfType<XshdColor>()
				where color.ExampleText != null
				group color by color.Name into g
				orderby g.Key
				select g.First();
			customizationList = CustomizedHighlightingColor.LoadColors();
			foreach (XshdColor namedColor in namedColors) {
				listBox.Items.Add(new CustomizedHighlightingItem(customizationList, new NamedColorHighlightingItem(namedColor), null));
			}
			if (listBox.Items.Count > 0)
				listBox.SelectedIndex = 0;
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