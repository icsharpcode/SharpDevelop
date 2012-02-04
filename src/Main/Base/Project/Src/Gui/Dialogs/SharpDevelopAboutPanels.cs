// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class AboutSharpDevelopTabPage : UserControl
	{
		Label      buildLabel   = new Label();
		TextBox    buildTextBox = new TextBox();
		
		Label      versionLabel   = new Label();
		TextBox    versionTextBox = new TextBox();
		
		Label      sponsorLabel   = new Label();
		TextBox    versionInfoTextBox = new TextBox();
		
		public AboutSharpDevelopTabPage()
		{
			versionTextBox.Text = RevisionClass.Major + "." + RevisionClass.Minor + "." + RevisionClass.Build;
			buildTextBox.Text   = RevisionClass.Revision;
			
			versionLabel.Location = new System.Drawing.Point(8, 8);
			versionLabel.Text = ResourceService.GetString("Dialog.About.label1Text");
			versionLabel.Size = new System.Drawing.Size(64, 16);
			versionLabel.TabIndex = 1;
			Controls.Add(versionLabel);
			
			versionTextBox.Location = new System.Drawing.Point(64 + 8 + 4, 8);
			versionTextBox.ReadOnly = true;
			versionTextBox.TabIndex = 4;
			versionTextBox.Size = new System.Drawing.Size(48, 20);
			Controls.Add(versionTextBox);
			
			buildLabel.Location = new System.Drawing.Point(64 + 12 + 48 + 4, 8);
			buildLabel.Text = ResourceService.GetString("Dialog.About.label2Text");
			buildLabel.Size = new System.Drawing.Size(48, 16);
			buildLabel.TabIndex = 2;
			Controls.Add(buildLabel);
			
			buildTextBox.Location = new System.Drawing.Point(64 + 12 + 48 + 4 + 48 + 4, 8);
			buildTextBox.ReadOnly = true;
			buildTextBox.TabIndex = 3;
			buildTextBox.Size = new System.Drawing.Size(72, 20);
			Controls.Add(buildTextBox);
			
			sponsorLabel.Location = new System.Drawing.Point(8, 34);
			sponsorLabel.Text = LicenseSentence;
			sponsorLabel.Size = new System.Drawing.Size(362, 24);
			sponsorLabel.TabIndex = 8;
			Controls.Add(sponsorLabel);
			
			versionInfoTextBox.Location = new System.Drawing.Point(8, 34 + 28);
			versionInfoTextBox.Size = new System.Drawing.Size(362, 100);
			versionInfoTextBox.Multiline = true;
			versionInfoTextBox.ReadOnly = true;
			versionInfoTextBox.WordWrap = false;
			versionInfoTextBox.Text = GetVersionInformationString();
			versionInfoTextBox.ScrollBars = ScrollBars.Both;
			versionInfoTextBox.TabIndex = 9;
			versionInfoTextBox.Font = WinFormsResourceService.LoadFont("Courier New", 8);
			versionInfoTextBox.KeyDown += new KeyEventHandler(versionInfoTextBox_KeyDown);
			versionInfoTextBox.RightToLeft = RightToLeft.No;
			Controls.Add(versionInfoTextBox);
			
			Dock = DockStyle.Fill;
		}

		void versionInfoTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			#if DEBUG
			if (e.KeyData == (Keys.Control | Keys.Shift | Keys.E)) {
				throw new ClownFishException();
			} else if (e.KeyData == (Keys.Control | Keys.Shift | Keys.G)) {
				GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
				GC.WaitForPendingFinalizers();
				GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
				versionInfoTextBox.Text = GetVersionInformationString();
			}
			#endif
		}
		
		[Serializable]
		class ClownFishException : Exception { }
		
		public static string LicenseSentence {
			get {
				return StringParser.Parse("${res:Dialog.About.License}",
				                          new StringTagPair("License", "GNU Lesser General Public License"));
			}
		}
		
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static string GetVersionInformationString()
		{
			string str = "";
			object[] attr = typeof(AboutSharpDevelopTabPage).Assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
			if (attr.Length == 1) {
				AssemblyInformationalVersionAttribute aiva = (AssemblyInformationalVersionAttribute)attr[0];
				str += "SharpDevelop Version : " + aiva.InformationalVersion + Environment.NewLine;
			}
			str += ".NET Version         : " + Environment.Version.ToString() + Environment.NewLine;
			str += "OS Version           : " + Environment.OSVersion.ToString() + Environment.NewLine;
			string cultureName = null;
			try {
				cultureName = CultureInfo.CurrentCulture.Name;
				str += "Current culture      : " + CultureInfo.CurrentCulture.EnglishName + " (" + cultureName + ")" + Environment.NewLine;
			} catch {}
			try {
				if (cultureName == null || !cultureName.StartsWith(ResourceService.Language)) {
					str += "Current UI language  : " + ResourceService.Language + Environment.NewLine;
				}
			} catch {}
			try {
				if (IntPtr.Size != 4) {
					str += "Running as " + (IntPtr.Size * 8) + " bit process" + Environment.NewLine;
				}
				string PROCESSOR_ARCHITEW6432 = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432");
				if (!string.IsNullOrEmpty(PROCESSOR_ARCHITEW6432)) {
					if (PROCESSOR_ARCHITEW6432 == "AMD64")
						PROCESSOR_ARCHITEW6432 = "x86-64";
					str += "Running under WOW6432, processor architecture: " + PROCESSOR_ARCHITEW6432 + Environment.NewLine;
				}
			} catch {}
			try {
				if (SystemInformation.TerminalServerSession) {
					str += "Terminal Server Session" + Environment.NewLine;
				}
				if (SystemInformation.BootMode != BootMode.Normal) {
					str += "Boot Mode            : " + SystemInformation.BootMode + Environment.NewLine;
				}
			} catch {}
			str += "Working Set Memory   : " + (Environment.WorkingSet / 1024) + "kb" + Environment.NewLine;
			str += "GC Heap Memory       : " + (GC.GetTotalMemory(false) / 1024) + "kb" + Environment.NewLine;
			return str;
		}
	}
	
	public class VersionInformationTabPage : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button button;
		private System.Windows.Forms.ColumnHeader columnHeader;
		
		public VersionInformationTabPage()
		{
			InitializeComponent();
			Dock = DockStyle.Fill;
			FillListView();
		}
		
		void FillListView()
		{
			listView.BeginUpdate();
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
				AssemblyName name = asm.GetName();
				ListViewItem newItem = new ListViewItem(name.Name);
				newItem.SubItems.Add(name.Version.ToString());
				try {
					newItem.SubItems.Add(asm.Location);
				} catch (NotSupportedException) {
					// assembly.Location throws NotSupportedException for assemblies emitted using
					// Reflection.Emit by custom controls used in the forms designer
					newItem.SubItems.Add("dynamic");
				}
				
				listView.Items.Add(newItem);
			}
			listView.EndUpdate();
		}
		
		void CopyButtonClick(object sender, EventArgs e)
		{
			StringBuilder versionInfo = new StringBuilder();
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
				AssemblyName name = asm.GetName();
				versionInfo.Append(name.Name);
				versionInfo.Append(",");
				versionInfo.Append(name.Version.ToString());
				versionInfo.Append(",");
				try {
					versionInfo.Append(asm.Location);
				} catch (NotSupportedException) {
					// assembly.Location throws NotSupportedException for assemblies emitted using
					// Reflection.Emit by custom controls used in the forms designer
					versionInfo.Append("dynamic");
				}
				
				versionInfo.Append(Environment.NewLine);
			}
			
			ClipboardWrapper.SetText(versionInfo.ToString());
		}
		
		// THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
		// DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
		void InitializeComponent() {
			
			this.columnHeader = new System.Windows.Forms.ColumnHeader();
			this.button = new System.Windows.Forms.Button();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.listView = new System.Windows.Forms.ListView();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			
			// 
			// columnHeader
			// 
			this.columnHeader.Text = ResourceService.GetString("Global.Name");
			this.columnHeader.Width = 130;
			
			// 
			// button
			// 
			this.button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button.Location = new System.Drawing.Point(8, 184);
			this.button.Name = "button";
			this.button.TabIndex = 1;
			this.button.Text = ResourceService.GetString("Dialog.About.VersionInfoTabName.CopyButton");
			this.button.Click += new EventHandler(CopyButtonClick);
			this.button.FlatStyle = FlatStyle.System;
			
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = ResourceService.GetString("Dialog.About.VersionInfoTabName.VersionColumn");
			this.columnHeader2.Width = 100;
			
			// 
			// listView
			// 
			this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			                                                              | System.Windows.Forms.AnchorStyles.Left)
			                                                             | System.Windows.Forms.AnchorStyles.Right)));
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			                               	this.columnHeader,
			                               	this.columnHeader2,
			                               	this.columnHeader3});
			this.listView.FullRowSelect = true;
			this.listView.GridLines = true;
			this.listView.Sorting   = SortOrder.Ascending;
			this.listView.Location = new System.Drawing.Point(0, 0);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(248, 176);
			this.listView.TabIndex = 0;
			this.listView.View = System.Windows.Forms.View.Details;
			//this.listView.RightToLeftLayout = true;
			//this.listView.RightToLeft = RightToLeft;
			
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = ResourceService.GetString("Global.Path");
			this.columnHeader3.Width = 150;
			
			//
			// CreatedUserControl
			// 
			this.Controls.Add(this.button);
			this.Controls.Add(this.listView);
			this.Name = "CreatedUserControl";
			this.Size = new System.Drawing.Size(248, 216);
			this.ResumeLayout(false);
		}
	}
}
