// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Debugger;
using Debugger.AddIn;
using Debugger.AddIn.Pads.Controls;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class WatchPad : AbstractPadContent
	{
		Grid panel;
		ToolBar toolBar;
		SharpTreeView tree;
		
		public override object Control {
			get { return panel; }
		}
		
		public SharpTreeView Tree {
			get { return tree; }
		}
		
		public SharpTreeNodeCollection Items {
			get { return tree.Root.Children; }
		}
		
		public WatchPad()
		{
			var res = new CommonResources();
			res.InitializeComponent();
			
			panel = new Grid();
			
			toolBar = ToolBarService.CreateToolBar(toolBar, this, "/SharpDevelop/Pads/WatchPad/ToolBar");
			panel.Children.Add(toolBar);
			
			tree = new SharpTreeView();
			tree.Root = new WatchRootNode();
			tree.ShowRoot = false;
			tree.View = (GridView)res["variableGridView"];
			tree.SetValue(GridViewColumnAutoSize.AutoWidthProperty, "50%;25%;25%");
			tree.MouseDoubleClick += delegate(object sender, MouseButtonEventArgs e) {
				if (this.tree.SelectedItem == null) {
					AddWatch(focus: true);
				}
			};
			panel.Children.Add(tree);
			
			panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			panel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

			Grid.SetRow(tree, 1);
			
//			ProjectService.SolutionLoaded  += delegate { LoadNodes(); };
//			SD.ProjectService.CurrentSolution.PreferencesSaving += delegate { SaveNodes(); };
//			LoadNodes();
			
			WindowsDebugger.RefreshingPads += RefreshPad;
			RefreshPad();
		}

		public void AddWatch(string expression = null, bool focus = false)
		{
			var node = MakeNode(expression);
			this.Items.Add(node);
			
			if (focus) {
				var view = tree.View as SharpGridView;
				tree.Dispatcher.BeginInvoke(
					DispatcherPriority.Input, (Action)delegate {
						var container = tree.ItemContainerGenerator.ContainerFromItem(node) as SharpTreeViewItem;
						if (container == null) return;
						var textBox = container.NodeView.FindAncestor<StackPanel>().FindName("name") as AutoCompleteTextBox;
						if (textBox == null) return;
						textBox.FocusEditor();
					});
			}
		}
		
		SharpTreeNodeAdapter MakeNode(string name)
		{
			LoggingService.Info("Evaluating watch: " + name);
			TreeNode node = null;
			try {
				node = new ValueNode(null, name,
				                     () => {
				                     	if (string.IsNullOrWhiteSpace(name))
				                     		return null;
				                     	return WindowsDebugger.Evaluate(name);
				                     });
			} catch (GetValueException e) {
				node = new TreeNode(null, name, e.Message, string.Empty, null);
			}
			node.CanDelete = true;
			node.CanSetName = true;
			node.PropertyChanged += (s, e) => {
				if (e.PropertyName == "Name")
					WindowsDebugger.RefreshPads();
			};
			return node.ToSharpTreeNode();
		}
		
		protected void RefreshPad()
		{
			Process process = WindowsDebugger.CurrentProcess;
			if (process != null && process.IsPaused) {
				var expressions = this.Items.OfType<SharpTreeNodeAdapter>()
					.Select(n => n.Node.Name)
					.ToList();
				this.Items.Clear();
				process.EnqueueForEach(
					Dispatcher.CurrentDispatcher,
					expressions,
					expr => this.Items.Add(MakeNode(expr)),
					expr => this.Items.Add(MakeNode(expr))
				);
			}
		}
	}
	
	class WatchRootNode : SharpTreeNode
	{
		public override bool CanPaste(IDataObject data)
		{
			return data.GetDataPresent(DataFormats.StringFormat);
		}
		
		public override void Paste(IDataObject data)
		{
			var watchValue = data.GetData(DataFormats.StringFormat) as string;
			if (string.IsNullOrEmpty(watchValue)) return;
			
			var pad = SD.Workbench.GetPad(typeof(WatchPad)).PadContent as WatchPad;
			if (pad == null) return;
			
			pad.AddWatch(watchValue);
		}
		
		public override DragDropEffects GetDropEffect(DragEventArgs e, int index)
		{
			return DragDropEffects.Copy;
		}
	}
	
	static class WpfExtensions
	{
		public static T FindAncestor<T>(this DependencyObject d) where T : class
		{
			return AncestorsAndSelf(d).OfType<T>().FirstOrDefault();
		}

		public static IEnumerable<DependencyObject> AncestorsAndSelf(this DependencyObject d)
		{
			while (d != null) {
				yield return d;
				d = VisualTreeHelper.GetParent(d);
			}
		}

	}
}
