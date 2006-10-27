// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using NoGoop.Obj;
using NoGoop.ObjBrowser.LinkHelpers;
using NoGoop.ObjBrowser.Panels;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.TreeNodes
{

	internal class TypeTreeNode : MemberTreeNode, 
        IMenuTreeNode, IDesignSurfaceNode
	{

        protected Type              _type;

        // Will this type appear on the design surface
        protected bool              _onDesignSurface;

        // Points to one of the strings below
        protected String            _typeKindName;


        public override Type Type
        {
            get
                {
                    return _type;
                }
        }

        public override bool IsMember
        {
            get
                {
                    return false;
                }
        }


        public override bool OnDesignSurface
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


        internal const bool NESTEDTYPE = true;

        internal TypeTreeNode(Type type, 
                              bool nestedType)
                : base(type)
		{

            _typeKindName = ObjectInfo.GetTypeKind(type);
            if (!nestedType)
            {
                SetPresInfo(_typeKindName);

                int inType = 0;

                if (type.IsEnum)
                    inType = IntermediateNodeType.INTNODE_ENUM;
                else if (type.IsClass)
                    inType = IntermediateNodeType.INTNODE_CLASS;
                else if (type.IsInterface)
                    inType = IntermediateNodeType.INTNODE_INTERFACE;
                else if (type.IsValueType)
                    inType = IntermediateNodeType.INTNODE_STRUCT;
                else if (type.IsPrimitive)
                    inType = IntermediateNodeType.INTNODE_CLASS;
                else if (type.Equals(typeof(Enum)))
                    inType = IntermediateNodeType.INTNODE_CLASS;
            
                IntermediateNodeTypes = new ArrayList();
                IntermediateNodeTypes.Add
                    (IntermediateNodeType.
                     GetNodeType(inType));
            }

            _type = type;

            // We allow classes to be dragged
            _isDragSource = true;

            _onDesignSurface = true;

            PostConstructor();
		}

        // Gets the objects to iterate over to make the child nodes
        protected override ICollection GetChildren()
        {
            Array members = _type.GetMembers
                (ReflectionHelper.ALL_STATIC_BINDINGS);
            Array.Sort(members, new ObjectInfo.MemberCompare());
            return members;
        }

        // Allocates the correct type of node
        protected override BrowserTreeNode AllocateChildNode(Object obj)
        {
            if (obj is Type)
                return new TypeTreeNode((Type)obj, NESTEDTYPE);
            else
                return new MemberTreeNode((MemberInfo)obj);
        }


        internal const bool FIND_NESTED = true;

        internal MemberTreeNode FindMemberNode(MemberInfo member,
                                               bool findNested)
        {
            ExpandNode();
            foreach (MemberTreeNode memberNode in LogicalNodes)
            {
                if (ReflectionHelper.IsMemberEqual(memberNode.Member, 
                                                   member))
                    return memberNode;
            }

            if (findNested)
            {
                foreach (MemberTreeNode memberNode in LogicalNodes)
                {
                    // Only types can be nested
                    if (memberNode is TypeTreeNode)
                    {
                        MemberTreeNode foundNode =
                            ((TypeTreeNode)memberNode).
                            FindMemberNode(member, findNested);
                        if (foundNode != null)
                            return foundNode;
                    }
                }
            }
            return null;
        }

        // Determines is this node has children
        protected override bool HasChildren()
        {
            return _type.GetMembers
                (ReflectionHelper.ALL_STATIC_BINDINGS).Length > 0;
        }


        public override bool HasDesignSurface()
        {
            if (typeof(Control).IsAssignableFrom(Type))
                return true;
            return false;
        }


        public override void DesignSurfaceClick(object sender, EventArgs e)
        {
            _onDesignSurface = !_onDesignSurface;
            ((BrowserTree)TreeView).TreeMenuHelper.
                DesignSurfaceClick(_onDesignSurface);
        }


        public override void SetupParamPanel()
        {
            ObjectBrowser.ParamPanel.Setup(ActionMenuHelper.CalcInvokeActionName(this, false),
                 new EventHandler(((BrowserTree)TreeView).TreeNodePopupCreateObj),
                 this,
                 null);
        }

        public override String GetName()
		{
            return _type.Name;
		}

        public override void GetDetailText()
		{
            base.GetDetailText();

            ObjectInfo.GetDesc(_type, _typeKindName);

            String baseIfType;
            if (_type.IsInterface)
                baseIfType = "Inherited Interface";
            else
                baseIfType = "Implemented Interface";

            foreach (Type intf in _type.GetInterfaces())
            {
                DetailPanel.AddLink(baseIfType,
                                    !ObjectBrowser.INTERNAL,
                                    60,
                                    TypeLinkHelper.TLHelper,
                                    intf);
            }

            if (_type.BaseType != null)
            {
                DetailPanel.AddLink("Base Type",
                                    !ObjectBrowser.INTERNAL,
                                    51,
                                    TypeLinkHelper.TLHelper,
                                    _type.BaseType);
            }

            ComObjectInfo.GetDetailAxType(_type, null, null, 0);
		}

	}

}
