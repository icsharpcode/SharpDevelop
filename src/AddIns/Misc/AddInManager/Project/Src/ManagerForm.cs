/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 26.11.2005
 * Time: 14:53
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.AddInManager
{
	public class ManagerForm : System.Windows.Forms.Form
	{
		static ManagerForm instance;
		
		public static ManagerForm Instance {
			get {
				return instance;
			}
		}
		
		public static void ShowForm()
		{
			if (instance == null) {
				instance = new ManagerForm();
				instance.Owner = ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm;
				instance.Show();
			} else {
				instance.Activate();
			}
		}
		
		public ManagerForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			ICSharpCode.SharpDevelop.Gui.FormLocationHelper.Apply(this, "AddInManager.WindowBounds", true);
			
			Stack<AddInControl> stack = new Stack<AddInControl>();
			int index = 0;
			AddInControl ctl;
			
			foreach (AddIn addIn in AddInTree.AddIns) {
				string identity = addIn.Manifest.PrimaryIdentity;
				if (identity == null || identity == "SharpDevelop") // || identity == "ICSharpCode.AddInManager"
					continue;
				ctl = new AddInControl(addIn);
				ctl.Dock = DockStyle.Top;
				ctl.TabIndex = index++;
				stack.Push(ctl);
				ctl.Enter += OnControlEnter;
				ctl.Click += OnControlClick;
			}
			while (stack.Count > 0) {
				splitContainer.Panel1.Controls.Add(stack.Pop());
			}
			splitContainer.Panel2Collapsed = true;
		}
		
		void OnControlClick(object sender, EventArgs e)
		{
			// clicking again on already focused item:
			// remove selection of other items / or with Ctrl: toggle selection
			if (((Control)sender).Focused)
				OnControlEnter(sender, e);
		}
		
		AddInControl oldFocus;
		bool ignoreFocusChange;
		
		void OnControlEnter(object sender, EventArgs e)
		{
			if (ignoreFocusChange)
				return;
			bool ctrl = (ModifierKeys & Keys.Control) == Keys.Control;
			if ((ModifierKeys & Keys.Shift) == Keys.Shift && sender != oldFocus) {
				bool sel = false;
				foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
					if (ctl == sender || ctl == oldFocus) {
						sel = !sel;
						ctl.Selected = true;
					} else {
						if (sel || !ctrl) {
							ctl.Selected = sel;
						}
					}
				}
			} else if (ctrl) {
				foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
					if (ctl == sender)
						ctl.Selected = !ctl.Selected;
				}
				oldFocus = (AddInControl)sender;
			} else {
				foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
					ctl.Selected = ctl == sender;
				}
				oldFocus = (AddInControl)sender;
			}
			UpdateActionBox();
		}
		
		List<AddIn> selected;
		AddInAction selectedAction;
		
		void UpdateActionBox()
		{
			ignoreFocusChange = true;
			selected = new List<AddIn>();
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				if (ctl.Selected)
					selected.Add(ctl.AddIn);
			}
			splitContainer.Panel2Collapsed = selected.Count == 0;
			if (selected.Count > 0) {
				dependencyTable.Visible = false;
				uninstallButton.Enabled = false;
				runActionButton.Enabled = false;
				runActionButton.Visible = true;
				uninstallButton.Visible = true;
				
				bool allEnabled  = true;
				bool allDisabled = true;
				foreach (AddIn addIn in selected) {
					allEnabled  &= addIn.Action == AddInAction.Enable;
					allDisabled &= addIn.Action == AddInAction.Disable;
				}
				if (allEnabled) {
					selectedAction = AddInAction.Disable;
					actionGroupBox.Text = runActionButton.Text = "Disable";
					actionDescription.Text = "Disables the selected AddIns.";
					ShowDependencies(selected, false);
					uninstallButton.Enabled = runActionButton.Enabled = !dependencyTable.Visible;
				} else if (allDisabled) {
					selectedAction = AddInAction.Enable;
					actionGroupBox.Text = runActionButton.Text = "Enable";
					actionDescription.Text = "Enables the selected AddIns.";
					ShowDependencies(selected, true);
					runActionButton.Enabled = !dependencyTable.Visible;
					uninstallButton.Enabled = true;
				} else {
					actionGroupBox.Text = "";
					actionDescription.Text = "AddIns with multiple states are selected";
					runActionButton.Visible = false;
					uninstallButton.Visible = false;
				}
			}
			ignoreFocusChange = false;
		}
		
		void ShowDependencies(List<AddIn> addIns, bool enable)
		{
			
		}
		
		void CloseButtonClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void RunActionButtonClick(object sender, EventArgs e)
		{
			if (selectedAction == AddInAction.Disable) {
				ICSharpCode.Core.AddInManager.Disable(selected);
			} else if (selectedAction == AddInAction.Enable) {
				ICSharpCode.Core.AddInManager.Enable(selected);
			}
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				ctl.Invalidate();
			}
			UpdateActionBox();
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			instance = null;
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.topPanel = new System.Windows.Forms.Panel();
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.installButton = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.actionGroupBox = new System.Windows.Forms.GroupBox();
			this.actionFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.actionDescription = new System.Windows.Forms.Label();
			this.dependencyTable = new System.Windows.Forms.TableLayoutPanel();
			this.runActionButton = new System.Windows.Forms.Button();
			this.uninstallButton = new System.Windows.Forms.Button();
			this.bottomPanel.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.actionGroupBox.SuspendLayout();
			this.actionFlowLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// topPanel
			// 
			this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.topPanel.Location = new System.Drawing.Point(0, 0);
			this.topPanel.Name = "topPanel";
			this.topPanel.Size = new System.Drawing.Size(460, 33);
			this.topPanel.TabIndex = 0;
			// 
			// bottomPanel
			// 
			this.bottomPanel.Controls.Add(this.installButton);
			this.bottomPanel.Controls.Add(this.closeButton);
			this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomPanel.Location = new System.Drawing.Point(0, 355);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size(460, 35);
			this.bottomPanel.TabIndex = 2;
			// 
			// installButton
			// 
			this.installButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.installButton.Location = new System.Drawing.Point(274, 6);
			this.installButton.Name = "installButton";
			this.installButton.Size = new System.Drawing.Size(93, 23);
			this.installButton.TabIndex = 0;
			this.installButton.Text = "Install AddIn";
			this.installButton.UseCompatibleTextRendering = true;
			this.installButton.UseVisualStyleBackColor = true;
			// 
			// closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.Location = new System.Drawing.Point(373, 6);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 1;
			this.closeButton.Text = "Close";
			this.closeButton.UseCompatibleTextRendering = true;
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer.Location = new System.Drawing.Point(0, 33);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.AutoScroll = true;
			this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Window;
			this.splitContainer.Panel1MinSize = 100;
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.actionGroupBox);
			this.splitContainer.Panel2MinSize = 100;
			this.splitContainer.Size = new System.Drawing.Size(460, 322);
			this.splitContainer.SplitterDistance = 310;
			this.splitContainer.TabIndex = 1;
			// 
			// actionGroupBox
			// 
			this.actionGroupBox.Controls.Add(this.actionFlowLayoutPanel);
			this.actionGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.actionGroupBox.Location = new System.Drawing.Point(0, 0);
			this.actionGroupBox.Name = "actionGroupBox";
			this.actionGroupBox.Size = new System.Drawing.Size(146, 322);
			this.actionGroupBox.TabIndex = 0;
			this.actionGroupBox.TabStop = false;
			this.actionGroupBox.Text = "actionGroupBox";
			this.actionGroupBox.UseCompatibleTextRendering = true;
			// 
			// actionFlowLayoutPanel
			// 
			this.actionFlowLayoutPanel.AutoScroll = true;
			this.actionFlowLayoutPanel.Controls.Add(this.actionDescription);
			this.actionFlowLayoutPanel.Controls.Add(this.dependencyTable);
			this.actionFlowLayoutPanel.Controls.Add(this.runActionButton);
			this.actionFlowLayoutPanel.Controls.Add(this.uninstallButton);
			this.actionFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.actionFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.actionFlowLayoutPanel.Location = new System.Drawing.Point(3, 17);
			this.actionFlowLayoutPanel.Name = "actionFlowLayoutPanel";
			this.actionFlowLayoutPanel.Size = new System.Drawing.Size(140, 302);
			this.actionFlowLayoutPanel.TabIndex = 0;
			this.actionFlowLayoutPanel.WrapContents = false;
			// 
			// actionDescription
			// 
			this.actionDescription.AutoSize = true;
			this.actionDescription.Location = new System.Drawing.Point(3, 0);
			this.actionDescription.Name = "actionDescription";
			this.actionDescription.Size = new System.Drawing.Size(90, 18);
			this.actionDescription.TabIndex = 0;
			this.actionDescription.Text = "actionDescription";
			this.actionDescription.UseCompatibleTextRendering = true;
			// 
			// dependencyTable
			// 
			this.dependencyTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.dependencyTable.ColumnCount = 2;
			this.dependencyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.dependencyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.dependencyTable.Location = new System.Drawing.Point(3, 21);
			this.dependencyTable.Name = "dependencyTable";
			this.dependencyTable.RowCount = 2;
			this.dependencyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.dependencyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.dependencyTable.Size = new System.Drawing.Size(106, 100);
			this.dependencyTable.TabIndex = 1;
			// 
			// runActionButton
			// 
			this.runActionButton.Location = new System.Drawing.Point(3, 127);
			this.runActionButton.Name = "runActionButton";
			this.runActionButton.Size = new System.Drawing.Size(91, 23);
			this.runActionButton.TabIndex = 2;
			this.runActionButton.Text = "runAction";
			this.runActionButton.UseCompatibleTextRendering = true;
			this.runActionButton.UseVisualStyleBackColor = true;
			this.runActionButton.Click += new System.EventHandler(this.RunActionButtonClick);
			// 
			// uninstallButton
			// 
			this.uninstallButton.Location = new System.Drawing.Point(3, 156);
			this.uninstallButton.Name = "uninstallButton";
			this.uninstallButton.Size = new System.Drawing.Size(91, 23);
			this.uninstallButton.TabIndex = 3;
			this.uninstallButton.Text = "Uninstall";
			this.uninstallButton.UseCompatibleTextRendering = true;
			this.uninstallButton.UseVisualStyleBackColor = true;
			// 
			// ManagerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(460, 390);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.bottomPanel);
			this.Controls.Add(this.topPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MinimumSize = new System.Drawing.Size(250, 200);
			this.Name = "ManagerForm";
			this.Text = "AddIn Manager";
			this.bottomPanel.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.actionGroupBox.ResumeLayout(false);
			this.actionFlowLayoutPanel.ResumeLayout(false);
			this.actionFlowLayoutPanel.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button installButton;
		private System.Windows.Forms.Button uninstallButton;
		private System.Windows.Forms.Button runActionButton;
		private System.Windows.Forms.TableLayoutPanel dependencyTable;
		private System.Windows.Forms.Label actionDescription;
		private System.Windows.Forms.FlowLayoutPanel actionFlowLayoutPanel;
		private System.Windows.Forms.GroupBox actionGroupBox;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.Panel bottomPanel;
		private System.Windows.Forms.Panel topPanel;
		#endregion
	}
}
