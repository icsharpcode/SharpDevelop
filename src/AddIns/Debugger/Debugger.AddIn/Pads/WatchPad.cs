// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Debugger;
using Debugger.AddIn;
using Debugger.AddIn.Pads.Controls;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
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
			
			this.tree = new SharpTreeView();
			this.tree.Root = new SharpTreeNode();
			this.tree.ShowRoot = false;
			this.tree.View = (GridView)res["variableGridView"];
			this.tree.ContextMenu = MenuService.CreateContextMenu(this, "/SharpDevelop/Pads/WatchPad/ContextMenu");
			this.tree.MouseDoubleClick += delegate(object sender, MouseButtonEventArgs e) {
				if (this.tree.SelectedItem == null) {
					AddWatchCommand cmd = new AddWatchCommand { Owner = this };
					cmd.Run();
				}
			};
			
			ProjectService.SolutionLoaded  += delegate { LoadNodes(); };
			ProjectService.SolutionClosing += delegate { SaveNodes(); };
			LoadNodes();
			
			WindowsDebugger.RefreshingPads += RefreshPad;
			RefreshPad();
		}

		void LoadNodes()
		{
			if (ProjectService.OpenSolution != null) {
				var props = ProjectService.OpenSolution.Preferences.NestedProperties("Watches");
				foreach (var key in props.Keys) {
					this.Items.Add(new TreeNode(props.Get(key, ""), () => null).ToSharpTreeNode());
				}
			}
		}

		void SaveNodes()
		{
			if (ProjectService.OpenSolution != null) {
				var props = new Properties();
				ProjectService.OpenSolution.Preferences.SetNestedProperties("Watches", props);
				foreach(var node in this.Items.OfType<TreeNode>()) {
					props.Set(node.Name, (object)null);
				}
			}
		}
		
		SharpTreeNodeAdapter MakeNode(string name)
		{
			LoggingService.Info("Evaluating watch: " + name);
			TreeNode node = null;
			try {
				node = new ValueNode(null, name, () => WindowsDebugger.Evaluate(name));
			} catch (GetValueException e) {
				node = new TreeNode(SD.ResourceService.GetImage("Icons.16x16.Error"), name, e.Message, string.Empty, null);
			}
			node.CanDelete = true;
			node.CanSetName = true;
			node.PropertyChanged += (s, e) => { if (e.PropertyName == "Name") WindowsDebugger.RefreshPads(); };
			return node.ToSharpTreeNode();
		}
		
		protected void RefreshPad()
		{
			Process process = WindowsDebugger.CurrentProcess;
			if (process != null) {
				var names = this.Items.OfType<SharpTreeNodeAdapter>().Select(n => n.Node.Name).ToList();
				this.Items.Clear();
				process.EnqueueForEach(
					Dispatcher.CurrentDispatcher,
					names,
					name => this.Items.Add(MakeNode(name))
				);
			}
		}
	}
}
