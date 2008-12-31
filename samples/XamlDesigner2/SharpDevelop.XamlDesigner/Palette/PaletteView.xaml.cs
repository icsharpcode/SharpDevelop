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
using System.Windows.Markup;
using SharpDevelop.XamlDesigner.Controls;
using System.Collections;
using Microsoft.Win32;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.Palette
{
	public partial class PaletteView : UserControl, IHasContext
	{
		public PaletteView()
		{
			InitializeComponent();
			InitializeContextMenu();
			AllowDrop = true;
		}

		PaletteData paletteData;
		OpenFileDialog openFileDialog;

		public static readonly DependencyProperty ContextProperty =
			DependencyProperty.Register("Context", typeof(DesignContext), typeof(PaletteView));

		public DesignContext Context
		{
			get { return (DesignContext)GetValue(ContextProperty); }
			set { SetValue(ContextProperty, value); }
		}

		//protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		//{
		//    base.OnPropertyChanged(e);

		//    if (e.Property == ContextProperty) {
		//        if (e.NewValue != null) {
		//            (e.NewValue as DesignContext).AttachedPaletteData = paletteData;
		//        }
		//    }
		//}

		public void LoadData(string xaml)
		{
			paletteData = XamlReader.Parse(xaml) as PaletteData;
			uxDataContextHolder.DataContext = paletteData;
			(Resources["PaletteData"] as ObjectDataProvider).ObjectInstance = DataContext;
		}

		public void ResetData()
		{
			LoadData(DesignResources.GetString("Palette/DefaultPaletteData.xaml"));
		}

		public string SaveData()
		{
			if (paletteData == null) {
				throw new Exception();
			}
			return XamlWriter.Save(paletteData);
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			ProcessDrag(e);
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			ProcessDrag(e);
		}

		protected override void OnDrop(DragEventArgs e)
		{
			ProcessPaths(e.Data.Paths());
		}

		void ProcessDrag(DragEventArgs e)
		{
			e.Effects = DragDropEffects.None;
			e.Handled = true;

			foreach (var path in e.Data.Paths()) {
				if (path.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) ||
					path.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)) {
					e.Effects = DragDropEffects.Copy;
					break;
				}
			}
		}

		void ProcessPaths(IEnumerable<string> paths)
		{
			foreach (var path in paths) {
				if (path.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) ||
					path.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)) {
					paletteData.AddAssembly(path);
				}
			}
		}

		void InitializeContextMenu()
		{
			ContextMenuOpening += new ContextMenuEventHandler(PaletteView_ContextMenuOpening);
			uxInclude.Click += new RoutedEventHandler(uxInclude_Click);
			uxExclude.Click += new RoutedEventHandler(uxExclude_Click);
			uxAddAssembly.Click += new RoutedEventHandler(uxAddAssembly_Click);
			uxRemoveAssembly.Click += new RoutedEventHandler(uxRemoveAssembly_Click);
			uxResetPalette.Click += new RoutedEventHandler(uxResetPalette_Click);
		}

		IEnumerable<PaletteNode> GetSelectedNodes()
		{
			foreach (var item in uxTree.SelectedItems) {
				yield return (item as TreeBoxItemCore).Item as PaletteNode;
			}
		}

		void uxAddAssembly_Click(object sender, RoutedEventArgs e)
		{
			if (openFileDialog == null) {
				openFileDialog = new OpenFileDialog();
				openFileDialog.Filter = "Assemblies (*.dll; *.exe)|*.dll;*.exe";
			} 
			if (openFileDialog.ShowDialog().Value) {
				paletteData.AddAssembly(openFileDialog.FileName);
			}
		}

		void uxResetPalette_Click(object sender, RoutedEventArgs e)
		{
			ResetData();
		}

		void uxInclude_Click(object sender, RoutedEventArgs e)
		{
			paletteData.Include(GetSelectedNodes());
		}

		void uxExclude_Click(object sender, RoutedEventArgs e)
		{
			paletteData.Exclude(GetSelectedNodes());
		}

		void uxRemoveAssembly_Click(object sender, RoutedEventArgs e)
		{
			paletteData.Remove(GetSelectedNodes());
		}

		void PaletteView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			uxInclude.Visibility = Visibility.Collapsed;
			uxExclude.Visibility = Visibility.Collapsed;
			uxRemoveAssembly.Visibility = Visibility.Collapsed;

			bool onlyAssemblies = true;

			foreach (var node in GetSelectedNodes()) {
				var paletteItem = node as PaletteItem;
				if (paletteItem != null) {
					if (paletteItem.IsIncluded) {
						uxExclude.Visibility = Visibility.Visible;
					}
					else {
						uxInclude.Visibility = Visibility.Visible;
					}
					onlyAssemblies = false;
				}
			}

			if (onlyAssemblies) {
				uxRemoveAssembly.Visibility = Visibility.Visible;
			}
		}
	}
}
