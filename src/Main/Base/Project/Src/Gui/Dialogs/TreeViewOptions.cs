// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class GradientHeaderPanel : Label
	{
		public GradientHeaderPanel(int fontSize) : this()
		{
			Font = WinFormsResourceService.LoadFont("Tahoma", fontSize);
		}
		
		public GradientHeaderPanel() : base()
		{
			
			
			ResizeRedraw = true;
			Text = String.Empty;
		}

		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			base.OnPaintBackground(pe);
			Graphics g = pe.Graphics;
			
			using (Brush brush = new LinearGradientBrush(new Point(0, 0), new Point(Width, Height),
			                                             SystemColors.Window, SystemColors.Control)) {
				g.FillRectangle(brush, new Rectangle(0, 0, Width, Height));
			}
		}
	}
	
	/// <summary>
	/// TreeView options are used, when more options will be edited (for something like
	/// IDE Options + Plugin Options)
	/// </summary>
	public class TreeViewOptions : BaseSharpDevelopForm
	{
		protected GradientHeaderPanel optionsPanelLabel;
		
		protected List<IDialogPanel> OptionPanels = new List<IDialogPanel>();
		
		protected Font plainFont = null;
		protected Font boldFont  = null;
		
		protected void AcceptEvent(object sender, EventArgs e)
		{
			foreach (IDialogPanel pane in OptionPanels) {
				if (!pane.ReceiveDialogMessage(DialogMessage.OK)) {
					return;
				}
			}
			DialogResult = DialogResult.OK;
		}
		
		protected void ResetImageIndex(TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes) {
				if (node.Nodes.Count > 0) {
					ResetImageIndex(node.Nodes);
				} else {
					node.ImageIndex         = 2;
					node.SelectedImageIndex = 3;
				}
			}
		}
		
		protected bool b = true;
		
		protected void BeforeExpandNode(object sender, TreeViewCancelEventArgs e)
		{
			if (!b) {
				return;
			}
			b = false;
			((TreeView)ControlDictionary["optionsTreeView"]).BeginUpdate();
			// search first leaf node (leaf nodes have no children)
			TreeNode node = e.Node.FirstNode;
			while (node.Nodes.Count > 0) {
				node = node.FirstNode;
			}
			((TreeView)ControlDictionary["optionsTreeView"]).CollapseAll();
			node.EnsureVisible();
			node.ImageIndex = 3;
			((TreeView)ControlDictionary["optionsTreeView"]).EndUpdate();
			SetOptionPanelTo(node);
			b = true;
		}
		
		protected void BeforeSelectNode(object sender, TreeViewCancelEventArgs e)
		{
			ResetImageIndex(((TreeView)ControlDictionary["optionsTreeView"]).Nodes);
			if (b) {
				CollapseOrExpandNode(e.Node);
			}
		}
		
		protected void HandleClick(object sender, EventArgs e)
		{
			if (((TreeView)ControlDictionary["optionsTreeView"]).GetNodeAt(((TreeView)ControlDictionary["optionsTreeView"]).PointToClient(Control.MousePosition)) == ((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode && b) {
				CollapseOrExpandNode(((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode);
			}
		}
		
		void CollapseOrExpandNode(TreeNode node)
		{
			if (node.Nodes.Count > 0) {  // only folders
				if (node.IsExpanded) {
					node.Collapse();
				}  else {
					node.Expand();
				}
			}
		}
		
		protected void SetOptionPanelTo(TreeNode node)
		{
			IDialogPanelDescriptor descriptor = node.Tag as IDialogPanelDescriptor;
			if (descriptor != null && descriptor.DialogPanel != null && descriptor.DialogPanel.Control != null) {
				if (!OptionPanels.Contains(descriptor.DialogPanel)) {
					descriptor.DialogPanel.Control.Dock = DockStyle.Fill;
					OptionPanels.Add(descriptor.DialogPanel);
				}
				
				descriptor.DialogPanel.ReceiveDialogMessage(DialogMessage.Activated);
				ControlDictionary["optionControlPanel"].Controls.Clear();
				RightToLeftConverter.ConvertRecursive(descriptor.DialogPanel.Control);
				ControlDictionary["optionControlPanel"].Controls.Add(descriptor.DialogPanel.Control);
				optionsPanelLabel.Text = descriptor.Label;
			}
		}
		
		void TreeMouseDown(object sender, MouseEventArgs e)
		{
			TreeNode node = ((TreeView)ControlDictionary["optionsTreeView"]).GetNodeAt(((TreeView)ControlDictionary["optionsTreeView"]).PointToClient(Control.MousePosition));
			if (node != null) {
				if (node.Nodes.Count == 0) ((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode = node;
			}
		}
		
		protected void AddNodes(TreeNodeCollection nodes, IEnumerable<IDialogPanelDescriptor> dialogPanelDescriptors)
		{
			nodes.Clear();
			foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors) {
				TreeNode newNode = new TreeNode(descriptor.Label);
				newNode.Tag = descriptor;
				newNode.NodeFont = plainFont;
				nodes.Add(newNode);
				if (descriptor.ChildDialogPanelDescriptors != null) {
					AddNodes(newNode.Nodes, descriptor.ChildDialogPanelDescriptors);
				}
			}
		}
		
		protected void SelectNode(object sender, TreeViewEventArgs e)
		{
			SetOptionPanelTo(((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode);
		}
		
		protected void InitImageList()
		{
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(new Bitmap(1, 1));
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.SelectionArrow"));
			
			((TreeView)ControlDictionary["optionsTreeView"]).ImageList = imglist;
		}
		
		protected void ShowOpenFolderIcon(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.Nodes.Count > 0) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
			}
		}
		
		protected void ShowClosedFolderIcon(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.Nodes.Count > 0) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
			}
		}
		
		public TreeViewOptions(AddInTreeNode node)
		{
			this.Text = StringParser.Parse("${res:Dialog.Options.TreeViewOptions.DialogName}");

			this.InitializeComponent();
			
			plainFont = new Font(((TreeView)ControlDictionary["optionsTreeView"]).Font, FontStyle.Regular);
			boldFont  = new Font(((TreeView)ControlDictionary["optionsTreeView"]).Font, FontStyle.Bold);
			
			InitImageList();
			
			if (node != null) {
				AddNodes(((TreeView)ControlDictionary["optionsTreeView"]).Nodes, node.BuildChildItems<IDialogPanelDescriptor>(this));
			}
		}
		
		protected void InitializeComponent()
		{
			Owner = WorkbenchSingleton.MainForm;

			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.TreeViewOptionsDialog.xfrm"));
			
			this.optionsPanelLabel             = new GradientHeaderPanel();
			this.optionsPanelLabel.Font        = new Font("Tahoma", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.optionsPanelLabel.TextAlign   = System.Drawing.ContentAlignment.MiddleLeft;
			this.optionsPanelLabel.BorderStyle = BorderStyle.Fixed3D;
			this.optionsPanelLabel.Dock        = DockStyle.Fill;
			ControlDictionary["headerPanel"].Controls.Add(optionsPanelLabel);
			Icon  = null;
			ControlDictionary["okButton"].Click += new EventHandler(AcceptEvent);
			
			((TreeView)ControlDictionary["optionsTreeView"]).Click          += new EventHandler(HandleClick);
			((TreeView)ControlDictionary["optionsTreeView"]).AfterSelect    += new TreeViewEventHandler(SelectNode);
			((TreeView)ControlDictionary["optionsTreeView"]).BeforeSelect   += new TreeViewCancelEventHandler(BeforeSelectNode);
			((TreeView)ControlDictionary["optionsTreeView"]).BeforeExpand   += new TreeViewCancelEventHandler(BeforeExpandNode);
			((TreeView)ControlDictionary["optionsTreeView"]).BeforeExpand   += new TreeViewCancelEventHandler(ShowOpenFolderIcon);
			((TreeView)ControlDictionary["optionsTreeView"]).BeforeCollapse += new TreeViewCancelEventHandler(ShowClosedFolderIcon);
			((TreeView)ControlDictionary["optionsTreeView"]).MouseDown      += new MouseEventHandler(TreeMouseDown);
		}
	}
}
