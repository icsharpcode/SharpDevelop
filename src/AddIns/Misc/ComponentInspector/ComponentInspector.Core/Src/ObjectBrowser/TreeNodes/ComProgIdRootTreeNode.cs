// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Microsoft.Win32;
using NoGoop.Obj;
using NoGoop.Win32;

namespace NoGoop.ObjBrowser.TreeNodes
{

	internal class ComProgIdRootTreeNode : ComRootTreeNode
	{

        protected class ProgIdNode
        {
            internal BasicInfo          _info;
            internal String             _progId;
        }


        // Used to hold the progId between ProcessChild() and SortKey()
        protected String                _sortKey;

        internal ComProgIdRootTreeNode() : base()
		{
            SetPresInfo(PresentationMap.COM_FOLDER_APPID);

            _baseKey = Windows.KeyClassRoot;
            _progressName = "ProgId";
		}

        // Used to get the basic info that is used by this type
        // of node
        protected override Object ProcessChild(RegistryKey key,
                                                  String subKeyName)
        {

            // Eliminate the known keys and the file extensions
            if (subKeyName.StartsWith(".") ||
                subKeyName.Equals("CLSID") ||
                subKeyName.Equals("CID") ||
                subKeyName.Equals("AppId") ||
                subKeyName.Equals("Interface") ||
                subKeyName.Equals("TypeLib"))
                return null;

            ComClassInfo classInfo = null;
            _sortKey = subKeyName;

            ProgIdNode node = new ProgIdNode();
            node._progId = _sortKey;
            
            // See if there is a CLSID for this program id
            RegistryKey clsIdKey = 
                key.OpenSubKey("CLSID");
            if (clsIdKey != null)
            {
                String clsId = (String)clsIdKey.GetValue(null);
                if (clsId != null)
                {
                    RegistryKey classKey = 
                        Windows.KeyCLSID.OpenSubKey(clsId);
                                
                    if (classKey != null)
                    {
                        classInfo = 
                            ComClassInfo.GetClassInfo(classKey, clsId);
                        node._info = classInfo;
                        classInfo.AddProgId(subKeyName);
                    }
                }
            }

            // Don't bother with it unless it refers to a class
            if (classInfo == null)
                return null;
            return node;
        }

        protected override Object GetSortKey(Object info)
        {
            return _sortKey;
        }

        // Allocates the correct type of node
        protected override BrowserTreeNode AllocateChildNode(Object obj)
        {
            if (_progress != null)
                _progress.UpdateProgress(1);

            ProgIdNode progIdNode = (ProgIdNode)obj;
            ComTypeTreeNode node = new ComTypeTreeNode(progIdNode._info, 
                                                       progIdNode._progId);
            node.IntermediateNodeTypes = null;
            return node;
        }


        // Determines is this node has children
        protected override bool HasChildren()
        {
            // Assume there are classes on the system
            return true;
        }



        public override String GetName()
		{
            return "ProgIds";
		}

        public override void GetDetailText()
		{
            base.GetDetailText();
		}


	}

}
