// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Debugger;
using Debugger.AddIn;
using Debugger.AddIn.Pads.Controls;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class WatchPad : AbstractPadContent
	{
		SharpTreeView tree;
		
		public override object Control {
			get { return tree; }
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
			
			tree = new SharpTreeView();
			tree.Root = new SharpTreeNode();
			tree.ShowRoot = false;
			tree.View = (GridView)res["variableGridView"];
			tree.SetValue(GridViewColumnAutoSize.AutoWidthProperty, "50%;25%;25%");
			tree.ContextMenu = MenuService.CreateContextMenu(this, "/SharpDevelop/Pads/WatchPad/ContextMenu");
			tree.MouseDoubleClick += delegate(object sender, MouseButtonEventArgs e) {
				if (this.tree.SelectedItem == null) {
					AddWatchCommand cmd = new AddWatchCommand { Owner = this };
					cmd.Run();
				}
			};
			
//			ProjectService.SolutionLoaded  += delegate { LoadNodes(); };
//			ProjectService.SolutionClosing += delegate { SaveNodes(); };
//			LoadNodes();
			
			WindowsDebugger.RefreshingPads += RefreshPad;
			RefreshPad();
		}

//		void LoadNodes()
//		{
//			if (ProjectService.OpenSolution != null) {
//				var props = ProjectService.OpenSolution.Preferences.NestedProperties("Watches");
//				foreach (var key in props.Keys) {
//					this.Items.Add(new TreeNode(props.Get(key, ""), () => null).ToSharpTreeNode());
//				}
//			}
//		}
//
//		void SaveNodes()
//		{
//			if (ProjectService.OpenSolution != null) {
//				var props = new Properties();
//				ProjectService.OpenSolution.Preferences.SetNestedProperties("Watches", props);
//				foreach(var node in this.Items.OfType<TreeNode>()) {
//					props.Set(node.Name, node.EvalEnabled);
//				}
//			}
//		}
		
		public void AddWatch(string expression = null)
		{
			var node = MakeNode(expression);
			this.Items.Add(node);
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
			if (process != null) {
				var expressions = this.Items.OfType<SharpTreeNodeAdapter>()
					.Select(n => n.Node.Name)
					.ToList();
				this.Items.Clear();
				process.EnqueueForEach(
					Dispatcher.CurrentDispatcher,
					expressions,
					expr => this.Items.Add(MakeNode(expr))
				);
			}
		}
	}
}
