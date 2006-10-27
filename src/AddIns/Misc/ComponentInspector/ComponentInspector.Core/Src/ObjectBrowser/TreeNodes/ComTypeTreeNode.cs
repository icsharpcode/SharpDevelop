// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using NoGoop.Obj;

namespace NoGoop.ObjBrowser.TreeNodes
{

	internal class ComTypeTreeNode : ComMemberTreeNode,
        IDesignSurfaceNode, IMenuTreeNode, ITargetType
	{

        // This will be shown if not null, this is used
        // for the progId portion of the tree
        protected String            _progId;


        // Will this type appear on the design surface
        protected bool              _onDesignSurface;

        public bool OnDesignSurface
        {
            get
                {
                    return _onDesignSurface;
                }
            set
                {
                    _onDesignSurface = value;
                }
        }


        public Type Type
        {
            get
                {
                    if (_memberInfo is ComClassInfo)
                        return ((ComClassInfo)_memberInfo).Type;
                    return null;
                }
        }

        public bool IsMember
        {
            get
                {
                    return false;
                }
        }

        internal ComTypeTreeNode(BasicInfo basicInfo) : 
                this(basicInfo, null)
		{
        }


        internal ComTypeTreeNode(BasicInfo basicInfo,
                                 String progId) : base(basicInfo)
		{
            _progId = progId;

            // We only allow classes to be dragged
            if (_memberInfo._typeKind == TYPEKIND.TKIND_COCLASS)
                _isDragSource = true;

            _onDesignSurface = true;

            if (_memberInfo.PresInfo != null)
            {
                _intermediateNodeTypes = new ArrayList();
                _intermediateNodeTypes.Add
                    (_memberInfo.PresInfo._intermediateNodeType);
            }

            PostConstructor();
		}

        public override String GetSearchNameString()
        {
            if (_progId != null)
                return _progId;
            return base.GetSearchNameString();
        }


        // Gets the objects to iterate over to make the child nodes
        protected override ICollection GetChildren()
        {
            return _memberInfo.Members;
        }

        // Allocates the correct type of node
        protected override BrowserTreeNode AllocateChildNode(Object obj)
        {
            // For inherited interfaces
            if (obj is ComInterfaceInfo)
            {
                BrowserTreeNode node = new ComTypeTreeNode((BasicInfo)obj);
                node.IntermediateNodeTypes = new ArrayList();
                node.IntermediateNodeTypes.Add
                    (IntermediateNodeType.
                     GetNodeType(IntermediateNodeType.INTNODE_COM_BASEINT));
                return node;
            }
            return new ComMemberTreeNode((BasicInfo)obj);
        }

        // Determines is this node has children
        protected override bool HasChildren()
        {
            return _memberInfo.Members.Count > 0;
        }

        public void DesignSurfaceClick(object sender, EventArgs e)
        {
            _onDesignSurface = !_onDesignSurface;
            ((BrowserTree)TreeView).TreeMenuHelper.
                DesignSurfaceClick(_onDesignSurface);
        }

        public override bool HasCreateObj()
        {
            if (_isDragSource)
                return true;
            return false;
        }

        public override bool HasDesignSurface()
        {
            Type type = null;
            try
            {
                // This can throw
                type = Type;
            }
            catch
            {
                return false;
            }

            if (typeof(Control).IsAssignableFrom(type))
                return true;
            return false;
        }


        public override String GetName()
		{
            if (_progId != null)
                return _progId;
            return base.GetName();
		}



	}

}
