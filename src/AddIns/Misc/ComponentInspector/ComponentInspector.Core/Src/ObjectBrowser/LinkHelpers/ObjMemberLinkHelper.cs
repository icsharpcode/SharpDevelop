// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

using NoGoop.Controls;
using NoGoop.Obj;
using NoGoop.ObjBrowser;
using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.Util;
using NoGoop.Win32;

namespace NoGoop.ObjBrowser.LinkHelpers
{

	// Used to find the specified member of the specified object
	// in the object tree.

	// We expect the link modifier to implment IObjectMember
	internal class ObjMemberLinkHelper : ILinkTarget
	{

		protected static ObjMemberLinkHelper         _objMemberLinkHelper;


		public static ObjMemberLinkHelper OMLHelper
		{
			get
				{
					return _objMemberLinkHelper;
				}
		}


		static ObjMemberLinkHelper()
		{
			_objMemberLinkHelper = new ObjMemberLinkHelper();
		}


		// For linking to the type information for this object
		public String GetLinkName(Object linkModifier)
		{
			return ((IObjectMember)linkModifier).Member.ToString();
		}


		public void ShowTarget(Object linkModifier)
		{
			IObjectMember om = (IObjectMember)linkModifier;
			ObjectTreeNode.
				SelectObjectMember(om, ObjectTreeNode.CREATE_OBJ);
		}

	}
}
