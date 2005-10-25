// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Threading;
using System.Resources;
using System.Text;

using Microsoft.Win32;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using SA = ICSharpCode.SharpAssembly.Assembly;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class AssemblyTree : TreeView
	{
		public static IAmbience CurrentAmbience = AmbienceService.CurrentAmbience;
				
		ArrayList assemblies = new ArrayList();
		AssemblyScoutViewContent _parent;
		
		public ShowOptions showInternalTypes, showInternalMembers;
		public ShowOptions showPrivateTypes, showPrivateMembers;
		
		public bool showSpecial = false;
		
		MenuItem mnuBack;
		MenuItem mnuLoadAsm, mnuLoadStd, mnuLoadRef;
		MenuItem mnuRemAsm, mnuCopyTree, mnuSaveRes, mnuJump, mnuOpenRef, mnuDisAsm;
		
		Stack history = new Stack();
		bool histback = false;
		
		AssemblyTreeNode selnode;
		
		public event EventHandler Changed;
		
		public AssemblyTree(AssemblyScoutViewContent parent) : base()
		{
			if (Changed != null) {} // only to prevent these pesky compiler warning :) M.K.
			
			Dock = DockStyle.Fill;
			
			string resPrefix = "ObjectBrowser.Menu.";
						
			this.ImageList = ClassBrowserIconService.ImageList;
			
			LabelEdit     = false;
			HotTracking   = false;
			AllowDrop     = true;
			HideSelection = false;
			
			mnuBack    = new MenuItem(StringParser.Parse("${res:" + resPrefix + "GoBack}"), new EventHandler(Back));
			mnuLoadAsm = new MenuItem(StringParser.Parse("${res:" + resPrefix + "LoadAssembly}"), new EventHandler(LoadAssembly));
			mnuLoadStd = new MenuItem(StringParser.Parse("${res:" + resPrefix + "LoadStd}"), new EventHandler(LoadStdAssemblies));
			mnuLoadRef = new MenuItem(StringParser.Parse("${res:" + resPrefix + "LoadRef}"), new EventHandler(LoadRefAssemblies));
			
			mnuRemAsm   = new MenuItem(StringParser.Parse("${res:" + resPrefix + "RemoveAsm}"), new EventHandler(RemoveAssembly));
			mnuCopyTree = new MenuItem(StringParser.Parse("${res:" + resPrefix + "CopyTree}"), new EventHandler(CopyAssemblyTree));
			mnuSaveRes  = new MenuItem(StringParser.Parse("${res:" + resPrefix + "SaveRes}"), new EventHandler(SaveCurrentResource));
			mnuJump     = new MenuItem(StringParser.Parse("${res:" + resPrefix + "JumpType}"), new EventHandler(JumpLink));
			mnuOpenRef  = new MenuItem(StringParser.Parse("${res:" + resPrefix + "OpenRef}"), new EventHandler(OpenReference));
			mnuDisAsm   = new MenuItem(StringParser.Parse("${res:" + resPrefix + "DisasmToFile}"), new EventHandler(DisAssembly));
			
			ContextMenu = new ContextMenu(new MenuItem[] {
				mnuBack,
				new MenuItem("-"),
				mnuLoadAsm,
				mnuLoadStd,
				mnuLoadRef,
				new MenuItem("-"),
				mnuRemAsm,
				mnuCopyTree,
				mnuSaveRes,
				mnuJump,
				mnuOpenRef,
				mnuDisAsm
			});
			
			showPrivateTypes    = (ShowOptions)PropertyService.Get("AddIns.AssemblyScout.privateTypesBox", 1);
			showInternalTypes   = (ShowOptions)PropertyService.Get("AddIns.AssemblyScout.internalTypesBox", 1);
			showPrivateMembers  = (ShowOptions)PropertyService.Get("AddIns.AssemblyScout.privateMembersBox", 1);
			showInternalMembers = (ShowOptions)PropertyService.Get("AddIns.AssemblyScout.internalMembersBox", 1);
			showSpecial = PropertyService.Get("AddIns.AssemblyScout.ShowSpecialMethods", false);
			
			_parent = parent;
		}
		
		public ArrayList Assemblies {
			get {
				return assemblies;
			}
		}
		
		public PrintDocument PrintDocument {
			get {
				return null;
			}
		}
		
		public bool WriteProtected {
			get {
				return false;
			}
			set {
			}
		}
		
		public void LoadFile(string fileName) 
		{
			AddAssembly(SA.SharpAssembly.LoadFrom(fileName));
		}
		
		public bool IsAssemblyLoaded(string filename)
		{
			try {
				foreach(SA.SharpAssembly asm in assemblies) {
					if (asm.Location == filename) return true;
				}
			} catch {
			}
			return false;
		}
		
		public void SaveFile(string filename)
		{
		}
		
		public void AddAssembly(SA.SharpAssembly assembly)
		{
			try {
				if (IsAssemblyLoaded(assembly.Location)) return;
				
				assemblies.Add(assembly);
				
				TreeNode node = new FolderNode(System.IO.Path.GetFileNameWithoutExtension(assembly.Location), assembly, NodeType.Assembly, 0, 1);
				Nodes.Add(node);
				PopulateTree((AssemblyTreeNode)node);
			} catch (Exception e) {
				MessageBox.Show("Could not load assembly: " + e.ToString());
			}
		}

		public void RePopulateTreeView()
		{
			foreach (AssemblyTreeNode node in Nodes) {
				node.Populate(showPrivateTypes, showInternalTypes);
				PopulateTree(node);
			}
		}
		
		public void PopulateTree(AssemblyTreeNode parentnode)
		{
			if (!parentnode.Populated)
				parentnode.Populate(showPrivateTypes, showInternalTypes);
			
			foreach (AssemblyTreeNode node in parentnode.Nodes) {
				if (!node.Populated) {
					node.Populate(showPrivateTypes, showInternalTypes);
				}
				PopulateTree(node);
			}
		}
		
		public void GoToMember(IMember member) 
		{
			string paramtext = "";
			SharpAssemblyClass declaringType = member.DeclaringType as SharpAssemblyClass;
			TypeNode typenode = GetTypeNode((SA.SharpAssembly)declaringType.DeclaredIn, declaringType.Namespace, declaringType.FullyQualifiedName);
			if (typenode == null) return;
			
			bool isEnum = false;
			if (member.DeclaringType != null && member.DeclaringType.ClassType == ClassType.Enum) isEnum = true;
			
			paramtext = MemberNode.GetShortMemberName(member, isEnum);
			
			if (!typenode.MembersPopulated) {
				typenode.PopulateMembers(showPrivateMembers, showInternalMembers, showSpecial);
			}
			
			TreeNode foundnode = typenode.GetNodeFromChildren(paramtext);
			if (foundnode == null) return;
			
			foundnode.EnsureVisible();
			SelectedNode = foundnode;
		}
		
		public void GoToNamespace(SA.SharpAssembly asm, string name)
		{
			foreach (AssemblyTreeNode node in Nodes) {
				SA.SharpAssembly assembly = (SA.SharpAssembly)node.Attribute;
				if (asm.FullName == assembly.FullName) {
					
					// curnode contains Filename node
					AssemblyTreeNode curnode = (AssemblyTreeNode)node.GetNodeFromChildren(Path.GetFileName(assembly.Location));
					
					TreeNode tnode = curnode.GetNodeFromChildren(name); // get namespace node
					if (tnode == null) return;
					tnode.EnsureVisible();
					SelectedNode = tnode;
					return;
				}
			}
			
			// execute if assembly containing the type is not loaded
			AddAssembly(asm);
			GoToNamespace(asm, name);			
		}
		
		private TypeNode GetTypeNode(SA.SharpAssembly targetAssembly, string Namespace, string FullName)
		{
			foreach (AssemblyTreeNode node in Nodes) {
				SA.SharpAssembly assembly = (SA.SharpAssembly)node.Attribute;
				if (targetAssembly.FullName == assembly.FullName) {
					
					// curnode contains Filename node
					AssemblyTreeNode curnode = (AssemblyTreeNode)node.GetNodeFromChildren(Path.GetFileName(assembly.Location));
					
					TreeNode path;
					
					if (Namespace == null || Namespace == "") {
						path = curnode;
					} else {
						TreeNode tnode = curnode.GetNodeFromChildren(Namespace); // get namespace node
						if (tnode == null) {
							return null; // TODO : returns null if the tree isn't up to date.
						} else {
							path = tnode;
						}
					}

					string nodename = FullName.Substring(Namespace.Length + 1);
										
					TreeNode foundnode = node.GetNodeFromCollection(path.Nodes, nodename);
					return (TypeNode)foundnode;
				}
			}
			
			// execute if assembly containing the type is not loaded
			AddAssembly(targetAssembly);
			return GetTypeNode(targetAssembly, Namespace, FullName);
			
		}
		
		public void GoToType(IClass type)
		{
			AssemblyTreeNode node = GetTypeNode((SA.SharpAssembly)((SharpAssemblyClass)type).DeclaredIn, type.Namespace, type.FullyQualifiedName);
			if (node == null) {
				Console.WriteLine("No node for type found");
				return;
			}
			
			node.EnsureVisible();
			SelectedNode = node;
		}
		
		public void GoToType(IReturnType type)
		{
			AssemblyTreeNode node = GetTypeNode((SA.SharpAssembly)((SharpAssemblyClass)type).DeclaredIn, type.Namespace, type.FullyQualifiedName);
			if (node == null) return;
			
			node.EnsureVisible();
			SelectedNode = node;
		}
		
		protected override void OnDoubleClick(EventArgs e)
		{
			AssemblyTreeNode rn = (AssemblyTreeNode)SelectedNode;
			if (rn == null)
				return;
			switch (rn.Type) {
				
				case NodeType.Link: // clicked on link, jump to link.
					if (rn.Attribute is Type) {
						GoToType((IClass)rn.Attribute);
					}
					break;
				
				case NodeType.Reference: // clicked on assembly reference, open assembly
					// check if the assembly is open
					RefNodeAttribute attr = (RefNodeAttribute)rn.Attribute;
					OpenAssemblyByName(attr);
					break;
			}
		}
		
		public void OpenAssemblyByName(RefNodeAttribute attr)
		{
			foreach (AssemblyTreeNode node in Nodes) {
				if (node.Type == NodeType.Assembly) {
					if (attr.RefName.FullName == ((SA.SharpAssembly)node.Attribute).FullName) { // if yes, return
						node.EnsureVisible();
						SelectedNode = node;
						return;
					}
				}
			}
			try {
				AddAssembly(SA.SharpAssembly.Load(attr.RefName.FullName, System.IO.Path.GetDirectoryName(attr.Assembly.Location)));	
				OpenAssemblyByName(attr);
			} catch(Exception ex) {
				MessageBox.Show(String.Format(StringParser.Parse("${res:ObjectBrowser.LoadError}"), attr.RefName.Name, ex.Message), StringParser.Parse("${res:Global.ErrorText}"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
		
		protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
		{
			base.OnBeforeCollapse(e);
			((AssemblyTreeNode)e.Node).OnCollapse();
		}
		
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			base.OnBeforeExpand(e);
			
			AssemblyTreeNode rn = (AssemblyTreeNode)e.Node;
			if (!rn.Populated)
				rn.Populate(showPrivateTypes, showInternalTypes);
			
			if (e.Node is TypeNode) {
				TypeNode tn = e.Node as TypeNode;
				
				if (!tn.MembersPopulated)
					tn.PopulateMembers(showPrivateMembers, showInternalMembers, showSpecial);
			}
			
			((AssemblyTreeNode)e.Node).OnExpand();
		}
		
		protected override void OnMouseDown(MouseEventArgs ev)
		{
			base.OnMouseDown(ev);
			
			AssemblyTreeNode node = GetNodeAt(ev.X, ev.Y) as AssemblyTreeNode;
			if (node != null) {
				if (ev.Button == MouseButtons.Right) histback = true;
				SelectedNode = node;
				histback = false;
				mnuRemAsm.Visible   = (node.Type == NodeType.Assembly);
				mnuDisAsm.Visible   = (node.Type == NodeType.Assembly);
				mnuCopyTree.Visible = (node.Type == NodeType.Library);
				mnuSaveRes.Visible  = (node.Type == NodeType.Resource);
				mnuJump.Visible     = (node.Type == NodeType.Link);
				mnuOpenRef.Visible  = (node.Type == NodeType.Reference);
				selnode = node;
			} else {
				mnuRemAsm.Visible   = false;
				mnuDisAsm.Visible   = false;
				mnuCopyTree.Visible = false;
				mnuSaveRes.Visible  = false;
				mnuJump.Visible     = false;
				mnuOpenRef.Visible  = false;
				selnode = null;
			}
 			
		}
		
		public void LoadAssembly(object sender, EventArgs e)
		{
			using (SelectReferenceDialog selDialog = new SelectReferenceDialog(new TempProject())) {
				if (selDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					
					foreach (ProjectItem item in selDialog.ReferenceInformations) {
						if (item.ItemType != ItemType.Reference) continue;
							
						if (!IsAssemblyLoaded(item.FileName)) {
							try {
								LoadFile(item.FileName);
							} catch (Exception) {}
						}
					}
				}
			}
		}

		void LoadStdAssemblies(object sender, EventArgs e)
		{
			_parent.LoadStdAssemblies();
		}

		void LoadRefAssemblies(object sender, EventArgs e)
		{
			_parent.LoadRefAssemblies();
		}
		/*
		void ShowPrivTypesEvt(object sender, EventArgs e)
		{
			showPrivateTypes = !showPrivateTypes;
			PropertyService.Set("ObjectBrowser.ShowPrivTypes", showPrivateTypes);
			mnuShowPrivTypes.Checked = showPrivateTypes;
			RePopulateTreeView();
		}

		void ShowIntTypesEvt(object sender, EventArgs e)
		{
			showInternalTypes = !showInternalTypes;
			PropertyService.Set("ObjectBrowser.ShowIntTypes", showInternalTypes);
			mnuShowIntTypes.Checked = showInternalTypes;			
			RePopulateTreeView();
		}

		void ShowPrivMemEvt(object sender, EventArgs e)
		{
			showPrivateMembers = !showPrivateMembers;
			PropertyService.Set("ObjectBrowser.ShowPrivMembers", showPrivateMembers);
			mnuShowPrivMem.Checked = showPrivateMembers;			
			RePopulateTreeView();
		}
		
		void ShowIntMemEvt(object sender, EventArgs e)
		{
			showInternalMembers = !showInternalMembers;
			PropertyService.Set("ObjectBrowser.ShowIntMembers", showInternalMembers);
			mnuShowIntMem.Checked = showInternalMembers;			
			RePopulateTreeView();
		}
				
		void ShowSpecialEvt(object sender, EventArgs e)
		{
			showSpecial = !showSpecial;
			PropertyService.Set("ObjectBrowser.ShowSpecialMethods", showSpecial);
			mnuShowSpecial.Checked = showSpecial;			
			RePopulateTreeView();
		}
		*/
		
		public void RemoveAssembly(object sender, EventArgs e)
		{
			if (selnode == null) return;
			if (selnode.Type != NodeType.Assembly) return;
			
			assemblies.Remove((SA.SharpAssembly)selnode.Attribute);
			selnode.Remove();
		}
		
		public void CopyAssemblyTree(object sender, EventArgs e)
		{
			if (selnode == null) return;
			if (selnode.Type != NodeType.Library) return;
			
			StringBuilder stb = new StringBuilder();
			
			stb.Append(selnode.Text + "\n");
			GetSubNodeText(selnode, stb, 1);
			
			Clipboard.SetDataObject(stb.ToString(), true);
			
		}
		
		private static void GetSubNodeText(TreeNode node, StringBuilder build, int indentLevel) {
			foreach (TreeNode tn in node.Nodes) {
				build.Append('\t', indentLevel);
				build.Append(tn.Text + "\n");
				GetSubNodeText(tn, build, indentLevel + 1);
			}
		}
		
		

		public void DisAssembly(object sender, EventArgs e)
		{
			if (selnode == null) return;
			if (selnode.Type != NodeType.Assembly) return;
			
			SA.SharpAssembly asm = (SA.SharpAssembly)selnode.Attribute;
			
			SaveFileDialog sdialog 	= new SaveFileDialog();
			sdialog.AddExtension 	= true;			
			sdialog.FileName 		= asm.Name;
			sdialog.Filter          = "IL files (*.il)|*.il";
			sdialog.DefaultExt      = ".il";
			sdialog.InitialDirectory = Path.GetDirectoryName(asm.Location);

			DialogResult dr = sdialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			sdialog.Dispose();
			if(dr != DialogResult.OK) return;
			
			try {
				string args = '"' + asm.Location + "\" /NOBAR /OUT=\"" + sdialog.FileName + "\" /ALL ";
				ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(FileUtility.NetSdkInstallRoot, "bin\\ildasm.exe"), args);
				
				psi.RedirectStandardError  = true;
				psi.RedirectStandardOutput = true;
				psi.RedirectStandardInput  = true;
				psi.UseShellExecute        = false;
				psi.CreateNoWindow         = true;
				
				Process process = Process.Start(psi);
				string output   = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
								
				MessageBox.Show(String.Format(StringParser.Parse("${res:ObjectBrowser.ILDasmOutput}"), output));
			} catch(Exception ex) {
				MessageBox.Show(String.Format(StringParser.Parse("${res:ObjectBrowser.ILDasmError}"),  ex.ToString()));
			}
		}
		
		public void SaveCurrentResource(object sender, EventArgs e)
		{
			if (selnode == null) return;
			if (selnode.Type != NodeType.Resource) return;
			
			SA.SharpAssembly asm = (SA.SharpAssembly)selnode.Attribute;
			SaveResource(asm, selnode.Text);
			
		}
		
		public void SaveResource(SA.SharpAssembly asm, string name)
		{
			SaveFileDialog sdialog 	= new SaveFileDialog();
			sdialog.AddExtension 	= true;			
			sdialog.FileName 		= name;
			sdialog.Filter          = StringParser.Parse("${res:ObjectBrowser.Filters.Binary}") + "|*.*";
			sdialog.DefaultExt      = ".bin";

			DialogResult dr = sdialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			sdialog.Dispose();
			if(dr != DialogResult.OK) return;
			
			try {
				byte[] res = asm.GetManifestResource(name);
				FileStream fstr = new FileStream(sdialog.FileName, FileMode.Create);
				BinaryWriter wr = new BinaryWriter(fstr);
				wr.Write(res);
				fstr.Close();
			} catch {
			}
			
		}
		
		public void JumpLink(object sender, EventArgs e)
		{
			if (selnode == null) return;
			if (selnode.Type != NodeType.Link) return;
			
			OnDoubleClick(e);
		}
		
		public void OpenReference(object sender, EventArgs e)
		{
			if (selnode == null) return;
			if (selnode.Type != NodeType.Reference) return;
			
			OnDoubleClick(e);
		}
		
		void Back(object sender, EventArgs e)
		{
			if (history.Count == 0) return;
			try {
				histback = true;
				TreeNode selnode = history.Pop() as TreeNode;
				if (selnode != null) {
					selnode.EnsureVisible();
					SelectedNode = selnode;
				}
			} finally {
				histback = false;
			}
		}
		
		protected override void OnBeforeSelect(TreeViewCancelEventArgs ev)
		{
			base.OnBeforeSelect(ev);
			if (!histback) {
				// HACK : stack is cleared if too much elements
				if (history.Count >= 100) history.Clear();
				history.Push(SelectedNode);
			}
		}
		
		public void GoBack()
		{
			try {
				Back(mnuBack, new EventArgs());
			} catch {}
		}
		
		public class RefNodeAttribute
		{
			public SA.SharpAssembly     Assembly;
			public SA.SharpAssemblyName RefName;
			
			public RefNodeAttribute(SA.SharpAssembly asm, SA.SharpAssemblyName name)
			{
				Assembly = asm;
				RefName  = name;
			}
		}
		
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseUp(e);
			
			if (e.Button == MouseButtons.XButton1) {
				GoBack();
			}
		}
		
	}

	public class TreeNodeComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			return String.Compare(((TreeNode)x).Text, ((TreeNode)y).Text);
		}
	}
	
	public enum ShowOptions
	{
		Show = 0,
		GreyOut = 1,
		Hide = 2
	}
}
