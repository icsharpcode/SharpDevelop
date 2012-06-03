// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Build.Tasks;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using Mono.Cecil;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class GacReferencePanel : UserControl, IReferencePanel
	{
		class ColumnSorter : System.Collections.IComparer
		{
			private int column = 0;
			bool asc = true;
			
			public int CurrentColumn
			{
				get
				{
					return column;
				}
				set
				{
					if(column == value) asc = !asc;
					else column = value;
				}
			}
			
			public int Compare(object x, object y)
			{
				ListViewItem rowA = (ListViewItem)x;
				ListViewItem rowB = (ListViewItem)y;
				int result = String.Compare(rowA.SubItems[CurrentColumn].Text, rowB.SubItems[CurrentColumn].Text);
				if(asc) return result;
				else return result * -1;
			}
		}

		protected ListView listView;
		CheckBox chooseSpecificVersionCheckBox;
		TextBox filterTextBox;
		ToolTip toolTip = new ToolTip();
		ToolTip filterTextboxToolTip = new ToolTip();
		ISelectReferenceDialog selectDialog;
		ColumnSorter sorter;
		BackgroundWorker worker;
		List<ListViewItem> resultList = new List<ListViewItem>();
		Dictionary<string, AssemblyDefinition> assembliesCache = new Dictionary<string, AssemblyDefinition>();
		DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
		
		public GacReferencePanel(ISelectReferenceDialog selectDialog)
		{
			listView = new ListView();
			sorter = new ColumnSorter();
			listView.ListViewItemSorter = sorter;
			
			this.selectDialog = selectDialog;
			
			ColumnHeader referenceHeader = new ColumnHeader();
			referenceHeader.Text  = ResourceService.GetString("Dialog.SelectReferenceDialog.GacReferencePanel.ReferenceHeader");
			referenceHeader.Width = 240;
			listView.Columns.Add(referenceHeader);
			
			listView.Sorting = SortOrder.Ascending;
			
			ColumnHeader versionHeader = new ColumnHeader();
			versionHeader.Text  = ResourceService.GetString("Dialog.SelectReferenceDialog.GacReferencePanel.VersionHeader");
			versionHeader.Width = 120;
			listView.Columns.Add(versionHeader);
			
			listView.View = View.Details;
			listView.FullRowSelect = true;
			listView.ItemActivate += delegate { AddReference(); };
			listView.ColumnClick += new ColumnClickEventHandler(columnClick);
			
			listView.Dock = DockStyle.Fill;
			this.Dock = DockStyle.Fill;
			this.Controls.Add(listView);
			
			Panel upperPanel = new Panel { Dock = DockStyle.Top, Height = 20 };
			
			chooseSpecificVersionCheckBox = new CheckBox();
			chooseSpecificVersionCheckBox.Dock = DockStyle.Left;
			chooseSpecificVersionCheckBox.AutoSize = true;
			chooseSpecificVersionCheckBox.Text = StringParser.Parse("${res:Dialog.SelectReferenceDialog.GacReferencePanel.ChooseSpecificAssemblyVersion}");
			
			chooseSpecificVersionCheckBox.CheckedChanged += delegate {
				ResetList();
				Search();
			};
			
			filterTextBox = new TextBox { Width = 150, Dock = DockStyle.Right };
			filterTextboxToolTip.SetToolTip(filterTextBox, "Search by type name");
			filterTextBox.TextChanged += delegate { Search(); };
			
			IButtonControl defaultButton = null;
			filterTextBox.Enter += delegate { defaultButton = ((Form)selectDialog).AcceptButton; ((Form)selectDialog).AcceptButton = null; };
			filterTextBox.Leave += delegate { ((Form)selectDialog).AcceptButton = defaultButton; };
			
			upperPanel.Controls.Add(chooseSpecificVersionCheckBox);
			upperPanel.Controls.Add(filterTextBox);
			
			this.Controls.Add(upperPanel);
			
			PrintCache();
		}

		void ResetList()
		{
			listView.Items.Clear();
			if (chooseSpecificVersionCheckBox.Checked)
				listView.Items.AddRange(fullItemList);
			else
				listView.Items.AddRange(shortItemList);
			
		}

		void Search()
		{
			ResetList();
			if (string.IsNullOrWhiteSpace(filterTextBox.Text))
				return;
			SearchItems(filterTextBox.Text);
			listView.Items.Clear();
			listView.Items.AddRange(resultList.ToArray());
		}
		
		void SearchItems(string text)
		{
			var searchList = listView.Items.OfType<ListViewItem>().ToList();
			searchList.RemoveWhere(item => item.Text.IndexOf(text, StringComparison.OrdinalIgnoreCase) < 0);
			resultList = searchList;
		}

		/// <summary>
		/// Clear all resources used.
		/// </summary>
		new void Dispose()
		{
			// cancel the worker
			if (worker != null && worker.IsBusy && !worker.CancellationPending)
				worker.CancelAsync();
			worker = null;
			
			// clear all cached data
			if (assembliesCache.Count > 0)
				assembliesCache.Clear();
			assembliesCache = null;
			
			if (resultList.Count > 0)
				resultList.Clear();
			resultList = null;
			
			selectDialog = null;
			resolver = null;
			
			if (fullItemList.Length > 0)
				Array.Clear(fullItemList, 0, fullItemList.Length);
			fullItemList = null;
			
			// force a collection to reclam memory
			GC.Collect();
		}
		
		void columnClick(object sender, ColumnClickEventArgs e)
		{
			if (e.Column < 2) {
				sorter.CurrentColumn = e.Column;
				listView.Sort();
			}
		}
		
		public void AddReference()
		{
			foreach (ListViewItem item in listView.SelectedItems) {
				string include = chooseSpecificVersionCheckBox.Checked ? item.Tag.ToString() : item.Text;
				ReferenceProjectItem rpi = new ReferenceProjectItem(selectDialog.ConfigureProject, include);
				string requiredFrameworkVersion;
				if (chooseSpecificVersionCheckBox.Checked) {
					if (KnownFrameworkAssemblies.TryGetRequiredFrameworkVersion(item.Tag.ToString(), out requiredFrameworkVersion)) {
						rpi.SetMetadata("RequiredTargetFramework", requiredFrameworkVersion);
					}
				} else {
					// find the lowest version of the assembly and use its RequiredTargetFramework
					ListViewItem lowestVersion = item;
					foreach (ListViewItem item2 in fullItemList) {
						if (item2.Text == item.Text) {
							if (new Version(item2.SubItems[1].Text) < ((DomAssemblyName)lowestVersion.Tag).Version) {
								lowestVersion = item2;
							}
						}
					}
					if (KnownFrameworkAssemblies.TryGetRequiredFrameworkVersion(lowestVersion.Tag.ToString(), out requiredFrameworkVersion)) {
						rpi.SetMetadata("RequiredTargetFramework", requiredFrameworkVersion);
					}
				}
				selectDialog.AddReference(
					item.Text, "Gac", rpi.Include,
					rpi
				);
			}
		}
		
		ListViewItem[] fullItemList;
		
		/// <summary>
		/// Item list where older versions are filtered out.
		/// </summary>
		ListViewItem[] shortItemList;
		
		void PrintCache()
		{
			IList<DomAssemblyName> cacheContent = GetCacheContent();
			
			List<ListViewItem> itemList = new List<ListViewItem>();
			// Create full item list
			foreach (DomAssemblyName asm in cacheContent) {
				ListViewItem item = new ListViewItem(new string[] {asm.ShortName, asm.Version.ToString()});
				item.Tag = asm;
				itemList.Add(item);
			}
			fullItemList = itemList.ToArray();
			
			// Create short item list (without multiple versions)
			itemList.Clear();
			for (int i = 0; i < cacheContent.Count; i++) {
				DomAssemblyName asm = cacheContent[i];
				bool isDuplicate = false;
				for (int j = 0; j < itemList.Count; j++) {
					if (string.Equals(asm.ShortName, itemList[j].Text, StringComparison.OrdinalIgnoreCase)) {
						itemList[j].SubItems[1].Text += "/" + asm.Version.ToString();
						isDuplicate = true;
						break;
					}
				}
				if (!isDuplicate) {
					ListViewItem item = new ListViewItem(new string[] {asm.ShortName, asm.Version.ToString()});
					item.Tag = asm;
					itemList.Add(item);
				}
			}
			
			shortItemList = itemList.ToArray();
			
			listView.Items.AddRange(shortItemList);
			
			Thread resolveVersionsThread = new Thread(ResolveVersionsThread);
			resolveVersionsThread.IsBackground = true;
			resolveVersionsThread.Name = "resolveVersionsThread";
			resolveVersionsThread.Priority = ThreadPriority.BelowNormal;
			resolveVersionsThread.Start();
		}
		
		void ResolveVersionsThread()
		{
			try {
				ResolveVersionsWorker();
				//CreateReferenceToFrameworkTable();
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		void ResolveVersionsWorker()
		{
			MSBuildBasedProject project = selectDialog.ConfigureProject as MSBuildBasedProject;
			if (project == null)
				return;
			
			List<ListViewItem> itemsToResolveVersion = new List<ListViewItem>();
			List<ReferenceProjectItem> referenceItems = new List<ReferenceProjectItem>();
			WorkbenchSingleton.SafeThreadCall(
				delegate {
					foreach (ListViewItem item in shortItemList) {
						if (item.SubItems[1].Text.Contains("/")) {
							itemsToResolveVersion.Add(item);
							referenceItems.Add(new ReferenceProjectItem(project, item.Text));
						}
					}
				});
			
			MSBuildInternals.ResolveAssemblyReferences(project, referenceItems.ToArray(), resolveOnlyAdditionalReferences: true, logErrorsToOutputPad: false);
			
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					if (IsDisposed) return;
					for (int i = 0; i < itemsToResolveVersion.Count; i++) {
						if (referenceItems[i].Version != null) {
							itemsToResolveVersion[i].SubItems[1].Text = referenceItems[i].Version.ToString();
						}
					}
				});
		}
		
		#if DEBUG
		/// <summary>
		/// run this method with a .net 3.5 and .net 4.0 project to generate the table above.
		/// </summary>
		void CreateReferenceToFrameworkTable()
		{
			LoggingService.Warn("Running CreateReferenceToFrameworkTable()");
			
			MSBuildBasedProject project = selectDialog.ConfigureProject as MSBuildBasedProject;
			if (project == null)
				return;
			
			var redistNameToRequiredFramework = new Dictionary<string, string> {
				{ "Framework", null },
				{ "Microsoft-Windows-CLRCoreComp", null },
				{ "Microsoft.VisualStudio.Primary.Interop.Assemblies.8.0", null },
				{ "Microsoft-WinFX-Runtime", "3.0" },
				{ "Microsoft-Windows-CLRCoreComp.3.0", "3.0" },
				{ "Microsoft-Windows-CLRCoreComp-v3.5", "3.5" },
				{ "Microsoft-Windows-CLRCoreComp.4.0", "4.0" },
			};
			
			using (StreamWriter w = new StreamWriter("c:\\temp\\references.txt")) {
				List<ReferenceProjectItem> referenceItems = new List<ReferenceProjectItem>();
				WorkbenchSingleton.SafeThreadCall(
					delegate {
						foreach (ListViewItem item in fullItemList) {
							referenceItems.Add(new ReferenceProjectItem(project, item.Tag.ToString()));
						}
					});
				
				MSBuildInternals.ResolveAssemblyReferences(project, referenceItems.ToArray(), resolveOnlyAdditionalReferences: true, logErrorsToOutputPad: false);
				foreach (ReferenceProjectItem rpi in referenceItems) {
					if (string.IsNullOrEmpty(rpi.Redist)) continue;
					if (!redistNameToRequiredFramework.ContainsKey(rpi.Redist)) {
						LoggingService.Error("unknown redist: " + rpi.Redist);
					} else if (redistNameToRequiredFramework[rpi.Redist] != null) {
						w.Write("\t\t\t{ \"");
						w.Write(rpi.Include);
						w.Write("\", \"");
						w.Write(redistNameToRequiredFramework[rpi.Redist]);
						w.WriteLine("\" },");
					}
				}
			}
		}
		#endif
		
		protected virtual IList<DomAssemblyName> GetCacheContent()
		{
			List<DomAssemblyName> list = GacInterop.GetAssemblyList();
			list.RemoveAll(name => name.ShortName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase));
			return list;
		}
		
		protected override void Dispose(bool disposing)
		{
			Dispose();
			
			base.Dispose(disposing);
		}
	}
}
