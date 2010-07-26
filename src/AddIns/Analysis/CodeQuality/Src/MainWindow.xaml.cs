using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

using GraphSharp.Controls;
using ICSharpCode.CodeQualityAnalysis.Controls;
using ICSharpCode.Core.Presentation;
using ICSharpCode.WpfDesign.Designer.Controls;
using Microsoft.Win32;
using Mono.Cecil;
using Mono.Cecil.Cil;
using QuickGraph;
using QuickGraph.Collections;

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

			definitionTree.Items.Clear();
			
			MetricsReader = new MetricsReader(fileDialog.FileName);
			definitionTree.ItemsSource = metricsReader.Modules;

			FillMatrix();
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
				matrix.HeaderRows.Add(new MatrixCell<INode>(ns));
				foreach (var type in ns.Types) {
					matrix.HeaderRows.Add(new MatrixCell<INode>(type));
				}
				matrix.HeaderColumns.Add(new MatrixCell<INode>(ns));
				foreach (var type in ns.Types) {
					matrix.HeaderColumns.Add(new MatrixCell<INode>(type));
				}
			}

			matrixControl.Matrix = matrix;
			matrixControl.DrawMatrix();
			matrixControl.DrawTree(metricsReader.MainModule);
		}

		private void definitionTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var item = definitionTree.SelectedItem as INode;

			if (item != null && item.Dependency != null)
			{
				var graph = item.Dependency.BuildDependencyGraph();
				graphLayout.ChangeGraph(graph);
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
					txbTypeInfo.Text = vertex.Node.GetInfo();
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
	}
}
