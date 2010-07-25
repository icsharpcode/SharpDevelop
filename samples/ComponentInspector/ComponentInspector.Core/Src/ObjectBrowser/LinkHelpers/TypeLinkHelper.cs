// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using NoGoop.ObjBrowser.TreeNodes;

namespace NoGoop.ObjBrowser.LinkHelpers
{

	// A single instance of this exists whose purpose is to
	// resolve links to types or assemblies in the assembly/type tree.
	// The linkModifier is the MemberInfo, type or assembly to be
	// resolved.
	internal class TypeLinkHelper : ILinkTarget
	{

		protected static TypeLinkHelper         _typeLinkHelper;


		public static TypeLinkHelper TLHelper
		{
			get
				{
					return _typeLinkHelper;
				}
		}


		static TypeLinkHelper()
		{
			_typeLinkHelper = new TypeLinkHelper();
		}


		// For linking to the type information for this object
		public String GetLinkName(Object linkModifier)
		{
			if (linkModifier == null)
				return null;
			if (linkModifier is Assembly)
				return ((Assembly)linkModifier).FullName;
			return linkModifier.ToString();
		}

		// When a link is clicked to this, the linkModifier is the
		// type or the assembly
		public void ShowTarget(Object linkModifier)
		{
			if (linkModifier == null)
				return;

			Assembly assy;
			Type type = null;
			MemberInfo memberInfo = null;
			if (linkModifier is Assembly)
			{
				assy = (Assembly)linkModifier;
			}
			else if (linkModifier is Type)
			{
				type = (Type)linkModifier;
				assy = type.Assembly;
			}
			else // MemberInfo
			{
				memberInfo = (MemberInfo)linkModifier;
				type = memberInfo.DeclaringType;
				assy = type.Assembly;
			}

			AssemblySupport.SelectAssyTab();

			AssemblyTreeNode node = 
				AssemblySupport.FindAssemblyTreeNode(assy);
			
			if (node == null)
				throw new Exception("Bug: assembly not found for type: " + type);

			if (type != null)
			{
				TypeTreeNode typeNode = node.GetTypeNode(type);
				if (typeNode == null)
				{
					throw new Exception("Bug: typeNode not found for type: " 
										+ type);
				}

				if (memberInfo != null)
				{
					MemberTreeNode memberNode = 
						typeNode.FindMemberNode(memberInfo, 
												!TypeTreeNode.FIND_NESTED);
					if (memberNode != null)
					{
						memberNode.PointToNode();
					}
					else
					{
						throw new Exception("Bug: member not found for type: " 
											+ type + " mem: " + memberInfo);
					}
						
				}
				else
				{
					typeNode.PointToNode();
				}
			}
			else
			{
				node.PointToNode();
			}
		}

	}
}
