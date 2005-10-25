// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using UI = WeifenLuo.WinFormsUI;
using SA = ICSharpCode.SharpAssembly.Assembly;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	///////////////////////////////////////////
	// AssemblyScoutViewContent Class
	///////////////////////////////////////////
	public class AssemblyScoutViewContent : AbstractViewContent
	{		
		public UI.DockPanel leftTabs, rightTabs;
		Control control = null;
		AssemblyTree tree = null;
		
		public override Control Control {
			get {
				return control;
			}
		}
		
		public AssemblyTree Tree {
			get {
				return tree;
			}
		}
				
		string untitledName = "";
		public override string UntitledName {
			get {
				return untitledName;
			}
			set {
				untitledName = value;
			}
		}

//		public override string TitleName {
//			get {
//				return filename;
//			}
//			set {
//				filename = value;
//				OnTitleNameChanged(null);
//			}
//		}
		
		public override string TabPageText {
			get {
				return "Assemblies";
			}
		}
		
		public override bool IsViewOnly {
			get {
				return true;
			}
		}
	
		IWorkbenchWindow workbenchWindow;
		public override IWorkbenchWindow WorkbenchWindow {
			get {
				return workbenchWindow;
			}
			set {
				workbenchWindow = value;
				if (FileName == "") {
					workbenchWindow.Title = StringParser.Parse("${res:ObjectBrowser.AssemblyScout}");
				} else {
					workbenchWindow.Title = FileName;
				}
			}
		}
		
		public override void RedrawContent()
		{
		}
		
		public override void Dispose()
		{
			try {
				foreach(Control ctl in Control.Controls) {
					ctl.Dispose();
				}
			} catch {
				return;
			}
		}
				
		public void SaveFile()
		{
		}
		
		public bool CanCreateContentForFile(string fileName)
		{
			return Path.GetExtension(fileName) == ".dll" || Path.GetExtension(fileName) == ".exe";
		}
		
		public bool CanCreateContentForLanguage(string language)
		{
			return false;
		}
		
		public IViewContent CreateContentForFile(string fileName)
		{
			Load(fileName);
			return this;
		}

		public void Undo()
		{
		}

		public void Redo()
		{
		}

		public IViewContent CreateContentForLanguage(string language, string content)
		{
			return null;
		}
		
		public override void Save()
		{
		}
		
		public override void Save(string filename)
		{
		}
		
		public override void Load(string filename)
		{
			tree.LoadFile(filename);
			this.FileName = filename;
			this.TitleName = Path.GetFileName(filename);
		}
		
		public AssemblyScoutViewContent()
		{
			Panel panel = new Panel();
			panel.Dock = DockStyle.Fill;
			
			leftTabs           = new UI.DockPanel();
			leftTabs.Dock      = DockStyle.Left;
			leftTabs.Width     = 400;
			leftTabs.AllowDrop = false;
			leftTabs.AllowRedocking = false;
						
			AssemblyTree assemblyTree      = new AssemblyTree(this);
			this.tree = assemblyTree;
			
			UI.DockContent treeviewpage = new UI.DockContent();
			treeviewpage.Text = StringParser.Parse("${res:ObjectBrowser.Tree}");
			treeviewpage.Icon                  = ResourceService.GetIcon("Icons.16x16.Class");
			treeviewpage.DockPadding.All       = 8;
			treeviewpage.Controls.Add(assemblyTree);
			treeviewpage.DockableAreas = UI.DockAreas.Document;
			treeviewpage.CloseButton = false;
			treeviewpage.Show(leftTabs);
			
			UI.DockContent indexviewpage = new UI.DockContent();
			indexviewpage.Text = StringParser.Parse("${res:ObjectBrowser.Search}");
			indexviewpage.Icon                  = ResourceService.GetIcon("Icons.16x16.FindIcon");
			SearchPanel SearchPanel   = new SearchPanel(assemblyTree);
			SearchPanel.ParentDisplayInfo       = this;
			indexviewpage.DockPadding.All       = 8;
			indexviewpage.Controls.Add(SearchPanel);
			indexviewpage.DockableAreas = UI.DockAreas.Document;
			indexviewpage.CloseButton = false;
			indexviewpage.Show(leftTabs);
			
			Splitter vsplitter    = new Splitter();
			vsplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

			vsplitter.Location    = new System.Drawing.Point(0, 200);
			vsplitter.TabIndex    = 5;
			vsplitter.TabStop     = false;
			vsplitter.Size        = new System.Drawing.Size(3, 273);
			vsplitter.Dock        = DockStyle.Left;
			
			
			rightTabs = new UI.DockPanel();
			rightTabs.Dock       = DockStyle.Fill;
			rightTabs.AllowDrop  = false;
			rightTabs.AllowRedocking = false;
						
			UI.DockContent memberpage = new UI.DockContent();
			memberpage.Text = StringParser.Parse("${res:ObjectBrowser.Info}");
			memberpage.Icon                  = ResourceService.GetIcon("Icons.16x16.Information");
			memberpage.DockPadding.All       = 8;
			memberpage.Controls.Add(new InfoView(assemblyTree));
			memberpage.DockableAreas = UI.DockAreas.Document;
			memberpage.CloseButton = false;
			memberpage.Show(rightTabs);
			
			UI.DockContent ildasmviewpage = new UI.DockContent();
			ildasmviewpage.Text = StringParser.Parse("${res:ObjectBrowser.Disasm}");
			ildasmviewpage.Icon                  = ResourceService.GetIcon("Icons.16x16.ILDasm");
			ildasmviewpage.DockPadding.All       = 8;
			ildasmviewpage.Controls.Add(new ILDasmView(assemblyTree));
			ildasmviewpage.DockableAreas = UI.DockAreas.Document;
			ildasmviewpage.CloseButton = false;
			ildasmviewpage.Show(rightTabs);
			
			UI.DockContent sourceviewpage = new UI.DockContent();
			sourceviewpage.Text = StringParser.Parse("${res:ObjectBrowser.Source}");
			sourceviewpage.Icon                  = ResourceService.GetIcon("Icons.16x16.TextFileIcon");
			sourceviewpage.DockPadding.All       = 8;
			sourceviewpage.Controls.Add(new SourceView(assemblyTree));
			sourceviewpage.DockableAreas = UI.DockAreas.Document;
			sourceviewpage.CloseButton = false;
			sourceviewpage.Show(rightTabs);
			
			UI.DockContent xmlviewpage = new UI.DockContent();
			xmlviewpage.Text = StringParser.Parse("${res:ObjectBrowser.XML}");
			xmlviewpage.Icon                  = ResourceService.GetIcon("Icons.16x16.XMLFileIcon");
			xmlviewpage.DockPadding.All       = 8;
			xmlviewpage.Controls.Add(new XmlView(assemblyTree));
			xmlviewpage.DockableAreas = UI.DockAreas.Document;
			xmlviewpage.CloseButton = false;
			xmlviewpage.Show(rightTabs);
			
			UI.DockContent extproppage = new UI.DockContent();
			extproppage.Text = StringParser.Parse("${res:ObjectBrowser.Extended}");
			extproppage.Icon                  = ResourceService.GetIcon("Icons.16x16.Property");
			extproppage.DockPadding.All       = 8;
			extproppage.Controls.Add(new ExtendedPropsPanel(assemblyTree));
			extproppage.DockableAreas = UI.DockAreas.Document;
			extproppage.CloseButton = false;
			extproppage.Show(rightTabs);
			
			panel.Controls.Add(rightTabs);
			panel.Controls.Add(vsplitter);
			panel.Controls.Add(leftTabs);
			
			treeviewpage.Activate();
			memberpage.Activate();
			
			this.control = panel;
			this.TitleName = StringParser.Parse("${res:ObjectBrowser.AssemblyScout}");
		}
		
		public void LoadStdAssemblies() {
			//try {
			tree.AddAssembly(SA.SharpAssembly.Load("mscorlib"));
			tree.AddAssembly(SA.SharpAssembly.Load("System"));
			tree.AddAssembly(SA.SharpAssembly.Load("System.Xml"));
			tree.AddAssembly(SA.SharpAssembly.Load("System.Windows.Forms"));
			tree.AddAssembly(SA.SharpAssembly.Load("System.Drawing"));
			tree.AddAssembly(SA.SharpAssembly.Load("System.Data"));
			tree.AddAssembly(SA.SharpAssembly.Load("System.Design"));			
			tree.AddAssembly(SA.SharpAssembly.Load("System.Web"));			
			//} catch {}
		}
		
		public void LoadRefAssemblies() {
			try {
				if (ProjectService.CurrentProject == null) return;
				foreach(ProjectItem item in ProjectService.CurrentProject.Items) {
					if (item.ItemType == ItemType.Reference) {
						if (!tree.IsAssemblyLoaded(item.FileName)) {
							try {
								tree.LoadFile(item.FileName);
							} catch (Exception) {
								//MessageBox.Show("Object Browser error:\nError loading assembly " + pr.GetReferencedFileName(null) + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							}
						}
					}
				}
			} catch (Exception) {}
		}
	}
}
