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
    public partial class MainWindow : Window
    {
        private MetricsReader _metricsReader;

        public MainWindow()
        {
            InitializeComponent();
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
        
            _metricsReader = new MetricsReader(fileDialog.FileName);

            FillTree();
        }
        
        private void btnRelayout_Click(object sender, RoutedEventArgs e)
        {
            graphLayout.Relayout();
        }

        private void btnContinueLayout_Click(object sender, RoutedEventArgs e)
        {
            graphLayout.ContinueLayout();
        }

        /// <summary>
        /// Fill tree with module, types and methods and fields
        /// </summary>
        private void FillTree()
        {
            var itemModule = new MetricTreeViewItem
                                 {
                                     Text = _metricsReader.MainModule.Name,
                                     Dependency = _metricsReader.MainModule,
                                     Icon = NodeIconService.GetIcon(_metricsReader.MainModule)
                                 };

            definitionTree.Items.Add(itemModule);
            
            foreach (var ns in _metricsReader.MainModule.Namespaces)
            {
                var nsType = new MetricTreeViewItem
                                 {
                                     Text = ns.Name, 
                                     Dependency = ns, 
                                     Icon = NodeIconService.GetIcon(ns)
                                 };

                itemModule.Items.Add(nsType);

                foreach (var type in ns.Types)
                {
                    var itemType = new MetricTreeViewItem
                                       {
                                           Text = type.Name, 
                                           Dependency = type, 
                                           Icon = NodeIconService.GetIcon(type)
                                       };

                    nsType.Items.Add(itemType);

                    foreach (var method in type.Methods)
                    {
                        var itemMethod = new MetricTreeViewItem
                                             {
                                                 Text = method.Name,
                                                 Dependency = method,
                                                 Icon = NodeIconService.GetIcon(method)
                                             };

                        itemType.Items.Add(itemMethod);
                    }

                    foreach (var field in type.Fields)
                    {
                        var itemField = new MetricTreeViewItem
                                            {
                                                Text = field.Name,
                                                Dependency = field,
                                                Icon = NodeIconService.GetIcon(field)
                                            };

                        itemType.Items.Add(itemField);
                    }
                }
            }
        }

        private void definitionTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = definitionTree.SelectedItem as MetricTreeViewItem;

            if (item != null)
            {
                // would be better inherit from TreeViewItem and add reference into it
                // will do it later or will use another tree maybe tree from SharpDevelop
                string name = item.Header.ToString();
                txbTypeInfo.Text = "Infobox: \n" + name;
                /*var type = (from n in this._metricsReader.MainModule.Namespaces
                            from t in n.Types
                            where t.Name == name
                            select t).SingleOrDefault();*/

                var graph = item.Dependency.BuildDependencyGraph();

                try
                {
                    if (graph != null && graph.VertexCount > 0)
                    {
                        graphLayout.Graph = graph;
                    }
                }
                catch
                {
                } // ignore it if it fails
            }

        }

        private class MetricTreeViewItem : TreeViewItem
        {
            private readonly Image _iconControl;
            private readonly TextBlock _textControl;
            private BitmapSource _bitmap;

            public IDependency Dependency { get; set; }

            /// <summary>
            /// Gets or sets the text content for the item
            /// </summary>
            public string Text
            {
                get
                {
                    return _textControl.Text;   
                }
                set
                {
                    _textControl.Text = value;
                }
            }

            /// <summary>
            /// Gets or sets the icon for the item
            /// </summary>
            public BitmapSource Icon
            {
                get
                {
                    return _bitmap;
                } 
                set
                {
                    _iconControl.Source = _bitmap = value;
                }
            }
            
            public MetricTreeViewItem()
            {
                var stack = new StackPanel
                                {
                                    Orientation = Orientation.Horizontal
                                };

                Header = stack;

                _iconControl = new Image
                                    {
                                        Margin = new Thickness(0, 0, 5, 0)
                                    };

                _textControl = new TextBlock()
                                   {
                                       VerticalAlignment = VerticalAlignment.Center
                                   };

                stack.Children.Add(_iconControl);
                stack.Children.Add(_textControl);
            }
        }
    }
}
