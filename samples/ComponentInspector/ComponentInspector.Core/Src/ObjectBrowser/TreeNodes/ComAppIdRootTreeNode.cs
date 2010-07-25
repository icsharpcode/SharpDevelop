// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using NoGoop.Obj;
using NoGoop.Win32;

namespace NoGoop.ObjBrowser.TreeNodes
{

	internal class ComAppIdRootTreeNode : ComRootTreeNode
	{


        internal ComAppIdRootTreeNode() : base()
		{
            SetPresInfo(PresentationMap.FOLDER_CLOSED);

            _baseKey = Windows.KeyAppId;
            _progressName = "AppId";
		}

        // Used to get the basic info that is used by this type
        // of node
        protected override Object ProcessChild(RegistryKey key,
                                                  String subKeyName)
        {
            return new ComAppInfo(key, subKeyName);
        }

        // Allocates the correct type of node
        protected override BrowserTreeNode AllocateChildNode(Object obj)
        {
            ComTypeTreeNode node = new ComTypeTreeNode((BasicInfo)obj);
            node.IntermediateNodeTypes = null;
            node.SetPresInfo(TYPEKIND.TKIND_COCLASS);
            if (_progress != null)
                _progress.UpdateProgress(1);
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
            return "AppIds";
		}

        public override void GetDetailText()
		{
            base.GetDetailText();
		}


	}

}
