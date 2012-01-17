// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
			var fileDialog = new OpenFileDialog
			{
				Filter = "Component Files (*.dll, *.exe)|*.dll;*.exe"
			};

			fileDialog.ShowDialog();

			if (String.IsNullOrEmpty(fileDialog.FileName))
				return;
			
			var dataContext = this.DataContext as MainWindowViewModel;
			dataContext.ProgressbarVisible = Visibility.Visible;
			dataContext.AssemblyStatsVisible = Visibility.Hidden;
			dataContext.FileName = System.IO.Path.GetFileName(fileDialog.FileName);
			
			var worker = new BackgroundWorker();
			worker.DoWork += (source, args) => MetricsReader = new MetricsReader(fileDialog.FileName);
			worker.RunWorkerCompleted += (source, args) => {

				dataContext.ProgressbarVisible = Visibility.Hidden;
				dataContext.AssemblyStatsVisible = Visibility.Visible;
				dataContext.MainTabEnable = true;
				
				if (args.Error != null) {
					ICSharpCode.Core.MessageService.ShowException(args.Error);
					return;
				}

				Helper.FillTree(definitionTree, metricsReader.Assembly);
				dataContext.MainModule = metricsReader.Assembly;                        
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

			foreach (var ns in metricsReader.Assembly.Namespaces) {
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
			matrixControl.DrawTree(metricsReader.Assembly);
		}
		
		
		private void definitionTree_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
		{
			var item = e.AddedItems[0] as DependecyTreeNode;
			if (item != null && item.INode.Dependency != null)
			{
				definitionTree.SelectedItem = item;
				var graph = item.INode.Dependency.BuildDependencyGraph();
				graphLayout.ChangeGraph(graph);
				var viewModel = this.DataContext as MainWindowViewModel;
				//testhalber
				viewModel.SelectedTreeNode = item.INode;
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
/*
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

		*/
		/*
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
//				var r =  from ns in MetricsReader.MainModule.Namespaces
//									  from type in ns.Types
//									  from method in type.Methods
//									  select method;
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
		*/
		
		
		//http://social.msdn.microsoft.com/Forums/en-MY/wpf/thread/798e100e-249d-413f-a501-50d1db680b94
		
		void TreeMaps_Loaded(object sender, RoutedEventArgs e)
		{
			ItemsControl itemsControl = sender as ItemsControl;
			
			if (itemsControl!=null)
			{
				itemsControl.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
			}
		}
		
		void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
		{
			ItemContainerGenerator icg = sender as ItemContainerGenerator;
			if (icg!=null&&icg.Status==GeneratorStatus.ContainersGenerated)
			{
				//Do what you want
			//	Mouse.OverrideCursor = Cursors.Wait;
				icg.StatusChanged -= ItemContainerGenerator_StatusChanged;
			}
		}
		
		void MenuClose_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
