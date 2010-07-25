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

	internal class AssemblyTreeNode : BrowserTreeNode, 
        IMenuTreeNode
	{

        protected TypeLibrary           _typeLib;
        protected Assembly              _assembly;
        protected bool                  _noClose;


        internal Assembly Assembly
        {
            get
            {
                return _assembly;
            }
        }

        internal TypeLibrary TypeLib
        {
            get
            {
                return _typeLib;
            }
            set
            {
                _typeLib = value;
            }
        }

        internal bool NoClose
        {
            get
            {
                return _noClose;
            }
            set
            {
                _noClose = value;
            }
        }


        internal AssemblyTreeNode(Assembly assembly,
                                  TypeLibrary typeLib) : base()
		{
            _assembly = assembly;
            _typeLib = typeLib;

            MethodInfo entryPoint = null;
            try
            {
                entryPoint = _assembly.EntryPoint;
            }
            catch
            {
                // Sadly, this can throw for some reason
            }

            if (entryPoint != null)
                _isDragSource = true;
            PostConstructor();
		}


        // Gets the objects to iterate over to make the child nodes
        protected override ICollection GetChildren()
        {
            return _assembly.GetModules();
        }

        // Allocates the correct type of node
        protected override BrowserTreeNode AllocateChildNode(Object obj)
        {
            return new ModuleTreeNode((Module)obj);
        }


        // Determines is this node has children
        protected override bool HasChildren()
        {
            return _assembly.GetModules().Length > 0;
        }


        internal NamespaceTreeNode FindNamespace(String ns)
        {
            ExpandNode();
            foreach (ModuleTreeNode modNode in LogicalNodes)
            {
                modNode.ExpandNode();
                foreach (NamespaceTreeNode nameNode in modNode.LogicalNodes)
                {
                    if (nameNode.Namespace.Equals(ns))
                    {
                        return nameNode;
                    }
                    
                }
            }
            return null;
        }

        internal TypeTreeNode GetTypeNode(Type t)
        {
            String ns = t.Namespace;
            bool isNestedType = (t.MemberType == MemberTypes.NestedType);

            ExpandNode();
            foreach (ModuleTreeNode modNode in LogicalNodes)
            {
                modNode.ExpandNode();
                foreach (NamespaceTreeNode nameNode in modNode.LogicalNodes)
                {
                    if (nameNode.Namespace.Equals(ns) || 
                        (ns == null && nameNode.Namespace.Equals("")) )
                    {
                        nameNode.ExpandNode();
                        foreach (TypeTreeNode typeNode in 
                                 nameNode.LogicalNodes)
                        {
                            if (typeNode.Type.Equals(t))
                            {
                                return typeNode;
                            } 
                            else if (isNestedType)
                            {
                                TypeTreeNode nestedNode =
                                    (TypeTreeNode)typeNode.FindMemberNode
                                    (t, TypeTreeNode.FIND_NESTED);
                                if (nestedNode != null)
                                    return nestedNode;
                            }
                        }
                        break;
                    }
                }
            }
            return null;
        }

        public override bool HasCreateObj()
        {
            if (_assembly.EntryPoint != null)
                return true;
            return false;
        }


        public override bool HasClose()
        {
            if (_noClose)
                return false;
            return true;
        }

        public override void SetupParamPanel()
        {
            if (_assembly.EntryPoint != null)
            {
                ObjectBrowser.ParamPanel.Setup
                    (ActionMenuHelper.CalcInvokeActionName(this, false),
                     new EventHandler
                         (((BrowserTree)TreeView).TreeNodePopupCreateObj),
                     this,
                     _assembly.EntryPoint.GetParameters());
                return;
            }
            base.SetupParamPanel();
        }

        public override void RemoveLogicalNode()
        {
            AssemblySupport.CloseAssembly(this);
            ControlTree.RemoveAssy(_assembly);
            base.RemoveLogicalNode();
        }



        public override String GetName()
		{
            return _assembly.GetName().Name;
		}

        public override void GetDetailText()
		{
            base.GetDetailText();

            if (_typeLib != null)
                _typeLib.GetDetailText(!TypeLibrary.SHOW_ASSY);

            AssemblySupport.GetDetailText(_assembly);
		}

	}

}
