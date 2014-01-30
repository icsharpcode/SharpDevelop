// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

// created on 07.08.2003 at 13:46
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.FormsDesigner.Gui
{
	#pragma warning disable 618
	public class AddComponentsDialog : BaseSharpDevelopForm
	{
		ArrayList selectedComponents;
		ListView componentListView;
		
		public ArrayList SelectedComponents {
			get {
				return selectedComponents;
			}
		}
		
		public AddComponentsDialog()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.FormsDesigner.Resources.AddSidebarComponentsDialog.xfrm"));
			
			componentListView = (ListView)ControlDictionary["componentListView"];
			
			Icon = null;
			PrintGACCache();
			
			ControlDictionary["browseButton"].Click += new System.EventHandler(this.browseButtonClick);
			((ListView)ControlDictionary["gacListView"]).SelectedIndexChanged += new System.EventHandler(this.gacListViewSelectedIndexChanged);
//			((TextBox)ControlDictionary["fileNameTextBox"]).TextChanged += new System.EventHandler(this.fileNameTextBoxTextChanged);
			ControlDictionary["okButton"].Click += new System.EventHandler(this.buttonClick);
			ControlDictionary["loadButton"].Click += new System.EventHandler(this.loadButtonClick);
		}
		
		void PrintGACCache()
		{
			foreach (DomAssemblyName asm in SD.GlobalAssemblyCache.Assemblies) {
				ListViewItem item = new ListViewItem(new string[] {asm.ShortName, asm.Version.ToString()});
				item.Tag = asm.FullName;
				((ListView)ControlDictionary["gacListView"]).Items.Add(item);
			}
		}
		
		void BeginFillComponentsList()
		{
			componentListView.BeginUpdate();
			componentListView.Items.Clear();
			componentListView.Controls.Clear();
		}
		
		void EndFillComponentsList(Assembly lastAssembly)
		{
			if (componentListView.Items.Count == 0) {
				if (componentListView.Controls.Count == 0) {
					string name = String.Empty;
					if (lastAssembly != null) {
						name = lastAssembly.FullName;
					}
					ClearComponentsList(StringParser.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.NoComponentsFound}", new StringTagPair("Name", name)));
				}
			}
			componentListView.EndUpdate();
		}
		
		void AddComponentsToList(Assembly assembly, string loadPath)
		{
			if (assembly != null) {
				Hashtable images = new Hashtable();
				ImageList il = new ImageList();
				// try to load res icon
				string[] imgNames = assembly.GetManifestResourceNames();
				
				foreach (string im in imgNames) {
					try {
						Bitmap b = new Bitmap(Image.FromStream(assembly.GetManifestResourceStream(im)));
						b.MakeTransparent();
						images[im] = il.Images.Count;
						il.Images.Add(b);
					} catch {}
				}
				try {
					componentListView.SmallImageList = il;
					foreach (Type t in assembly.GetExportedTypes()) {
						if (!t.IsAbstract) {
							if (t.IsDefined(typeof(ToolboxItemFilterAttribute), true) || t.IsDefined(typeof(ToolboxItemAttribute), true) || typeof(System.ComponentModel.IComponent).IsAssignableFrom(t)) {
								object[] attributes  = t.GetCustomAttributes(false);
								object[] filterAttrs = t.GetCustomAttributes(typeof(DesignTimeVisibleAttribute), true);
								foreach (DesignTimeVisibleAttribute visibleAttr in filterAttrs) {
									if (!visibleAttr.Visible) {
										goto skip;
									}
								}
								
								if (images[t.FullName + ".bmp"] == null) {
									if (t.IsDefined(typeof(ToolboxBitmapAttribute), false)) {
										foreach (object attr in attributes) {
											if (attr is ToolboxBitmapAttribute) {
												ToolboxBitmapAttribute toolboxBitmapAttribute = (ToolboxBitmapAttribute)attr;
												images[t.FullName + ".bmp"] = il.Images.Count;
												Bitmap b = new Bitmap(toolboxBitmapAttribute.GetImage(t));
												b.MakeTransparent(Color.Fuchsia);
												il.Images.Add(b);
												break;
											}
										}
									}
								}
								
								ListViewItem newItem = new ListViewItem(t.Name);
								newItem.SubItems.Add(t.Namespace);
								newItem.SubItems.Add(assembly.ToString());
								newItem.SubItems.Add(assembly.Location);
								newItem.SubItems.Add(t.Namespace);
								if (images[t.FullName + ".bmp"] != null) {
									newItem.ImageIndex = (int)images[t.FullName + ".bmp"];
								}
								newItem.Checked  = true;
								ToolComponent toolComponent = new ToolComponent(t.FullName, new ComponentAssembly(assembly.FullName, loadPath), true);
								newItem.Tag = toolComponent;
								componentListView.Items.Add(newItem);
								ToolboxItem item = new ToolboxItem(t);
								skip:;
							}
						}
					}
				} catch (Exception e) {
					ClearComponentsList(e.Message);
				}
			}
		}
		
		void gacListViewSelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (((ListView)ControlDictionary["gacListView"]).SelectedItems != null && ((ListView)ControlDictionary["gacListView"]).SelectedItems.Count == 1) {
				string assemblyName = ((ListView)ControlDictionary["gacListView"]).SelectedItems[0].Tag.ToString();
				try {
					Assembly asm = Assembly.Load(assemblyName);
					BeginFillComponentsList();
					AddComponentsToList(asm, null);
					EndFillComponentsList(asm);
				} catch (Exception ex) {
					EndFillComponentsList(null);
					ClearComponentsList(ex.Message);
				}
			} else {
				ClearComponentsList(null);
			}
		}
		
		void ClearComponentsList(string message)
		{
			componentListView.Items.Clear();
			componentListView.Controls.Clear();
			if (message != null) {
				Label lbl = new Label();
				lbl.BackColor = SystemColors.Window;
				lbl.Text = StringParser.Parse(message);
				lbl.Dock = DockStyle.Fill;
				componentListView.Controls.Add(lbl);
			}
		}
		
		void loadButtonClick(object sender, System.EventArgs e)
		{
			BeginFillComponentsList();
			try {
				string assemblyFileNames = ControlDictionary["fileNameTextBox"].Text;
				Assembly lastAssembly = null;
				foreach (string assemblyFileName in assemblyFileNames.Split(';')) {
					if (!System.IO.File.Exists(assemblyFileName)) {
						EndFillComponentsList(null);
						ClearComponentsList(assemblyFileName + " was not found.");
						MessageService.ShowWarning("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.EnterValidFilename}");
						return;
					}
					
					Assembly asm = Assembly.LoadFrom(assemblyFileName);
					lastAssembly = asm;
					AddComponentsToList(asm, Path.GetDirectoryName(assemblyFileName));
				}
				EndFillComponentsList(lastAssembly);
			} catch {
				EndFillComponentsList(null);
				MessageService.ShowWarning("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.FileIsNotAssembly}");
				ClearComponentsList(null);
			}
		}
		
		void buttonClick(object sender, System.EventArgs e)
		{
			selectedComponents = new ArrayList();
			foreach (ListViewItem item in componentListView.Items) {
				if (item.Checked) {
					selectedComponents.Add((ToolComponent)item.Tag);
				}
			}
		}
		
		void browseButtonClick(object sender, System.EventArgs e)
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				
				fdiag.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.AssemblyFiles}|*.dll;*.exe|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.Multiselect     = true;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
					ControlDictionary["fileNameTextBox"].Text = string.Join(";", fdiag.FileNames);
				}
			}
		}
	}
}
