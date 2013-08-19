using System.IO;
using ICSharpCode.WpfDesign.Designer.OutlineView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
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
using ICSharpCode.WpfDesign.Designer.Services;
using Microsoft.Win32;

namespace ICSharpCode.XamlDesigner
{
	public partial class ToolboxView
	{
		public ToolboxView()
		{
			DataContext = Toolbox.Instance;
			InitializeComponent();

			new DragListener(this).DragStarted += Toolbox_DragStarted;
			uxTreeView.SelectedItemChanged += uxTreeView_SelectedItemChanged;
			uxTreeView.GotKeyboardFocus += uxTreeView_GotKeyboardFocus;
		}

		void uxTreeView_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			PrepareTool(uxTreeView.SelectedItem as ControlNode, false);
		}

		void uxTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			PrepareTool(uxTreeView.SelectedItem as ControlNode, false);
		}

		void Toolbox_DragStarted(object sender, MouseButtonEventArgs e)
		{
			PrepareTool(e.GetDataContext() as ControlNode, true);
		}

		void PrepareTool(ControlNode node, bool drag)
		{
			if (node != null) {
				var tool = new CreateComponentTool(node.Type);
				if (Shell.Instance.CurrentDocument != null) {
					Shell.Instance.CurrentDocument.DesignContext.Services.Tool.CurrentTool = tool;
					if (drag) {
						DragDrop.DoDragDrop(this, tool, DragDropEffects.Copy);
					}
				}
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Delete) {
				Remove();
			}
		}

		void Remove()
		{
			AssemblyNode node = uxTreeView.SelectedItem as AssemblyNode;
			if (node != null) {
				Toolbox.Instance.Remove(node);
			}
		}
		
		private void BrowseForAssemblies_OnClick(object sender, RoutedEventArgs e)
		{
			var dlg = new OpenFileDialog();
			dlg.Filter = "Assemblies (*.dll)|*.dll";
			dlg.Multiselect = true;
			dlg.CheckFileExists = true;
			if (dlg.ShowDialog().Value)
			{
				foreach (var fileName in dlg.FileNames)
				{
					Toolbox.Instance.AddAssembly(fileName);
				}
			}
		}
	}
}
