// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using Microsoft.Win32;
using NoGoop.Obj;
using NoGoop.Win32;

namespace NoGoop.ObjBrowser.TreeNodes
{

	internal class ComCatRootTreeNode : ComRootTreeNode
	{
        internal ComCatRootTreeNode() : base()
		{
            SetPresInfo(PresentationMap.COM_FOLDER_CLASS);

            _baseKey = Windows.KeyComponentCategories;
            _progressName = "Component Category";
		}

        // Used to get the basic info that is used by this type
        // of node
        protected override Object ProcessChild(RegistryKey key,
                                                  String subKeyName)
        {
            BasicInfo info = new BasicInfo(key, subKeyName);
            foreach (String valueName in key.GetValueNames())
                {
                    if (valueName != null && 
                        !valueName.Equals("") && 
                        info.Name == null)
                        info.Name = (String)key.GetValue(valueName);
                }
            if (info.Name == null)
            {
                info.Name = subKeyName;
                info.PrintName = subKeyName + " (guid)";
            }
            info._infoType = "Category";
            return info;
        }

        // Allocates the correct type of node
        protected override BrowserTreeNode AllocateChildNode(Object obj)
        {
            ComCatTreeNode node = new ComCatTreeNode((BasicInfo)obj);
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
            return StringParser.Parse("${res:ComponentInspector.ComCategoryRootTreeNode.Text}");
		}

        public override void GetDetailText()
		{
            base.GetDetailText();
		}


	}

}
