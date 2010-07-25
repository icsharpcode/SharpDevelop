// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using NoGoop.Obj;
using NoGoop.ObjBrowser.TreeNodes;

namespace NoGoop.ObjBrowser.LinkHelpers
{

    // A single instance of this is used to resolve object
    // links to the object tree.  If the object is already present
    // in the object tree, it is found, if not a new top-level
    // object is created for the object
	internal class ObjLinkHelper : ILinkTarget
	{

        protected static ObjLinkHelper         _objLinkHelper;


        public static ObjLinkHelper OLHelper
        {
            get
                {
                    return _objLinkHelper;
                }
        }


        static ObjLinkHelper()
        {
            _objLinkHelper = new ObjLinkHelper();
        }


        // For linking to the type information for this object
        public String GetLinkName(Object linkModifier)
        {
            ObjectInfo objInfo = ObjectInfoFactory.GetObjectInfo(true,
                                                                 linkModifier);
            String retString = linkModifier.ToString();

            // Don't return a boring type
            if (retString.Equals(Win32.ActiveX.COM_ROOT_TYPE_NAME))
                return objInfo.ObjType.ToString();
            return retString;
        }

        public void ShowTarget(Object linkModifier)
        {
            ObjectTreeNode.SelectObject
                (linkModifier, ObjectTreeNode.CREATE_OBJ);
        }

    }
}
