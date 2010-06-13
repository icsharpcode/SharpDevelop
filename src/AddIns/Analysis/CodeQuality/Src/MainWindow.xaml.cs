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

        /// <summary>
        /// Fill tree with module, types and methods and TODO: fields
        /// </summary>
        private void FillTree()
        {
            var itemModule = new MetricTreeViewItem() { Header = _metricsReader.MainModule.Name, Dependency = _metricsReader.MainModule };
            definitionTree.Items.Add(itemModule);

            foreach (var ns in _metricsReader.MainModule.Namespaces)
            {
                var nsType = new MetricTreeViewItem() { Header = ns.Name, Dependency = ns };
                itemModule.Items.Add(nsType);

                foreach (var type in ns.Types)
                {
                    var itemType = new MetricTreeViewItem() { Header = type.Name, Dependency = type };
                    nsType.Items.Add(itemType);

                    foreach (var method in type.Methods)
                    {
                        var itemMethod = new MetricTreeViewItem() { Header = method.Name, Dependency = method };
                        itemType.Items.Add(itemMethod);
                    }

                    foreach (var field in type.Fields)
                    {
                        var itemField = new MetricTreeViewItem() { Header = field.Name, Dependency = field };
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
                if (graph != null && graph.VertexCount > 0)
                {
                    graphLayout.Graph = graph;
                }
            }

        }

        private class MetricTreeViewItem : TreeViewItem
        {
            public IDependency Dependency { get; set; }
        }
    }
}
