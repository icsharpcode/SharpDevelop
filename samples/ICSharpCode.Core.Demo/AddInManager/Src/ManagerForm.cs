// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.AddInManager
{
	public class ManagerForm : System.Windows.Forms.Form
	{
		#region Form Initialization
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
				#if !STANDALONE
				instance.Owner = ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm;
				#endif
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
			
			#if !STANDALONE
			ICSharpCode.SharpDevelop.Gui.FormLocationHelper.Apply(this, "AddInManager.WindowBounds", true);
			#endif
			
			#if STANDALONE
			actionFlowLayoutPanel.BackgroundImage = new Bitmap(typeof(ManagerForm).Assembly.GetManifestResourceStream("ICSharpCode.AddInManager.WizardBackground.png"));
			#else
			actionFlowLayoutPanel.BackgroundImage = ResourceService.GetBitmap("GeneralWizardBackground");
			#endif
			
			installButton.Text = ResourceService.GetString("AddInManager.InstallButton");
			uninstallButton.Text = ResourceService.GetString("AddInManager.ActionUninstall");
			closeButton.Text = ResourceService.GetString("Global.CloseButtonText");
			showPreinstalledAddInsCheckBox.Text = ResourceService.GetString("AddInManager.ShowPreinstalledAddIns");
			this.Text = ResourceService.GetString("AddInManager.Title");
			CreateAddInList();
		}
		
		void OnSplitContainerPanel1Paint(object sender, PaintEventArgs e)
		{
			if (visibleAddInCount == 0) {
				Rectangle rect = splitContainer.Panel1.ClientRectangle;
				rect.Offset(16, 16);
				rect.Inflate(-32, -32);
				e.Graphics.DrawString(ResourceService.GetString("AddInManager.NoAddInsInstalled"),
				                      Font, SystemBrushes.WindowText, rect);
			}
		}
		
		void CreateAddInList()
		{
			Stack<AddInControl> stack = new Stack<AddInControl>();
			int index = 0;
			AddInControl addInControl;
			
			List<AddIn> addInList = new List<AddIn>(AddInTree.AddIns);
			addInList.Sort(delegate(AddIn a, AddIn b) {
			               	return a.Name.CompareTo(b.Name);
			               });
			foreach (AddIn addIn in addInList) {
				string identity = addIn.Manifest.PrimaryIdentity;
				if (identity == null || addIn.Properties["addInManagerHidden"] == "true")
					continue;
				addInControl = new AddInControl(addIn);
				addInControl.Dock = DockStyle.Top;
				addInControl.TabIndex = index++;
				stack.Push(addInControl);
				addInControl.Enter += OnControlEnter;
				addInControl.Click += OnControlClick;
			}
			while (stack.Count > 0) {
				splitContainer.Panel1.Controls.Add(stack.Pop());
			}
			ShowPreinstalledAddInsCheckBoxCheckedChanged(null, null);
			#if SHOWALLADDINS
			showPreinstalledAddInsCheckBox.Visible = false;
			showPreinstalledAddInsCheckBox.Checked = true;
			#endif
			splitContainer.Panel2Collapsed = true;
		}
		
		void RefreshAddInList()
		{
			List<AddIn> oldSelected = selected;
			foreach (Control ctl in splitContainer.Panel1.Controls) {
				ctl.Dispose();
			}
			splitContainer.Panel1.Controls.Clear();
			CreateAddInList();
			if (oldSelected != null) {
				foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
					if (oldSelected.Contains(ctl.AddIn))
						ctl.Selected = true;
				}
			}
			UpdateActionBox();
		}
		#endregion
		
		#region AddInList-Management
		int visibleAddInCount = 0;
		
		void ShowPreinstalledAddInsCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			visibleAddInCount = 0;
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				ctl.Selected = false;
				bool visible;
				if (showPreinstalledAddInsCheckBox.Checked) {
					visible = true;
				} else {
					if (ctl == oldFocus)
						oldFocus = null;
					visible = !FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, ctl.AddIn.FileName);
				}
				if (visible)
					visibleAddInCount += 1;
				ctl.Visible = visible;
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
		#endregion
		
		#region UpdateActionBox
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
				runActionButton.Visible = true;
				uninstallButton.Visible = true;
				
				bool allEnabled      = true;
				bool allDisabled     = true;
				bool allInstalling   = true;
				bool allUninstalling = true;
				bool allUpdating     = true;
				bool allUninstallable = true;
				bool hasErrors = false;
				foreach (AddIn addIn in selected) {
					allEnabled      &= addIn.Action == AddInAction.Enable;
					if (addIn.Action == AddInAction.DependencyError || addIn.Action == AddInAction.InstalledTwice)
						hasErrors = true;
					else
						allDisabled     &= addIn.Action == AddInAction.Disable;
					allUpdating     &= addIn.Action == AddInAction.Update;
					allInstalling   &= addIn.Action == AddInAction.Install;
					allUninstalling &= addIn.Action == AddInAction.Uninstall;
					if (allUninstallable) {
						if (FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, addIn.FileName)) {
							allUninstallable = false;
						}
					}
				}
				if (allEnabled) {
					selectedAction = AddInAction.Disable;
					actionGroupBox.Text = runActionButton.Text = ResourceService.GetString("AddInManager.ActionDisable");
					actionDescription.Text = ResourceService.GetString("AddInManager.DescriptionDisable");
					runActionButton.Enabled = ShowDependencies(selected, ShowDependencyMode.Disable);
					uninstallButton.Enabled = allUninstallable && runActionButton.Enabled;
				} else if (allDisabled) {
					selectedAction = AddInAction.Enable;
					actionGroupBox.Text = runActionButton.Text = ResourceService.GetString("AddInManager.ActionEnable");
					actionDescription.Text = ResourceService.GetString("AddInManager.DescriptionEnable");
					runActionButton.Enabled = ShowDependencies(selected, ShowDependencyMode.Enable);
					if (hasErrors)
						runActionButton.Enabled = false;
					uninstallButton.Enabled = allUninstallable;
				} else if (allInstalling) {
					selectedAction = AddInAction.Uninstall;
					actionGroupBox.Text = runActionButton.Text = ResourceService.GetString("AddInManager.ActionCancelInstallation");
					actionDescription.Text = ResourceService.GetString("AddInManager.DescriptionCancelInstall");
					runActionButton.Enabled = ShowDependencies(selected, ShowDependencyMode.Disable);
					uninstallButton.Visible = false;
				} else if (allUninstalling) {
					selectedAction = AddInAction.Enable;
					actionGroupBox.Text = runActionButton.Text = ResourceService.GetString("AddInManager.ActionCancelDeinstallation");
					actionDescription.Text = ResourceService.GetString("AddInManager.DescriptionCancelDeinstallation");
					runActionButton.Enabled = ShowDependencies(selected, ShowDependencyMode.Enable);
					uninstallButton.Visible = false;
				} else if (allUpdating) {
					selectedAction = AddInAction.InstalledTwice;
					actionGroupBox.Text = runActionButton.Text = ResourceService.GetString("AddInManager.ActionCancelUpdate");
					actionDescription.Text = ResourceService.GetString("AddInManager.DescriptionCancelUpdate");
					runActionButton.Enabled = ShowDependencies(selected, ShowDependencyMode.CancelUpdate);
					uninstallButton.Visible = false;
				} else {
					actionGroupBox.Text = "";
					actionDescription.Text = ResourceService.GetString("AddInManager.DescriptionInconsistentSelection");
					runActionButton.Visible = false;
					uninstallButton.Visible = false;
				}
			}
			ignoreFocusChange = false;
		}
		
		enum ShowDependencyMode {
			Disable,
			Enable,
			CancelUpdate
		}
		
		bool ShowDependencies(IList<AddIn> addIns, ShowDependencyMode mode)
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
			if (mode != ShowDependencyMode.Disable) {
				foreach (AddIn addIn in addIns) {
					if (mode == ShowDependencyMode.CancelUpdate && !addIn.Enabled) {
						continue;
					}
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
				if (dependencies.Count == 0) {
					dependencyTable.RowCount = 1 + dependenciesToSel.Count;
				} else if (dependenciesToSel.Count == 0) {
					dependencyTable.RowCount = 1 + dependencies.Count;
				} else {
					dependencyTable.RowCount = 2 + dependencies.Count + dependenciesToSel.Count;
				}
				while (dependencyTable.RowStyles.Count < dependencyTable.RowCount) {
					dependencyTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
				}
				int rowIndex = 0;
				if (dependencies.Count > 0) {
					AddLabelRow(rowIndex++, ResourceService.GetString("AddInManager.RequiredDependencies"));
					foreach (AddInReference dep in dependencies) {
						if (!AddDependencyRow(addInDict, dep, rowIndex++, null))
							allDepenciesOK = false;
					}
				}
				if (dependenciesToSel.Count > 0) {
					AddLabelRow(rowIndex++, ResourceService.GetString("AddInManager.RequiredBy"));
					foreach (KeyValuePair<AddIn, AddInReference> pair in dependenciesToSel) {
						if (!AddDependencyRow(addInDict, pair.Value, rowIndex++, pair.Key.Name))
							allDepenciesOK = false;
					}
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
			box.Image = WinFormsResourceService.GetBitmap(isOK ? "Icons.16x16.OK" : "Icons.16x16.DeleteIcon");
			dependencyTable.Controls.Add(label, 1, rowIndex);
			dependencyTable.Controls.Add(box,   0, rowIndex);
			return isOK;
		}
		
		void AddLabelRow(int rowIndex, string text)
		{
			Label label = new Label();
			label.AutoSize = true;
			label.Text = text;
			dependencyTable.Controls.Add(label, 0, rowIndex);
			dependencyTable.SetColumnSpan(label, 2);
		}
		
		string GetDisplayName(string identity)
		{
			foreach (AddIn addIn in AddInTree.AddIns) {
				if (addIn.Manifest.Identities.ContainsKey(identity))
					return addIn.Name;
			}
			return identity;
		}
		#endregion
		
		#region Install new AddIns
		void InstallButtonClick(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog()) {
				dlg.Filter = ResourceService.GetString("AddInManager.FileFilter");
				dlg.Multiselect = true;
				if (dlg.ShowDialog() == DialogResult.OK) {
					if (ShowInstallableAddIns(dlg.FileNames)) {
						if (runActionButton.Visible && runActionButton.Enabled)
							runActionButton.PerformClick();
					}
				}
			}
		}
		
		public bool ShowInstallableAddIns(IEnumerable<string> fileNames)
		{
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				ctl.Selected = false;
			}
			UpdateActionBox();
			List<InstallableAddIn> list = new List<InstallableAddIn>();
			foreach (string file in fileNames) {
				try {
					// Same file-extension check is in Panel1DragEnter
					switch (Path.GetExtension(file).ToLowerInvariant()) {
						case ".addin":
							if (FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, file)) {
								MessageService.ShowMessage("You cannot install AddIns inside the SharpDevelop directory, " +
								                           "they will be picked up as pre-installed AddIns automatically.");
								return false;
							}
							list.Add(new InstallableAddIn(file, false));
							break;
						case ".sdaddin":
						case ".zip":
							list.Add(new InstallableAddIn(file, true));
							break;
						default:
							MessageService.ShowMessage("Unknown file format: " + Path.GetExtension(file));
							return false;
					}
				} catch (AddInLoadException ex) {
					MessageService.ShowMessage("Error loading " + file + ":\n" + ex.Message);
					return false;
				}
			}
			ShowInstallableAddIns(list);
			return true;
		}
		
		IList<InstallableAddIn> shownAddInPackages;
		
		void ShowInstallableAddIns(IList<InstallableAddIn> addInPackages)
		{
			shownAddInPackages = addInPackages;
			ignoreFocusChange = true;
			splitContainer.Panel2Collapsed = false;
			dependencyTable.Visible = false;
			runActionButton.Visible = true;
			uninstallButton.Visible = false;
			
			selectedAction = AddInAction.Install;
			List<string> installAddIns = new List<string>();
			List<string> updateAddIns = new List<string>();
			foreach (InstallableAddIn addInPackage in addInPackages) {
				string identity = addInPackage.AddIn.Manifest.PrimaryIdentity;
				AddIn foundAddIn = null;
				foreach (AddIn addIn in AddInTree.AddIns) {
					if (addIn.Action != AddInAction.Install
					    && addIn.Manifest.Identities.ContainsKey(identity))
					{
						foundAddIn = addIn;
						break;
					}
				}
				if (foundAddIn != null) {
					updateAddIns.Add(addInPackage.AddIn.Name);
				} else {
					installAddIns.Add(addInPackage.AddIn.Name);
				}
			}
			
			if (updateAddIns.Count == 0) {
				actionGroupBox.Text = runActionButton.Text = ResourceService.GetString("AddInManager.ActionInstall");
			} else if (installAddIns.Count == 0) {
				actionGroupBox.Text = runActionButton.Text = ResourceService.GetString("AddInManager.ActionUpdate");
			} else {
				actionGroupBox.Text = runActionButton.Text =
					ResourceService.GetString("AddInManager.ActionInstall")
					+ " + " +
					ResourceService.GetString("AddInManager.ActionUpdate");
			}
			List<AddIn> addInList = new List<AddIn>();
			StringBuilder b = new StringBuilder();
			if (installAddIns.Count == 1) {
				b.Append("Installs the AddIn " + installAddIns[0]);
			} else if (installAddIns.Count > 1) {
				b.Append("Installs the AddIns " + string.Join(",", installAddIns.ToArray()));
			}
			if (updateAddIns.Count > 0 && installAddIns.Count > 0)
				b.Append("; ");
			if (updateAddIns.Count == 1) {
				b.Append("Updates the AddIn " + updateAddIns[0]);
			} else if (updateAddIns.Count > 1) {
				b.Append("Updates the AddIns " + string.Join(",", updateAddIns.ToArray()));
			}
			actionDescription.Text = b.ToString();
			runActionButton.Enabled = ShowDependencies(addInList, ShowDependencyMode.Enable);
		}
		
		void RunInstallation()
		{
			// install new AddIns
			foreach (InstallableAddIn addInPackage in shownAddInPackages) {
				string identity = addInPackage.AddIn.Manifest.PrimaryIdentity;
				AddIn foundAddIn = null;
				foreach (AddIn addIn in AddInTree.AddIns) {
					if (addIn.Manifest.Identities.ContainsKey(identity)) {
						foundAddIn = addIn;
						break;
					}
				}
				if (foundAddIn != null) {
					addInPackage.Install(true);
					if (foundAddIn.Action != AddInAction.Enable) {
						ICSharpCode.Core.AddInManager.Enable(new AddIn[] { foundAddIn });
					}
					if (foundAddIn.Action != AddInAction.Install) {
						foundAddIn.Action = AddInAction.Update;
					}
				} else {
					addInPackage.Install(false);
				}
			}
			RefreshAddInList();
		}
		#endregion
		
		#region Uninstall AddIns
		void UninstallButtonClick(object sender, EventArgs e)
		{
			ICSharpCode.Core.AddInManager.RemoveExternalAddIns(selected);
			InstallableAddIn.Uninstall(selected);
			RefreshAddInList();
		}
		#endregion
		
		#region Drag'N'Drop
		void Panel1DragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
				e.Effect = DragDropEffects.None;
				return;
			}
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			int addInCount = 0;
			int packageCount = 0;
			foreach (string file in files) {
				switch (Path.GetExtension(file).ToLowerInvariant()) {
					case ".addin":
						addInCount += 1;
						break;
					case ".sdaddin":
					case ".zip":
						packageCount += 1;
						break;
					default:
						e.Effect = DragDropEffects.None;
						return;
				}
			}
			if (addInCount == 0 && packageCount == 0) {
				e.Effect = DragDropEffects.None;
			} else if (addInCount == 0) {
				e.Effect = DragDropEffects.Copy;
			} else {
				e.Effect = DragDropEffects.Link;
			}
		}
		
		void Panel1DragDrop(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
				return;
			ShowInstallableAddIns((string[])e.Data.GetData(DataFormats.FileDrop));
		}
		#endregion
		
		void CloseButtonClick(object sender, EventArgs e)
		{
			Close();
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			instance = null;
		}
		
		public void TryRunAction(AddIn addIn, AddInAction action)
		{
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				ctl.Selected = ctl.AddIn == addIn;
			}
			UpdateActionBox();
			if (selectedAction == action && runActionButton.Visible && runActionButton.Enabled)
				runActionButton.PerformClick();
		}
		
		public void TryUninstall(AddIn addIn)
		{
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				ctl.Selected = ctl.AddIn == addIn;
			}
			UpdateActionBox();
			if (uninstallButton.Visible && uninstallButton.Enabled)
				uninstallButton.PerformClick();
		}
		
		void RunActionButtonClick(object sender, EventArgs e)
		{
			switch (selectedAction) {
				case AddInAction.Disable:
					for (int i = 0; i < selected.Count; i++) {
						if (selected[i].Manifest.PrimaryIdentity == "ICSharpCode.AddInManager") {
							MessageService.ShowMessage("You cannot disable the AddInManager because you need it to re-enable AddIns!");
							selected.RemoveAt(i--);
						}
					}
					ICSharpCode.Core.AddInManager.Disable(selected);
					break;
				case AddInAction.Enable:
					ICSharpCode.Core.AddInManager.Enable(selected);
					break;
				case AddInAction.Install:
					RunInstallation();
					return;
				case AddInAction.Uninstall:
					UninstallButtonClick(sender, e);
					return;
				case AddInAction.InstalledTwice: // used to cancel installation of update
					InstallableAddIn.CancelUpdate(selected);
					foreach (AddIn addIn in selected) {
						addIn.Action = addIn.Enabled ? AddInAction.Enable : AddInAction.Disable;
					}
					break;
				default:
					throw new NotImplementedException();
			}
			foreach (AddInControl ctl in splitContainer.Panel1.Controls) {
				ctl.Invalidate();
			}
			UpdateActionBox();
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
			this.topPanel.TabIndex = 1;
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
			this.bottomPanel.TabIndex = 0;
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
			this.splitContainer.BackColor = System.Drawing.SystemColors.Window;
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer.Location = new System.Drawing.Point(0, 33);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.AllowDrop = true;
			this.splitContainer.Panel1.AutoScroll = true;
			this.splitContainer.Panel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.Panel1DragDrop);
			this.splitContainer.Panel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.Panel1DragEnter);
			this.splitContainer.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.OnSplitContainerPanel1Paint);
			this.splitContainer.Panel1MinSize = 100;
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.actionGroupBox);
			this.splitContainer.Panel2MinSize = 100;
			this.splitContainer.Size = new System.Drawing.Size(460, 322);
			this.splitContainer.SplitterDistance = 248;
			this.splitContainer.TabIndex = 2;
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
			this.actionFlowLayoutPanel.ForeColor = System.Drawing.SystemColors.WindowText;
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
			this.runActionButton.AutoSize = true;
			this.runActionButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.runActionButton.Location = new System.Drawing.Point(3, 63);
			this.runActionButton.MinimumSize = new System.Drawing.Size(91, 25);
			this.runActionButton.Name = "runActionButton";
			this.runActionButton.Size = new System.Drawing.Size(91, 25);
			this.runActionButton.TabIndex = 2;
			this.runActionButton.Text = "runAction";
			this.runActionButton.UseCompatibleTextRendering = true;
			this.runActionButton.UseVisualStyleBackColor = true;
			this.runActionButton.Click += new System.EventHandler(this.RunActionButtonClick);
			// 
			// uninstallButton
			// 
			this.uninstallButton.AutoSize = true;
			this.uninstallButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.uninstallButton.Location = new System.Drawing.Point(3, 94);
			this.uninstallButton.MinimumSize = new System.Drawing.Size(91, 25);
			this.uninstallButton.Name = "uninstallButton";
			this.uninstallButton.Size = new System.Drawing.Size(91, 25);
			this.uninstallButton.TabIndex = 3;
			this.uninstallButton.Text = "Uninstall";
			this.uninstallButton.UseCompatibleTextRendering = true;
			this.uninstallButton.UseVisualStyleBackColor = true;
			this.uninstallButton.Click += new System.EventHandler(this.UninstallButtonClick);
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
