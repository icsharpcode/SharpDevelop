// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;

using ICSharpCode.Core;
using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Dom;
using SA = ICSharpCode.SharpAssembly.Assembly;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public enum NodeType {
		Folder,
		Resource,
		Assembly,
		Library,
		Namespace,
		Type,
		Constructor,
		Method,
		Field,
		Property,
		SubTypes,
		SuperTypes,
		Reference,
		Event,
		Link,
		Module,
		SingleResource,
	}
	
	public class AssemblyTreeNode : TreeNode
	{
		protected const int CLASSINDEX     = 14;
		protected const int STRUCTINDEX    = CLASSINDEX + 1 * 4;
		protected const int INTERFACEINDEX = CLASSINDEX + 2 * 4;
		protected const int ENUMINDEX      = CLASSINDEX + 3 * 4;
		protected const int METHODINDEX    = CLASSINDEX + 4 * 4;
		protected const int PROPERTYINDEX  = CLASSINDEX + 5 * 4;
		protected const int FIELDINDEX     = CLASSINDEX + 6 * 4;
		protected const int DELEGATEINDEX  = CLASSINDEX + 7 * 4;
		
		protected NodeType type;
		protected string   name;
		protected object   attribute;
		
		protected bool  populated = false;
		
		public static ResourceService ress = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));

		public NodeType Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		public object Attribute {
			get {
				return attribute;
			}
		}
		
		public bool Populated {
			get {
				return populated;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public AssemblyTreeNode(string name, object attribute, NodeType type) : base(name)
		{
			this.attribute = attribute;
			this.type = type;
			this.name = name;
		
			SetIcon();
		}
		
	
		protected virtual void SetIcon()
		{
			
			
			switch (type) {
				case NodeType.Link:
					break;
				
				case NodeType.Resource:
				case NodeType.SingleResource: // TODO : single res icon
					ImageIndex  = SelectedImageIndex = 11;
					break;
				
				case NodeType.Reference:
					ImageIndex  = SelectedImageIndex = 8;
					break;
				
				case NodeType.Module:
					ImageIndex  = SelectedImageIndex = 46;
					break;
				
				case NodeType.SubTypes:
					ImageIndex  = SelectedImageIndex = 4;
					break;
					
				case NodeType.SuperTypes:
					ImageIndex  = SelectedImageIndex = 5;
					break;
				
				default:
					throw new Exception("ReflectionFolderNode.SetIcon : unknown ReflectionNodeType " + type.ToString());
			}
		}
		
		public virtual void Populate(ShowOptions Private, ShowOptions Internal)
		{
			switch (type) {
				case NodeType.Assembly:
					PopulateAssembly((SA.SharpAssembly)attribute, this);
					break;
				
				case NodeType.Library:
					PopulateLibrary((SA.SharpAssembly)attribute, this, Private, Internal);
					break;
			}
			populated = true;
		}
		
		public AssemblyTreeNode GetNodeFromChildren(string title)
		{
			foreach (AssemblyTreeNode node in this.Nodes) {
				if (node.Text == title) {
					return node;
				}
			}
			return null;
		}
		
		public AssemblyTreeNode GetNodeFromCollection(TreeNodeCollection collection, string title)
		{
			foreach (AssemblyTreeNode node in collection)
				if (node.Text == title) {
					return node;
				}
			return null;
		}
		
		void PopulateLibrary(SA.SharpAssembly assembly, TreeNode parentnode, ShowOptions Private, ShowOptions Internal)
		{
			parentnode.Nodes.Clear();
			IClass[] types = new IClass[0];
			
			try {
				types = SharpAssemblyClass.GetAssemblyTypes(assembly);
			} catch {
				MessageBox.Show(ress.GetString("ObjectBrowser.ErrorLoadingTypes"), ress.GetString("Global.WarningText"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);					
			}
			
			ArrayList nodes = new ArrayList();
			ArrayList namespaces = new ArrayList();
			ArrayList namespacenames = new ArrayList();
			TreeNodeComparer comp = new TreeNodeComparer();
			
			foreach (IClass type in types) {
				if(type.FullyQualifiedName.IndexOf("PrivateImplementationDetails") != -1) continue;
				if(type.IsInternal && Internal == ShowOptions.Hide) continue;
				if(type.IsPrivate && Private == ShowOptions.Hide) continue;

				TypeNode typenode = new TypeNode(GetShortTypeName(type.FullyQualifiedName), type);
				if ((type.IsInternal && Internal == ShowOptions.GreyOut) || 
				    (type.IsPrivate  && Private  == ShowOptions.GreyOut)) {
					typenode.ForeColor = SystemColors.GrayText;
				}
				nodes.Add(typenode);
				if (type.Namespace != null && type.Namespace != "") {
					if (!namespacenames.Contains(type.Namespace)) {
						namespaces.Add(new FolderNode(type.Namespace, assembly, NodeType.Namespace, 3, 3));
						namespacenames.Add(type.Namespace);
					}
				}
			}
			nodes.Sort(comp);
			namespaces.Sort(comp);
			foreach (TreeNode tn in namespaces) {
				parentnode.Nodes.Add(tn);
			}
			foreach (TreeNode tn in nodes) {
				IClass type = (IClass)((AssemblyTreeNode)tn).Attribute;
				if (type.Namespace != null && type.Namespace != "") {
					GetNodeFromCollection(parentnode.Nodes, type.Namespace).Nodes.Add(tn);
				} else {
					parentnode.Nodes.Add(tn);
				}
			}
		}
		
		string GetShortTypeName(string typename)
		{
			if (typename == null) return "";
			int lastIndex;
			
			lastIndex = typename.LastIndexOf('.');
			
			if (lastIndex < 0) {
				return typename;
			} else {
				return typename.Substring(lastIndex + 1);
			}
			
		}
		
		void PopulateAssembly(SA.SharpAssembly assembly, TreeNode parentnode)
		{
			parentnode.Nodes.Clear();
			
			TreeNode node = new FolderNode(System.IO.Path.GetFileName(assembly.Location), assembly, NodeType.Library, 2, 2);
			parentnode.Nodes.Add(node);
			
			FolderNode resourcefolder = new FolderNode(ress.GetString("ObjectBrowser.Nodes.Resources"), assembly, NodeType.Folder, 6, 7);
			string[] resources = assembly.GetManifestResourceNames();
			foreach (string resource in resources) {
				resourcefolder.Nodes.Add(new ResourceNode(resource, assembly, true));
			}
			parentnode.Nodes.Add(resourcefolder);
			
			FolderNode referencefolder = new FolderNode(ress.GetString("ObjectBrowser.Nodes.References"), assembly, NodeType.Folder, 9, 10);
			SA.SharpAssemblyName[] references = assembly.GetReferencedAssemblies();
			foreach (SA.SharpAssemblyName name in references) {
				referencefolder.Nodes.Add(new AssemblyTreeNode(name.Name, new AssemblyTree.RefNodeAttribute(assembly, name), NodeType.Reference));
			}
			parentnode.Nodes.Add(referencefolder);
		}
		
		public virtual void OnExpand()
		{
		}
		
		public virtual void OnCollapse()
		{
		}
	}
}
