// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using NoGoop.Obj;
using NoGoop.ObjBrowser.TreeNodes;

namespace NoGoop.ObjBrowser.LinkHelpers
{
	// A single instance of this exists whose purpose is to
	// resolve links to types in the assembly/type tree.
	// The linkModifier is the type to be resolved.
	// This supports links to members, types and typelibs
	internal class ComLinkHelper : ILinkTarget
	{
		protected static ComLinkHelper _comLinkHelper;
		
		public static ComLinkHelper CLHelper
		{
			get {
				return _comLinkHelper;
			}
		}
		
		static ComLinkHelper()
		{
			_comLinkHelper = new ComLinkHelper();
		}
		
		// For linking to the type information for this object
		public String GetLinkName(Object linkModifier)
		{
			if (linkModifier is TypeLibrary)
				return ((TypeLibrary)linkModifier).GetName();
			if (linkModifier is Type) {
				Type type = (Type)linkModifier;
				TypeLibrary typeLib = TypeLibrary.GetTypeLib(type);
				return typeLib.GetMemberName(type);
			}
			return linkModifier.ToString();
		}
		
		// This could either be a MemberInfo, Type or a TypeLibrary, 
		// and we point
		// to the correct node
		public void ShowTarget(Object linkModifier)
		{
			BrowserTreeNode resultNode = null;
			// A type library
			if (linkModifier is TypeLibrary) {
				resultNode = ComSupport.FindTypeLib(((TypeLibrary)linkModifier).Key);
			} else {
				Type type;
				if (linkModifier is Type)
					type = (Type)linkModifier;
				else
					type = ((MemberInfo)linkModifier).DeclaringType;
				// Get the typelib node
				TypeLibrary typeLib = TypeLibrary.GetTypeLib(type);
				if (typeLib == null)
					return;
				// Find the type, this could be a class or an interface
				ComTypeLibTreeNode typeLibNode = (ComTypeLibTreeNode)ComSupport.FindTypeLib(typeLib.Key);
				String typeName = typeLib.GetMemberName(type);
				typeLibNode.ExpandNode();
				resultNode = SearchNode(typeLibNode, typeName);
				// Find the member (Type is also a MemberInfo)
				if (!(linkModifier is Type)) {
					MemberInfo mi = (MemberInfo)linkModifier;
					ComTypeTreeNode typeTreeNode = (ComTypeTreeNode)resultNode;
					resultNode = FindMember(typeLibNode, typeTreeNode, mi);
				}
			}
			// Point to the result
			if (resultNode != null) {
				ObjectBrowser.TabControl.SelectedTab = ComSupport.ComTabPage;
				resultNode.PointToNode();
			}
		}
		protected BrowserTreeNode FindMember(ComTypeLibTreeNode typeLibNode, ComTypeTreeNode typeTreeNode, MemberInfo mi)
		{
			// If we have a class, we need to look at all of
			// its implemented interfaces
			if (typeTreeNode.MemberInfo is ComClassInfo) {
				BrowserTreeNode resultNode = null;
			
				ComClassInfo classInfo = (ComClassInfo)typeTreeNode.MemberInfo;
				foreach (BasicInfo iface in classInfo.Interfaces) {
					BrowserTreeNode ifaceNode = 
						SearchNode(typeLibNode, iface.Name);
					String searchName = mi.Name;
					// See if its a member name qualified by this
					// interface name
					if (mi.Name.StartsWith(iface.Name)) {
						searchName = mi.Name.Substring(iface.Name.Length + 1);
						if (mi is EventInfo && 
							searchName.StartsWith("Event_"))
							searchName = searchName.Substring(6);
					}
					resultNode = SearchNode(ifaceNode, searchName);
					if (resultNode != null)
						return resultNode;
				}
				throw new Exception("(bug) - cant find member " + mi);
			} else {
				return SearchNode(typeTreeNode, mi.Name);
			}
		}
		
		protected BrowserTreeNode SearchNode(BrowserTreeNode startNode, String searchName)
		{
			startNode.ExpandNode();
			foreach (ComMemberTreeNode node in startNode.LogicalNodes) {
				if (node.MemberInfo.Name.Equals(searchName))
					return node;
			}                        
			return null;
		}
	}
}
