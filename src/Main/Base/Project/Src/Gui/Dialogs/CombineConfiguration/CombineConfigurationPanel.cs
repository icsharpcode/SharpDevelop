//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//using System;
//using System.Collections;
//using System.ComponentModel;
//using System.Drawing;
//using System.Windows.Forms;
//
//using ICSharpCode.SharpDevelop.Project;
//using ICSharpCode.Core;
//using ICSharpCode.Core;
//using ICSharpCode.Core;
//
//namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
//{
//	/// <summary>
//	/// Summary description for UserControl3.
//	/// </summary>
//	public class CombineConfigurationPanel : AbstractOptionPanel
//	{
//		private System.Windows.Forms.Label label2;
//		private System.Windows.Forms.ComboBox comboBox1;
//		private System.Windows.Forms.Button button4;
//		private System.Windows.Forms.GroupBox groupBox1;
//		private System.Windows.Forms.CheckBox checkBox1;
//		private System.Windows.Forms.ComboBox actioncomboBox;
//		private System.Windows.Forms.ListView entrylistView;
//		private System.Windows.Forms.Label label1;
//		private System.Windows.Forms.ColumnHeader columnHeader1;
//		private System.Windows.Forms.ColumnHeader columnHeader2;
//		private System.Windows.Forms.ColumnHeader columnHeader3;
//		/// <summary> 
//		/// Required designer variable.
//		/// </summary>
//		private System.ComponentModel.Container components = null;
//		
//		
//		
//		Solution combine;
//		
//		void SetValues(object sender, EventArgs e)
//		{
//			this.combine = (Combine)((Properties)CustomizationObject).Get("Combine");
//			checkBox1.CheckedChanged += new EventHandler(OptionsChanged);
//			actioncomboBox.SelectedIndexChanged += new EventHandler(OptionsChanged);
//			entrylistView.SelectedIndexChanged += new EventHandler(SelectEntry);
//			button4.Click += new EventHandler(StartConfigurationManager);
//			
//			InitializeConfigurationComboBox();
//			InitializeEntryListView(null, null);
//		}
//		
//		public override bool ReceiveDialogMessage(DialogMessage message)
//		{
//			if (message == DialogMessage.OK) {
//				
//			}
//		// TODO
////			foreach (ListViewItem item in entrylistView.Items) {
////				CombineConfiguration.Config config = combine.ActiveConfiguration.GetConfiguration(item.Text);
////				config.Build             = Boolean.Parse(item.SubItems[1]);
////				config.ConfigurationName = Boolean.Parse(item.SubItems[0]);
////			}
//			return true;
//		}
//		
//		public CombineConfigurationPanel()
//		{
//			// This call is required by the Windows.Forms Form Designer.
//			InitializeComponent();
//			
//			CustomizationObjectChanged += new EventHandler(SetValues);
//		}
//		
//		void StartConfigurationManager(object sender, EventArgs e)
//		{
//			new ConfigurationManager().ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
//		}
//		void OptionsChanged(object sender, EventArgs e)
//		{
//			if (entrylistView.SelectedItems == null || 
//				entrylistView.SelectedItems.Count == 0) 
//				return;
//			ListViewItem item = entrylistView.SelectedItems[0]; 
//			if (item == null || item.SubItems.Count < 3)
//				return;
//			item.SubItems[2].Text = checkBox1.Checked.ToString();
//			item.SubItems[1].Text = actioncomboBox.SelectedItem.ToString();
//		}
//		
//		void SelectEntry(object sender, EventArgs e)
//		{
//			actioncomboBox.Items.Clear();
//			if (entrylistView.SelectedItems.Count == 0) {
//				actioncomboBox.Enabled = checkBox1.Enabled = false;
//				return;
//			}
//			actioncomboBox.Enabled = checkBox1.Enabled = true;
//			
//			ListViewItem item = entrylistView.SelectedItems[0]; 
//			
//			CombineConfiguration.Config config = null;
//			if (comboBox1.SelectedIndex == 0)
//				config = combine.ActiveConfiguration.GetConfiguration(combine.GetEntryNumber(item.Text));
//			else
//				config = ((CombineConfiguration)combine.Configurations[comboBox1.SelectedItem.ToString()]).GetConfiguration(combine.GetEntryNumber(item.Text));
//			
//			checkBox1.Checked = Boolean.Parse(item.SubItems[2].Text);
//			
//			CombineEntry entry = (CombineEntry)combine.Entries[combine.GetEntryNumber(item.Text)];
//			int index = 0;
//			if (entry.Entry is IProject) {
//				IProject subproject = (IProject)entry.Entry;
//				for (int i = 0; i < subproject.Configurations.Count; ++i) {
//					string name = ((IConfiguration)subproject.Configurations[i]).Name;
//					if (name == item.SubItems[1].Text)
//						index = i;
//					actioncomboBox.Items.Add(name);
//				}
//				
//			} else {
//				Combine    subcombine = (Combine)entry.Entry;
//				
//				for (int i = 0; i < subcombine.Configurations.Count; ++i) {
//					string name = ((CombineConfiguration)subcombine.Configurations[i]).Name;
//					if (name == item.SubItems[1].Text)
//						index = i;
//					actioncomboBox.Items.Add(name);
//				}
//			}
//			actioncomboBox.SelectedIndex = index;
//		}
//		
//		void InitializeEntryListView(object sender, EventArgs e)
//		{
//			entrylistView.BeginUpdate();
//			entrylistView.Items.Clear();
//			foreach (CombineEntry entry in combine.Entries) {
//				CombineConfiguration.Config config = combine.ActiveConfiguration.GetConfiguration(combine.GetEntryNumber(entry.Name));
//				entrylistView.Items.Add(new ListViewItem(new string[] {
//					entry.Name,
//					config.ConfigurationName,
//					config.Build.ToString()
//				}));
//			}
//			entrylistView.EndUpdate();
//		}
//		
//		void InitializeConfigurationComboBox()
//		{
//			if (combine.ActiveConfiguration != null) {
//				comboBox1.Items.Add("Active(" + combine.ActiveConfiguration.Name +")");
//			} else {
//				comboBox1.Items.Add("No active");
//			}
//			foreach (DictionaryEntry dentry in combine.Configurations) {
//				CombineConfiguration cconf = (CombineConfiguration)dentry.Value;
//				comboBox1.Items.Add(cconf.Name);
//			}
//			comboBox1.SelectedIndex = 0;
//			comboBox1.SelectedIndexChanged += new EventHandler(InitializeEntryListView);
//		}
//
//		/// <summary>
//		/// Clean up any resources being used.
//		/// </summary>
//		protected override void Dispose(bool disposing)
//		{
//			if (disposing) {
//				if (components != null){
//					components.Dispose();
//				}
//			}
//			base.Dispose(disposing);
//		}
//
//		#region Component Designer generated code
//		/// <summary> 
//		/// Required method for Designer support - do not modify 
//		/// the contents of this method with the code editor.
//		/// </summary>
//		private void InitializeComponent()
//		{
//			
//			bool flat = true;
//			this.label2 = new System.Windows.Forms.Label();
//			this.comboBox1 = new System.Windows.Forms.ComboBox();
//			this.button4 = new System.Windows.Forms.Button();
//			this.groupBox1 = new System.Windows.Forms.GroupBox();
//			this.checkBox1 = new System.Windows.Forms.CheckBox();
//			this.actioncomboBox = new System.Windows.Forms.ComboBox();
//			this.entrylistView = new System.Windows.Forms.ListView();
//			this.label1 = new System.Windows.Forms.Label();
//			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
//			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
//			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
//			this.groupBox1.SuspendLayout();
//			this.SuspendLayout();
//			// 
//			// label2
//			// 
//			this.label2.Location = new System.Drawing.Point(16, 8);
//			this.label2.Name = "label2";
//			this.label2.Size = new System.Drawing.Size(168, 16);
//			this.label2.TabIndex = 0;
//			this.label2.Text = ResourceService.GetString("Dialog.Options.CombineOptions.Configurations.CombineConfigLabel");
//			
//			//
//			// comboBox1
//			// 
//			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
//			this.comboBox1.DropDownWidth = 176;
//			this.comboBox1.Location = new System.Drawing.Point(16, 24);
//			this.comboBox1.Name = "comboBox1";
//			this.comboBox1.Size = new System.Drawing.Size(224, 21);
//			this.comboBox1.TabIndex = 1;
//			// 
//			// button4
//			// 
//			this.button4.Location = new System.Drawing.Point(248, 24);
//			this.button4.Name = "button4";
//			this.button4.Size = new System.Drawing.Size(136, 23);
//			this.button4.TabIndex = 8;
//			this.button4.Text = ResourceService.GetString("Dialog.Options.CombineOptions.Configurations.ConfigurationManagerButton");
//			button4.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
//			
//			//
//			// groupBox1
//			// 
//			this.groupBox1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
//				| System.Windows.Forms.AnchorStyles.Left) 
//				| System.Windows.Forms.AnchorStyles.Right);
//			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
//																					this.checkBox1,
//																					this.actioncomboBox,
//																					this.entrylistView,
//																					this.label1});
//			this.groupBox1.Location = new System.Drawing.Point(8, 56);
//			this.groupBox1.Name = "groupBox1";
//			this.groupBox1.Size = new System.Drawing.Size(432, 272);
//			this.groupBox1.TabIndex = 9;
//			this.groupBox1.TabStop = false;
//			this.groupBox1.Text = ResourceService.GetString("Dialog.Options.CombineOptions.Configurations.EntriesGroupBox");
//			groupBox1.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
//			
//			// 
//			// checkBox1
//			// 
//			this.checkBox1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
//			this.checkBox1.Location = new System.Drawing.Point(232, 248);
//			this.checkBox1.Name = "checkBox1";
//			this.checkBox1.Size = new System.Drawing.Size(128, 16);
//			this.checkBox1.TabIndex = 7;
//			this.checkBox1.Text = ResourceService.GetString("Dialog.Options.CombineOptions.Configurations.BuildCheckBox");
//			checkBox1.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
//			
//			// 
//			// actioncomboBox
//			// 
//			this.actioncomboBox.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
//			this.actioncomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
//			this.actioncomboBox.DropDownWidth = 168;
//			this.actioncomboBox.Location = new System.Drawing.Point(8, 248);
//			this.actioncomboBox.Name = "actioncomboBox";
//			this.actioncomboBox.Size = new System.Drawing.Size(216, 21);
//			this.actioncomboBox.TabIndex = 5;
//			
//			//
//			// entrylistView
//			// 
//			this.entrylistView.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
//				| System.Windows.Forms.AnchorStyles.Left) 
//				| System.Windows.Forms.AnchorStyles.Right);
//			this.entrylistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
//																							this.columnHeader1,
//																							this.columnHeader2,
//																							this.columnHeader3});
//			this.entrylistView.FullRowSelect = true;
//			this.entrylistView.GridLines = true;
//			this.entrylistView.HideSelection = false;
//			this.entrylistView.Location = new System.Drawing.Point(8, 16);
//			this.entrylistView.MultiSelect = false;
//			this.entrylistView.Name = "entrylistView";
//			this.entrylistView.Size = new System.Drawing.Size(416, 208);
//			this.entrylistView.TabIndex = 6;
//			this.entrylistView.View = System.Windows.Forms.View.Details;
//			entrylistView.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
//			
//			//
//			// label1
//			// 
//			this.label1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
//			this.label1.Location = new System.Drawing.Point(8, 232);
//			this.label1.Name = "label1";
//			this.label1.Size = new System.Drawing.Size(176, 16);
//			this.label1.TabIndex = 4;
//			this.label1.Text = ResourceService.GetString("Dialog.Options.CombineOptions.Configurations.EntryConfigurationLabel");
//			
//			//
//			// columnHeader1
//			// 
//			this.columnHeader1.Text = ResourceService.GetString("Dialog.Options.CombineOptions.Configurations.EntryColumnHeader");
//			this.columnHeader1.Width = 242;
//			
//			//
//			// columnHeader2
//			// 
//			this.columnHeader2.Text = ResourceService.GetString("Dialog.Options.CombineOptions.Configurations.ConfigurationColumnHeader");
//			this.columnHeader2.Width = 109;
//			
//			// 
//			// columnHeader3
//			// 
//			this.columnHeader3.Text = ResourceService.GetString("Dialog.Options.CombineOptions.Configurations.BuildColumnHeader");
//			
//			// 
//			// UserControl3
//			// 
//			this.Controls.AddRange(new System.Windows.Forms.Control[] {this.groupBox1,
//			                       this.label2,
//			                       this.comboBox1,
//			                       this.button4});
//			this.Name = "UserControl3";
//			this.Size = new System.Drawing.Size(448, 336);
//			this.groupBox1.ResumeLayout(false);
//			this.ResumeLayout(false);
//
//		}
//		#endregion
//	}
//}
//
