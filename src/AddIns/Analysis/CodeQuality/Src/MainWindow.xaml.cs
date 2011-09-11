// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using GraphSharp.Controls;
using ICSharpCode.CodeQualityAnalysis.Controls;
using ICSharpCode.CodeQualityAnalysis.Utility;
using Microsoft.Win32;

namespace ICSharpCode.CodeQualityAnalysis
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private MetricsReader metricsReader;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		public MetricsReader MetricsReader
		{
			get
			{
				return metricsReader;
			}
			
			set
			{
				metricsReader = value;
				NotifyPropertyChanged("MetricsReader");
			}
		}

		public MainWindow()
		{
			InitializeComponent();
		}
		
		
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		private void btnOpenAssembly_Click(object sender, RoutedEventArgs e)
		{
			
			var dataContext = this.DataContext as MainWindowViewModel;
			
			var fileDialog = new OpenFileDialog
			{
				Filter = "Component Files (*.dll, *.exe)|*.dll;*.exe"
			};

			fileDialog.ShowDialog();

			if (String.IsNullOrEmpty(fileDialog.FileName))
				return;
			
			dataContext.ProgressbarVisible = Visibility.Visible;
			dataContext.AssemblyStatsVisible = Visibility.Hidden;
			dataContext.FileName = System.IO.Path.GetFileName(fileDialog.FileName);
			
			var worker = new BackgroundWorker();
			worker.DoWork += (source, args) => MetricsReader = new MetricsReader(fileDialog.FileName);
			worker.RunWorkerCompleted += (source, args) => {

				dataContext.ProgressbarVisible = Visibility.Hidden;
				dataContext.AssemblyStatsVisible = Visibility.Visible;
				dataContext.MainTabEnable = true;

				Helper.FillTree(definitionTree, metricsReader.MainModule);
				dataContext.MainModule = metricsReader.MainModule;
				FillMatrix();
			};
			
			worker.RunWorkerAsync();
		}
		
		private void btnRelayout_Click(object sender, RoutedEventArgs e)
		{
			graphLayout.Relayout();
		}

		private void btnContinueLayout_Click(object sender, RoutedEventArgs e)
		{
			graphLayout.ContinueLayout();
		}

		private void FillMatrix()
		{
			var matrix = new DependencyMatrix();

			foreach (var ns in metricsReader.MainModule.Namespaces) {
				matrix.AddRow(ns);
				foreach (var type in ns.Types) {
					matrix.AddRow(type);
					foreach (var field in type.Fields)
						matrix.AddRow(field);
					foreach (var method in type.Methods)
						matrix.AddRow(method);
				}
				
				matrix.AddColumn(ns);
				foreach (var type in ns.Types) {
					matrix.AddColumn(type);
					foreach (var field in type.Fields)
						matrix.AddColumn(field);
					foreach (var method in type.Methods)
						matrix.AddColumn(method);
				}
			}

			matrixControl.Matrix = matrix;
			matrixControl.DrawTree(metricsReader.MainModule);
		}
		
		
		private void definitionTree_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
		{
			var item = e.AddedItems[0] as DependecyTreeNode;
			if (item != null && item.INode.Dependency != null)
			{
				definitionTree.SelectedItem = item;
				var graph = item.INode.Dependency.BuildDependencyGraph();
				graphLayout.ChangeGraph(graph);
				var d = this.DataContext as MainWindowViewModel;
				d.MetrixTabEnable = true;
			}
		}
		
	
		private void graphLayout_VertexClick(object sender, MouseButtonEventArgs e)
		{
			var vertexControl = sender as VertexControl;
			if (vertexControl != null)
			{
				var vertex = vertexControl.Vertex as DependencyVertex;
				if (vertex != null)
				{
					var d = this.DataContext as MainWindowViewModel;
					d.TypeInfo = vertex.Node.GetInfo();
				}
			}
		}

		private void btnResetGraph_Click(object sender, RoutedEventArgs e)
		{
			graphLayout.ResetGraph();
		}

		private void btnSaveImageGraph_Click(object sender, RoutedEventArgs e)
		{
		
			var fileDialog = new SaveFileDialog()
			{
				Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|BMP (*.bmp)|*.bmp|TIFF (.tiff)|*.tiff"
			};

			fileDialog.ShowDialog();

			if (String.IsNullOrEmpty(fileDialog.FileName))
				return;

			// render it
			var renderBitmap = new RenderTargetBitmap((int)graphLayout.ActualWidth,
			                                          (int)graphLayout.ActualHeight,
			                                          96d,
			                                          96d,
			                                          PixelFormats.Default);
			renderBitmap.Render(graphLayout);

			using (var outStream = new FileStream(fileDialog.FileName, FileMode.Create))
			{
				BitmapEncoder encoder;

				switch (fileDialog.FilterIndex)
				{
					case 1:
						encoder = new PngBitmapEncoder();
						break;
					case 2:
						encoder = new JpegBitmapEncoder();
						break;
					case 3:
						encoder = new GifBitmapEncoder();
						break;
					case 4:
						encoder = new BmpBitmapEncoder();
						break;
					case 5:
						encoder = new TiffBitmapEncoder();
						break;
					default:
						encoder = new PngBitmapEncoder();
						break;
				}
				
				encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
				encoder.Save(outStream);
			}
		}

		private void MetricLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var comboBoxItem = cbxMetrixLevel.SelectedItem as ComboBoxItem;
			if (comboBoxItem == null) return;

			cbxMetrics.Items.Clear();

			var content = comboBoxItem.Content.ToString();

			if (content == "Assembly") {
				//cbxMetrics.Items.Add(new ComboBoxItem { Content = });
			} else if (content == "Namespace") {

			} else if (content == "Type") {

			} else if (content == "Field") {

			} else if (content == "Method") {
				cbxMetrics.Items.Add(new ComboBoxItem { Content = "IL instructions" });
				cbxMetrics.Items.Add(new ComboBoxItem { Content = "Cyclomatic Complexity" });
				cbxMetrics.Items.Add(new ComboBoxItem { Content = "Variables" });
			}
		}

		
		private void Metrics_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var levelItem = cbxMetrixLevel.SelectedItem as ComboBoxItem;
			if (levelItem == null) return;

			var level = levelItem.Content.ToString();

			var metricItem = cbxMetrics.SelectedItem as ComboBoxItem;
			if (metricItem == null) return;

			var metric = metricItem.Content.ToString();

			// TODO: Redone this with enums or some smarter way

			if (level == "Assembly") {
				//cbxMetrics.Items.Add(new ComboBoxItem { Content = });
			} else if (level == "Namespace") {

			} else if (level == "Type") {

			} else if (level == "Field") {

			} else if (level == "Method") {
				var r =  from ns in MetricsReader.MainModule.Namespaces
									  from type in ns.Types
									  from method in type.Methods
									  select method;
				treemap.ItemsSource = from ns in MetricsReader.MainModule.Namespaces
									  from type in ns.Types
									  from method in type.Methods
									  select method;

				if (metric == "IL instructions")
					treemap.ItemDefinition.ValuePath = "Instructions.Count";

				if (metric == "Cyclomatic Complexity")
					treemap.ItemDefinition.ValuePath = "CyclomaticComplexity";

				if (metric == "Variables")
					treemap.ItemDefinition.ValuePath = "Variables";
			}
		}
	}
}
