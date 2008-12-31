using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpDevelop.XamlDesigner.PropertyGrid.Editors
{
	public partial class EditorTemplates : ResourceDictionary
	{
		public EditorTemplates()
		{
			InitializeComponent();
		}

		static EditorTemplates instance = new EditorTemplates();

		static DataTemplate GetEditor(object key)
		{
			return instance[key] as DataTemplate;
		}

		public static DataTemplate BoolEditor
		{
			get { return GetEditor("BoolEditor"); }
		}

		public static DataTemplate NuallableBoolEditor
		{
			get { return GetEditor("NuallableBoolEditor"); }
		}

		public static DataTemplate NumberEditor
		{
			get { return GetEditor("NumberEditor"); }
		}

		public static DataTemplate ThicknessEditor
		{
			get { return GetEditor("ThicknessEditor"); }
		}

		public static DataTemplate HorizontalAlignmentEditor
		{
			get { return GetEditor("HorizontalAlignmentEditor"); }
		}

		public static DataTemplate VerticalAlignmentEditor
		{
			get { return GetEditor("VerticalAlignmentEditor"); }
		}

		public static DataTemplate TextBoxEditor
		{
			get { return GetEditor("TextBoxEditor"); }
		}

		public static DataTemplate ComboBoxEditor
		{
			get { return GetEditor("ComboBoxEditor"); }
		}

		public static DataTemplate EventEditor
		{
			get { return GetEditor("EventEditor"); }
		}

		public static DataTemplate ObjectEditor
		{
			get { return GetEditor("ObjectEditor"); }
		}

		public static DataTemplate CollectionEditor
		{
			get { return GetEditor("CollectionEditor"); }
		}

		public static DataTemplate BrushEditor
		{
			get { return GetEditor("BrushEditor"); }
		}
	}
}
