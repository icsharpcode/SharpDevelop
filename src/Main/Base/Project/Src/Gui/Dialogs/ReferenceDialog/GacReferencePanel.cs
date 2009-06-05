// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

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
		ISelectReferenceDialog selectDialog;
		ColumnSorter sorter;
		
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
			
			chooseSpecificVersionCheckBox = new CheckBox();
			chooseSpecificVersionCheckBox.Dock = DockStyle.Top;
			chooseSpecificVersionCheckBox.Text = StringParser.Parse("${res:Dialog.SelectReferenceDialog.GacReferencePanel.ChooseSpecificAssemblyVersion}");
			this.Controls.Add(chooseSpecificVersionCheckBox);
			chooseSpecificVersionCheckBox.CheckedChanged += delegate {
				listView.Items.Clear();
				if (chooseSpecificVersionCheckBox.Checked)
					listView.Items.AddRange(fullItemList);
				else
					listView.Items.AddRange(shortItemList);
			};
			
			PrintCache();
		}
		
		void columnClick(object sender, ColumnClickEventArgs e)
		{
			if(e.Column < 2) {
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
					if (fullAssemblyNameToRequiredFrameworkVersionDictionary.TryGetValue(item.Tag.ToString(), out requiredFrameworkVersion)) {
						rpi.SetMetadata("RequiredTargetFramework", requiredFrameworkVersion);
					}
				} else {
					// find the lowest version of the assembly and use its RequiredTargetFramework
					ListViewItem lowestVersion = item;
					foreach (ListViewItem item2 in fullItemList) {
						if (item2.Text == item.Text) {
							if (new Version(item2.SubItems[1].Text) < new Version(((DomAssemblyName)lowestVersion.Tag).Version)) {
								lowestVersion = item2;
							}
						}
					}
					if (fullAssemblyNameToRequiredFrameworkVersionDictionary.TryGetValue(lowestVersion.Tag.ToString(), out requiredFrameworkVersion)) {
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
				ListViewItem item = new ListViewItem(new string[] {asm.ShortName, asm.Version});
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
						itemList[j].SubItems[1].Text += "/" + asm.Version;
						isDuplicate = true;
						break;
					}
				}
				if (!isDuplicate) {
					ListViewItem item = new ListViewItem(new string[] {asm.ShortName, asm.Version});
					item.Tag = asm;
					itemList.Add(item);
				}
			}
			
			shortItemList = itemList.ToArray();
			
			listView.Items.AddRange(shortItemList);
			
			Thread resolveVersionsThread = new Thread(ResolveVersionsWorker);
			resolveVersionsThread.SetApartmentState(ApartmentState.STA);
			resolveVersionsThread.IsBackground = true;
			resolveVersionsThread.Name = "resolveVersionsThread";
			resolveVersionsThread.Priority = ThreadPriority.BelowNormal;
			resolveVersionsThread.Start();
		}
		
		void ResolveVersionsThread()
		{
			try {
				ResolveVersionsWorker();
			} catch (Exception ex) {
				MessageService.ShowError(ex);
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
			
			MSBuildInternals.ResolveAssemblyReferences(project, referenceItems.ToArray());
			
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
		
		readonly static Dictionary<string, string> fullAssemblyNameToRequiredFrameworkVersionDictionary = new Dictionary<string, string> {
			{ "PresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "System.Printing, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "Microsoft.Build.Conversion.v3.5, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "3.5" },
			{ "Microsoft.Build.Engine, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "3.5" },
			{ "Microsoft.Build.Framework, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "3.5" },
			{ "Microsoft.Build.Tasks.v3.5, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "3.5" },
			{ "Microsoft.Build.Utilities.v3.5, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "3.5" },
			{ "Microsoft.VisualC.STLCLR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "3.5" },
			{ "PresentationBuildTasks, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "PresentationCFFRasterizer, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "PresentationFramework.Aero, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "PresentationFramework.Classic, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "PresentationFramework.Luna, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "PresentationFramework.Royale, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "PresentationUI, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "ReachFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "Sentinel.v3.5Client, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "3.5" },
			{ "System.AddIn, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.AddIn.Contract, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "3.5" },
			{ "System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.ComponentModel.DataAnnotations, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.5" },
			{ "System.Data.DataSetExtensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.Data.Entity.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.Data.Linq, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.Data.Services, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.Data.Services.Client, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.Data.Services.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.DirectoryServices.AccountManagement, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.IdentityModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.0" },
			{ "System.IdentityModel.Selectors, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.0" },
			{ "System.IO.Log, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "3.0" },
			{ "System.Management.Instrumentation, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.Net, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "3.5" },
			{ "System.Runtime.Serialization, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.0" },
			{ "System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.0" },
			{ "System.ServiceModel.Web, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.5" },
			{ "System.Speech, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "System.Web.Abstractions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.5" },
			{ "System.Web.DynamicData, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.5" },
			{ "System.Web.DynamicData.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.5" },
			{ "System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.Web.Entity.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.5" },
			{ "System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.5" },
			{ "System.Web.Routing, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.5" },
			{ "System.Windows.Presentation, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "System.Workflow.Activities, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "System.Workflow.ComponentModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "System.Workflow.Runtime, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "System.WorkflowServices, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.5" },
			{ "System.Xml.Linq, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "3.5" },
			{ "UIAutomationClient, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "UIAutomationClientsideProviders, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "UIAutomationProvider, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "UIAutomationTypes, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" },
			{ "WindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "3.0" }
		};
		
		#if DEBUG
		/// <summary>
		/// run this method with a .net 3.5 project to generate the table above.
		/// </summary>
		void CreateReferenceToFrameworkTable()
		{
			MSBuildBasedProject project = selectDialog.ConfigureProject as MSBuildBasedProject;
			if (project == null)
				return;
			
			var redistNameToRequiredFramework = new Dictionary<string, string> {
				{ "Framework", null },
				{ "Microsoft-Windows-CLRCoreComp", null },
				{ "Microsoft.VisualStudio.Primary.Interop.Assemblies.8.0", null },
				{ "Microsoft-WinFX-Runtime", "3.0" },
				{ "Microsoft-Windows-CLRCoreComp-v3.5", "3.5" }
			};
			
			using (StreamWriter w = new StreamWriter("c:\\temp\\references.txt")) {
				List<ReferenceProjectItem> referenceItems = new List<ReferenceProjectItem>();
				WorkbenchSingleton.SafeThreadCall(
					delegate {
						foreach (ListViewItem item in fullItemList) {
							referenceItems.Add(new ReferenceProjectItem(project, item.Tag.ToString()));
						}
					});
				
				MSBuildInternals.ResolveAssemblyReferences(project, referenceItems.ToArray());
				foreach (ReferenceProjectItem rpi in referenceItems) {
					if (string.IsNullOrEmpty(rpi.Redist)) continue;
					if (!redistNameToRequiredFramework.ContainsKey(rpi.Redist)) {
						throw new Exception("unknown redist: " + rpi.Redist);
					}
					if (redistNameToRequiredFramework[rpi.Redist] != null) {
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
			list.RemoveAll(name => name.ShortName.ToLowerInvariant().EndsWith(".resources"));
			return list;
		}
	}
}
