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

namespace NoGoop.ObjBrowser.TreeNodes
{

	internal class MemberTreeNode : BrowserTreeNode, 
        IInvokableTreeNode, IDesignSurfaceNode, IMenuTreeNode, ITargetType
	{

        protected MemberInfo              _member;


        public virtual bool OnDesignSurface
        {
            get
                {
                    return ((IDesignSurfaceNode)_logicalParent).
                        OnDesignSurface;
                }
            set
                {
                    ((IDesignSurfaceNode)_logicalParent).
                        OnDesignSurface = value;
                }
        }


        public virtual Type Type
        {
            get
                {
                    return _member.DeclaringType;
                }
        }

        public virtual bool IsMember
        {
            get
                {
                    return true;
                }
        }


        internal MemberTreeNode(MemberInfo member) : base()
		{
            _member = member;
            if (_member is ConstructorInfo)
                _isDragSource = true;

            SetPresInfo(member.MemberType);

            // Let the TypeTreeNode call the post constructor when it is done
            if (!(this is TypeTreeNode))
                PostConstructor();
		}


        // Used only for a constuctor to create a new instance of the object
        public void Invoke(bool setMember, 
                           bool autoInvoke,
                           bool ignoreException)
		{

		}

        public bool IsAutoInvoked(bool ignoreException) 
        {
            return false;
        }


        internal MemberInfo Member
        {
            get
                {
                    return _member;
                }
        }


        // Finds the node associated with this member
        internal static MemberTreeNode FindMember(MemberInfo member)
        {
            Type type = member.DeclaringType;
            Assembly assy = type.Assembly;

            AssemblyTreeNode assyNode = 
                AssemblySupport.FindAssemblyNode(assy);
            if (assyNode == null)
                return null;

            NamespaceTreeNode nameNode = 
                assyNode.FindNamespace(type.Namespace);
            if (nameNode == null)
                return null;

            TypeTreeNode typeNode =
                nameNode.FindType(type);
            if (typeNode == null)
                return null;

            return typeNode.FindMemberNode(member, !TypeTreeNode.FIND_NESTED);
        }


        // Gets the objects to iterate over to make the child nodes
        protected override ICollection GetChildren()
        {
            return new ArrayList();
        }

        // Allocates the correct type of node
        protected override BrowserTreeNode AllocateChildNode(Object obj)
        {
            return null;
        }


        // Determines is this node has children
        protected override bool HasChildren()
        {
            return false;
        }


        public override bool HasCreateObj()
        {
            if (_isDragSource)
                return true;
            return false;
        }


        public override bool HasDesignSurface()
        {
            if (_member is ConstructorInfo)
            {
                Type type = ((TypeTreeNode)_logicalParent).Type;
                if (typeof(Control).IsAssignableFrom(type))
                    return true;
            }
            return false;
        }


        public virtual void DesignSurfaceClick(object sender, EventArgs e)
        {
            OnDesignSurface = !OnDesignSurface;
            ((BrowserTree)TreeView).TreeMenuHelper.
                DesignSurfaceClick(OnDesignSurface);
        }

        public override void SetupParamPanel()
        {
            if (_member is ConstructorInfo)
            {
                ObjectBrowser.ParamPanel.Setup
                    (ActionMenuHelper.CalcInvokeActionName(this, false),
                     new EventHandler
                         (((BrowserTree)TreeView).TreeNodePopupCreateObj),
                     this,
                     ((ConstructorInfo)_member).GetParameters());
                return;
            }
            base.SetupParamPanel();
        }

        public override String GetName()
		{
            return _member.Name;
		}

        public override void GetDetailText()
		{
            ObjectInfo.MemberDetailText(_member);

            base.GetDetailText();

		}

	}

}
