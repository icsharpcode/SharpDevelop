// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
			
			List<AddIn> addInList = new List<AddIn>(AddInTree.AddIns);
			addInList.Sort(delegate(AddIn a, AddIn b) {
			               	return a.Name.CompareTo(b.Name);
			               });
			foreach (AddIn addIn in addInList) {
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
			ShowPreinstalledAddInsCheckBoxCheckedChanged(null, null);
			splitContainer.Panel2Collapsed = true;
			splitContainer.Panel1.Paint += delegate(object sender, PaintEventArgs e) {
				if (visibleAddInCount == 0) {
					Rectangle rect = splitContainer.Panel1.ClientRectangle;
					rect.Offset(16, 16);
					rect.Inflate(-32, -32);
					e.Graphics.DrawString("You don't have any AddIns installed.\n" +
					                      "Download an AddIn from the Internet, then click 'Install AddIn' and " +
					                      "choose the downloaded file to install it.",
					                      Font, SystemBrushes.ControlText, rect);
				}
			};
		}
		
		int visibleAddInCount = 0;
		
		void ShowPreinstalledAddInsCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			visibleAddInCount = 0;
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				if (showPreinstalledAddInsCheckBox.Checked) {
					ctl.Visible = true;
				} else {
					if (ctl.Selected)
						ctl.Selected = false;
					if (ctl == oldFocus)
						oldFocus = null;
					ctl.Visible = !FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, ctl.AddIn.FileName);
				}
				if (ctl.Visible)
					visibleAddInCount += 1;
			}
			UpdateActionBox();
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
					if (!ctl.Visible) continue;
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
				bool allUninstallable = true;
				foreach (AddIn addIn in selected) {
					allEnabled  &= addIn.Action == AddInAction.Enable;
					allDisabled &= addIn.Action == AddInAction.Disable;
					if (allUninstallable) {
						if (FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, addIn.FileName)) {
							allUninstallable = false;
						}
					}
				}
				if (allEnabled) {
					selectedAction = AddInAction.Disable;
					actionGroupBox.Text = runActionButton.Text = "Disable";
					actionDescription.Text = "Disables the selected AddIns.";
					runActionButton.Enabled = ShowDependencies(selected, false);
					if (dependencyTable.Visible) {
						actionDescription.Text += "\nThese AddIns are used by:";
					}
					uninstallButton.Enabled = allUninstallable && runActionButton.Enabled;
				} else if (allDisabled) {
					selectedAction = AddInAction.Enable;
					actionGroupBox.Text = runActionButton.Text = "Enable";
					actionDescription.Text = "Enables the selected AddIns.";
					runActionButton.Enabled = ShowDependencies(selected, true);
					if (dependencyTable.Visible) {
						actionDescription.Text += "\nRequired dependencies:";
					}
					uninstallButton.Enabled = allUninstallable;
				} else {
					actionGroupBox.Text = "";
					actionDescription.Text = "AddIns with multiple states are selected";
					runActionButton.Visible = false;
					uninstallButton.Visible = false;
				}
			}
			ignoreFocusChange = false;
		}
		
		bool ShowDependencies(List<AddIn> addIns, bool enable)
		{
			List<AddInReference> dependencies = new List<AddInReference>(); // only used with enable=true
			List<KeyValuePair<AddIn, AddInReference>> dependenciesToSel = new List<KeyValuePair<AddIn, AddInReference>>();
			Dictionary<string, Version> addInDict = new Dictionary<string, Version>();
			Dictionary<string, Version> modifiedAddIns = new Dictionary<string, Version>();
			
			// add available addins
			foreach (AddIn addIn in AddInTree.AddIns) {
				if (addIn.Action != AddInAction.Enable && addIn.Action != AddInAction.Install)
					continue;
				if (addIns.Contains(addIn))
					continue;
				foreach (KeyValuePair<string, Version> pair in addIn.Manifest.Identities) {
					addInDict[pair.Key] = pair.Value;
				}
			}
			
			// create list of modified addin names
			foreach (AddIn addIn in addIns) {
				foreach (KeyValuePair<string, Version> pair in addIn.Manifest.Identities) {
					modifiedAddIns[pair.Key] = pair.Value;
				}
			}
			
			// add new addins
			if (enable) {
				foreach (AddIn addIn in addIns) {
					foreach (KeyValuePair<string, Version> pair in addIn.Manifest.Identities) {
						addInDict[pair.Key] = pair.Value;
					}
					foreach (AddInReference dep in addIn.Manifest.Dependencies) {
						if (!dependencies.Contains(dep))
							dependencies.Add(dep);
					}
				}
			}
			
			// add dependencies to the to-be-changed addins
			foreach (AddIn addIn in AddInTree.AddIns) {
				if (addIn.Action != AddInAction.Enable && addIn.Action != AddInAction.Install)
					continue;
				if (addIns.Contains(addIn))
					continue;
				foreach (AddInReference dep in addIn.Manifest.Dependencies) {
					if (modifiedAddIns.ContainsKey(dep.Name)) {
						dependenciesToSel.Add(new KeyValuePair<AddIn, AddInReference>(addIn, dep));
					}
				}
			}
			
			foreach (Control ctl in dependencyTable.Controls) {
				ctl.Dispose();
			}
			dependencyTable.Controls.Clear();
			bool allDepenciesOK = true;
			if (dependencies.Count > 0 || dependenciesToSel.Count > 0) {
				dependencyTable.RowCount = dependencies.Count + dependenciesToSel.Count;
				while (dependencyTable.RowStyles.Count < dependencyTable.RowCount) {
					dependencyTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
				}
				int rowIndex = 0;
				foreach (AddInReference dep in dependencies) {
					if (!AddDependencyRow(addInDict, dep, rowIndex++, null))
						allDepenciesOK = false;
				}
				foreach (KeyValuePair<AddIn, AddInReference> pair in dependenciesToSel) {
					if (!AddDependencyRow(addInDict, pair.Value, rowIndex++, pair.Key.Name))
						allDepenciesOK = false;
				}
				dependencyTable.Visible = true;
			}
			return allDepenciesOK;
		}
		
		bool AddDependencyRow(Dictionary<string, Version> addInDict, AddInReference dep, int rowIndex, string requiredByName)
		{
			string text = requiredByName ?? GetDisplayName(dep.Name);
			Version versionFound;
			Label label = new Label();
			label.AutoSize = true;
			label.Text = text;
			PictureBox box = new PictureBox();
			box.BorderStyle = BorderStyle.None;
			box.Size = new Size(16, 16);
			bool isOK = dep.Check(addInDict, out versionFound);
			box.SizeMode = PictureBoxSizeMode.CenterImage;
			box.Image = isOK ? ResourceService.GetBitmap("Icons.16x16.OK") : ResourceService.GetBitmap("Icons.16x16.DeleteIcon");
			dependencyTable.Controls.Add(label, 1, rowIndex);
			dependencyTable.Controls.Add(box,   0, rowIndex);
			return isOK;
		}
		
		string GetDisplayName(string identity)
		{
			foreach (AddIn addIn in AddInTree.AddIns) {
				if (addIn.Manifest.Identities.ContainsKey(identity))
					return addIn.Name;
			}
			return identity;
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
		
		void InstallButtonClick(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog()) {
				dlg.Filter = "SharpDevelop AddIns|*.addin;*.sdaddin|All files|*.*";
				dlg.Multiselect = true;
				if (dlg.ShowDialog() == DialogResult.OK) {
					MessageService.ShowMessage("Not implemented.");
					//foreach (string file in dlg.FileNames) {
					//
					//}
				}
			}
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
			this.showPreinstalledAddInsCheckBox = new System.Windows.Forms.CheckBox();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.actionGroupBox = new System.Windows.Forms.GroupBox();
			this.actionFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.actionDescription = new System.Windows.Forms.Label();
			this.dependencyTable = new System.Windows.Forms.TableLayoutPanel();
			this.dummyLabel1 = new System.Windows.Forms.Label();
			this.dummyLabel2 = new System.Windows.Forms.Label();
			this.runActionButton = new System.Windows.Forms.Button();
			this.uninstallButton = new System.Windows.Forms.Button();
			this.bottomPanel.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.actionGroupBox.SuspendLayout();
			this.actionFlowLayoutPanel.SuspendLayout();
			this.dependencyTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// topPanel
			// 
			this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.topPanel.Location = new System.Drawing.Point(0, 0);
			this.topPanel.Name = "topPanel";
			this.topPanel.Size = new System.Drawing.Size(460, 33);
			this.topPanel.TabIndex = 0;
			this.topPanel.Visible = false;
			// 
			// bottomPanel
			// 
			this.bottomPanel.Controls.Add(this.installButton);
			this.bottomPanel.Controls.Add(this.closeButton);
			this.bottomPanel.Controls.Add(this.showPreinstalledAddInsCheckBox);
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
			this.installButton.TabIndex = 1;
			this.installButton.Text = "Install AddIn";
			this.installButton.UseCompatibleTextRendering = true;
			this.installButton.UseVisualStyleBackColor = true;
			this.installButton.Click += new System.EventHandler(this.InstallButtonClick);
			// 
			// closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.Location = new System.Drawing.Point(373, 6);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 2;
			this.closeButton.Text = "Close";
			this.closeButton.UseCompatibleTextRendering = true;
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
			// 
			// showPreinstalledAddInsCheckBox
			// 
			this.showPreinstalledAddInsCheckBox.Location = new System.Drawing.Point(3, 6);
			this.showPreinstalledAddInsCheckBox.Name = "showPreinstalledAddInsCheckBox";
			this.showPreinstalledAddInsCheckBox.Size = new System.Drawing.Size(169, 24);
			this.showPreinstalledAddInsCheckBox.TabIndex = 0;
			this.showPreinstalledAddInsCheckBox.Text = "Show preinstalled AddIns";
			this.showPreinstalledAddInsCheckBox.UseCompatibleTextRendering = true;
			this.showPreinstalledAddInsCheckBox.UseVisualStyleBackColor = true;
			this.showPreinstalledAddInsCheckBox.CheckedChanged += new System.EventHandler(this.ShowPreinstalledAddInsCheckBoxCheckedChanged);
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
			this.splitContainer.SplitterDistance = 248;
			this.splitContainer.TabIndex = 1;
			// 
			// actionGroupBox
			// 
			this.actionGroupBox.Controls.Add(this.actionFlowLayoutPanel);
			this.actionGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.actionGroupBox.Location = new System.Drawing.Point(0, 0);
			this.actionGroupBox.Name = "actionGroupBox";
			this.actionGroupBox.Size = new System.Drawing.Size(208, 322);
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
			this.actionFlowLayoutPanel.Size = new System.Drawing.Size(202, 302);
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
			this.dependencyTable.AutoSize = true;
			this.dependencyTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.dependencyTable.ColumnCount = 2;
			this.dependencyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.dependencyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.dependencyTable.Controls.Add(this.dummyLabel1, 1, 0);
			this.dependencyTable.Controls.Add(this.dummyLabel2, 1, 1);
			this.dependencyTable.Location = new System.Drawing.Point(3, 21);
			this.dependencyTable.Name = "dependencyTable";
			this.dependencyTable.RowCount = 2;
			this.dependencyTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.dependencyTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.dependencyTable.Size = new System.Drawing.Size(55, 36);
			this.dependencyTable.TabIndex = 1;
			// 
			// dummyLabel1
			// 
			this.dummyLabel1.AutoSize = true;
			this.dummyLabel1.Location = new System.Drawing.Point(23, 0);
			this.dummyLabel1.Name = "dummyLabel1";
			this.dummyLabel1.Size = new System.Drawing.Size(29, 18);
			this.dummyLabel1.TabIndex = 0;
			this.dummyLabel1.Text = "dep1";
			this.dummyLabel1.UseCompatibleTextRendering = true;
			// 
			// dummyLabel2
			// 
			this.dummyLabel2.AutoSize = true;
			this.dummyLabel2.Location = new System.Drawing.Point(23, 18);
			this.dummyLabel2.Name = "dummyLabel2";
			this.dummyLabel2.Size = new System.Drawing.Size(29, 18);
			this.dummyLabel2.TabIndex = 1;
			this.dummyLabel2.Text = "dep2";
			this.dummyLabel2.UseCompatibleTextRendering = true;
			// 
			// runActionButton
			// 
			this.runActionButton.Location = new System.Drawing.Point(3, 63);
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
			this.uninstallButton.Location = new System.Drawing.Point(3, 92);
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
			this.dependencyTable.ResumeLayout(false);
			this.dependencyTable.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label dummyLabel2;
		private System.Windows.Forms.Label dummyLabel1;
		private System.Windows.Forms.CheckBox showPreinstalledAddInsCheckBox;
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
