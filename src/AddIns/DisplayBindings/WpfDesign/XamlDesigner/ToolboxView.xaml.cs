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
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.XamlDesigner
{
    public partial class ToolboxView
    {
        public ToolboxView()
        {
			DataContext = Toolbox.Instance;
            InitializeComponent();

            new DragListener(this).DragStarted += new MouseEventHandler(Toolbox_DragStarted);
        }

        void Toolbox_DragStarted(object sender, MouseEventArgs e)
        {
            PrepareTool(e.GetDataContext() as ControlNode, true);
        }

		void uxTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			PrepareTool(uxTreeView.SelectedItem as ControlNode, false);
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
    }
}
