// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom;
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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.XamlBinding.PowerToys.Dialogs
{
	/// <summary>
	/// Interaction logic for SelectSourceClassDialog.xaml
	/// </summary>
	public partial class SelectSourceClassDialog : Window
	{
		IEnumerable<ClassWrapper> cache;
		
		struct ClassWrapper {
			public IClass Class { get; set; }
			public string Name { get; set; }
		}
		
		bool HasSelection {
			get { return this.lvClasses.SelectedIndex != -1; }
		}
		
		public IClass SelectedClass {
			get { return HasSelection ? ((ClassWrapper)this.lvClasses.SelectedItem).Class : null; }
		}
		
		public SelectSourceClassDialog()
		{
			InitializeComponent();
			
			this.cache = ProjectService.OpenSolution.Projects
				.Select(p => ParserService.GetProjectContent(p))
				.SelectMany(content => (content == null) ? Enumerable.Empty<IClass>() : content.Classes.Where(aClass => aClass.ClassType == ClassType.Class))
				.Where(myClass => !(myClass.IsAbstract || myClass.IsStatic))
				.Select(c => new ClassWrapper() { Class = c, Name = c.Namespace + "." + c.Name });
			
			this.lvClasses.ItemsSource = this.cache;
		}
		
		void BtnOKClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
		
		void BtnCancelClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
		
		void TxtFilterTextChanged(object sender, TextChangedEventArgs e)
		{
			string text = txtFilter.Text;
			this.lvClasses.ItemsSource = this.cache.AsParallel().Where(item => Filter(text, item)).AsEnumerable();
		}
		
		static bool Filter(string value, ClassWrapper item)
		{
			return item.Name.IndexOf(value, StringComparison.OrdinalIgnoreCase) > -1;
		}
	}
	
	public class SelectionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is int) {
				return ((int)value) != -1;
			}
			
			return false;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is bool)
				return ((bool)value) ? 0 : -1;
			
			return -1;
		}
	}
}
