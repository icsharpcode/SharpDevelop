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

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class TypeNode : AssemblyTreeNode
	{
		public bool MembersPopulated;
		
		public TypeNode(string name, IClass type) : base (name, type, NodeType.Type)
		{
		}
		
		protected override void SetIcon()
		{
			
			ImageIndex = SelectedImageIndex = ClassBrowserIconService.GetIcon((IClass)attribute);
		}
		
		public override void Populate(ShowOptions Private, ShowOptions Internal)
		{
			IClass type = attribute as IClass;

			Nodes.Clear();
			
			
			
			AssemblyTreeNode supertype = new AssemblyTreeNode(ress.GetString("ObjectBrowser.Nodes.SuperTypes"), type, NodeType.SuperTypes);
			Nodes.Add(supertype);
			
			SharpAssemblyClass sharptype = type as SharpAssemblyClass;
			if (sharptype == null) goto nobase;
			
			AddBaseTypes(sharptype, supertype, ClassBrowserIconService);
			
			// TODO: SubTypes is not implemented
			// Nodes.Add(new ReflectionNode("SubTypes", type, ReflectionNodeType.SubTypes));
			
		nobase:
			
			populated = true;
		}
		
		private void AddBaseTypes(SharpAssemblyClass type, AssemblyTreeNode node, ClassBrowserIconsService ClassBrowserIconService)
		{
			foreach (SharpAssemblyClass rettype in type.BaseTypeCollection) {
				AssemblyTreeNode basetype = new AssemblyTreeNode(rettype.Name, rettype, NodeType.Link);
				basetype.ImageIndex = basetype.SelectedImageIndex = ClassBrowserIconService.GetIcon(rettype);
				node.Nodes.Add(basetype);
				AddBaseTypes(rettype, basetype, ClassBrowserIconService);
			}
		}

		public void PopulateMembers(ShowOptions Private, ShowOptions Internal, bool Special)
		{
			IClass type = (IClass)attribute;
			ArrayList nodes = new ArrayList();
			TreeNodeComparer comp = new TreeNodeComparer();
			
			nodes.Clear();
			foreach (IMethod info in type.Methods) {
				if (Private == ShowOptions.Hide && info.IsPrivate) continue;
				if (Internal == ShowOptions.Hide && info.IsInternal) continue;
				if (!info.IsConstructor && info.IsSpecialName) continue;
				
				MemberNode node = new MemberNode(info);
				if ((info.IsInternal && Internal == ShowOptions.GreyOut) || 
				    (info.IsPrivate  && Private  == ShowOptions.GreyOut)) {
					node.ForeColor = SystemColors.GrayText;
				}
				
				nodes.Add(node);
			}
			nodes.Sort(comp);
			foreach (AssemblyTreeNode tn in nodes) {
				Nodes.Add(tn);
			}
			
			nodes.Clear();
			foreach (IProperty info in type.Properties) {
				if (Private == ShowOptions.Hide && info.IsPrivate) continue;
				if (Internal == ShowOptions.Hide && info.IsInternal) continue;
				
				MemberNode node = new MemberNode(info, Special);
				if ((info.IsInternal && Internal == ShowOptions.GreyOut) || 
				    (info.IsPrivate  && Private  == ShowOptions.GreyOut)) {
					node.ForeColor = SystemColors.GrayText;
				}
				nodes.Add(node);
			}
			nodes.Sort(comp);
			foreach (AssemblyTreeNode tn in nodes) {
				Nodes.Add(tn);
			}
			
			nodes.Clear();
			foreach (IField info in type.Fields) {
				if (Private == ShowOptions.Hide && info.IsPrivate) continue;
				if (Internal == ShowOptions.Hide && info.IsInternal) continue;
				if (info.IsSpecialName) continue;
				
				MemberNode node = new MemberNode(info, type.ClassType == ClassType.Enum);
				if ((info.IsInternal && Internal == ShowOptions.GreyOut) || 
				    (info.IsPrivate  && Private  == ShowOptions.GreyOut)) {
					node.ForeColor = SystemColors.GrayText;
				}
				nodes.Add(node);
			}
			nodes.Sort(comp);
			foreach (AssemblyTreeNode tn in nodes) {
				Nodes.Add(tn);
			}
			
			nodes.Clear();
			foreach (IEvent info in type.Events) {
				if (Private == ShowOptions.Hide && info.IsPrivate) continue;
				if (Internal == ShowOptions.Hide && info.IsInternal) continue;
				
				MemberNode node = new MemberNode(info, Special);
				if ((info.IsInternal && Internal == ShowOptions.GreyOut) || 
				    (info.IsPrivate  && Private  == ShowOptions.GreyOut)) {
					node.ForeColor = SystemColors.GrayText;
				}
				nodes.Add(node);
			}
			nodes.Sort(comp);
			foreach (AssemblyTreeNode tn in nodes) {
				Nodes.Add(tn);
			}
			
			MembersPopulated = true;
		}

	}
}
