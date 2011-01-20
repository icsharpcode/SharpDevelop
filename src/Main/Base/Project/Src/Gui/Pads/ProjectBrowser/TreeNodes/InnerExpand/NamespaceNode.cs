// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

using ICSharpCode.Core;
using Mono.Cecil;

namespace ICSharpCode.SharpDevelop.Project.InnerExpand
{
	public class NamespaceNode : CustomFolderNode
	{
		readonly List<TypeDefinition> types;
		
		public NamespaceNode(string name, List<TypeDefinition> types)
		{
			SetIcon("Icons.16x16.NameSpace");
			Text = name;
			this.types = types;
			
			this.PerformInitialization();
		}
		
		public void RefreshNodes(bool forceRefresh = false)
		{
			if (Nodes.Count > 0 && !forceRefresh)
				return;
			
			Nodes.Clear();
			
			foreach (var type in types) {
				TypeNode node = null;
				string name = type.Name;
				
				if (type.IsValueType) {
					if (type.IsPublic) {
						node = new PublicStructNode(name, type);
					} else {
						node = new PrivateStructNode(name, type);
					}
				} else {
					if (type.IsEnum) {
						if (type.IsPublic) {
							node = new PublicEnumNode(name, type);
						} else {
							node = new PrivateEnumNode(name, type);
						}
					} else {
						
						if (type.BaseType != null && type.BaseType.FullName == "System.MulticastDelegate"){
							if (type.IsPublic) {
								node = new PublicDelegateNode(name, type);
							} else {
								node = new PrivateDelegateNode(name, type);
							}
						} else {
							if (type.IsClass) {
								if (type.IsPublic) {
									node = new PublicClassNode(name, type);
								} else {
									node = new PrivateClassNode(name, type);
								}
							}
							else {
								if (type.IsInterface) {
									if (type.IsPublic) {
										node = new PublicInterfaceNode(name, type);
									} else {
										node = new PrivateInterfaceNode(name, type);
									}
								}
							}
						}
					}
				}
				
				if (node != null)
					node.InsertSorted(this);
			}
		}
		
		public override void Expanding()
		{
			foreach (var node in Nodes) {
				if (!(node is TypeNode)) continue;
				
				var n = (TypeNode)node;
				n.ShowMembers();
			}
			
			base.Expanding();
		}
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}