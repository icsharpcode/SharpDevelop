// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

using NoGoop.Obj;

namespace NoGoop.ObjBrowser.TreeNodes
{

	internal class ComMemberTreeNode : BrowserTreeNode
	{

        protected BasicInfo             _memberInfo;


        public int HelpContext
        {
            get
                {
                    return _memberInfo._helpContext;
                }
        }


        public String HelpFile
        {
            get
                {
                    return _memberInfo._helpFile;
                }
        }


        public HelpNavigator HelpNavigator
        {
            get
                {
                    return HelpNavigator.Topic;
                }
        }


        internal BasicInfo MemberInfo
        {
            get
            {
                return _memberInfo;
            }
        }


        internal ComMemberTreeNode(BasicInfo memberInfo) : base()
		{
            _memberInfo = memberInfo;
            _memberInfo.TreeNode = this;

            SetPresInfo(_memberInfo.PresInfo);

            PostConstructor();
		}

        public override ISearchMaterializer GetSearchMaterializer
            (ISearcher searcher)
        {
            return _memberInfo.GetSearchMaterializer(searcher);
        }

        public override String GetSearchNameString()
        {
            return _memberInfo.GetSearchNameString();
        }

        public override int GetImageIndex()
        {
            return _memberInfo.GetImageIndex();
        }

        public override ICollection GetSearchChildren()
        {
            return _memberInfo.GetSearchChildren();
        }
        
        public override bool HasSearchChildren(ISearcher searcher)
        {
            return _memberInfo.HasSearchChildren(searcher);
        }
        
        // Determines is this node has children
        protected override bool HasChildren()
        {
            return false;
        }

        public override String GetName()
		{
            return _memberInfo.GetName();
		}

        public override void GetDetailText()
		{
            base.GetDetailText();

            if (_memberInfo != null)
                _memberInfo.GetDetailText();

		}

	}

}
