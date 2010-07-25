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

	internal class NamespaceTreeNode : BrowserTreeNode
	{

        protected String            _namespace;
        protected Module            _module;

        // This contains only controls.  This means that the mode
        // can represent multiple assemblies
        protected bool              _controlsOnly;

        protected ArrayList         _assemblies;


        internal NamespaceTreeNode(String n,
                                   Module m) : base()
		{
            _namespace = n;
            _module = m;
            _controlsOnly = false;
            PostConstructor();
		}


        internal NamespaceTreeNode(String n) : base()
		{
            _namespace = n;
            _assemblies = new ArrayList();
            _controlsOnly = true;
            PostConstructor();
		}


        internal String Namespace
        {
            get
            {
                return _namespace;
            }
        }

        // Gets the objects to iterate over to make the child nodes
        protected override ICollection GetChildren()
        {
            ArrayList types = new ArrayList();

            // Controls are at the scope of the assembly
            if (_controlsOnly)
            {
                foreach (Assembly assy in _assemblies)
                    types.AddRange(assy.GetTypes());
            }
            else
                types.AddRange(_module.GetTypes());

            ArrayList outTypes = new ArrayList();
            foreach (Type t in types)
            {
                // Nested types show up under their parent type
                if (t.IsNestedAssembly ||
                    t.IsNestedFamANDAssem ||
                    t.IsNestedFamily ||
                    t.IsNestedFamORAssem ||
                    t.IsNestedPrivate ||
                    t.IsNestedPublic)
                    continue;

                if (_controlsOnly &&
                    !(typeof(Control).IsAssignableFrom(t)))
                    continue;

                String ns = t.Namespace;
                if (ns == null)
                    ns = "";
                if (ns.Equals(_namespace))
                    outTypes.Add(t);
            }
            outTypes.Sort(new ObjectInfo.TypeCompare());
            return outTypes;
        }

        // Allocates the correct type of node
        protected override BrowserTreeNode AllocateChildNode(Object obj)
        {
            return new TypeTreeNode((Type)obj, 
                                    !TypeTreeNode.NESTEDTYPE);
        }


        // Determines is this node has children
        protected override bool HasChildren()
        {
            // FIXME
            return true;
        }

        internal void AddAssy(Assembly assy)
        {
            if (_assemblies.Contains(assy))
                return;
            _assemblies.Add(assy);
        }

        // Returns true if there are still children
        internal bool RemoveAssy(Assembly assy)
        {
            for (int i = 0; i < LogicalNodes.Count; )
            {
                if (LogicalNodes[i] is TypeTreeNode)
                {
                    TypeTreeNode typeNode = (TypeTreeNode)LogicalNodes[i];
                    if (typeNode.Type.Assembly == assy)
                        typeNode.RemoveLogicalNode();
                    else
                        i++;
                }
                else
                    i++;
            }


            _assemblies.Remove(assy);
            if (_assemblies.Count == 0)
                return false;
            return true;
        }


        internal TypeTreeNode FindType(Type type)
        {
            ExpandNode();
            foreach (TypeTreeNode typeNode in LogicalNodes)
            {
                if (typeNode.Type.Equals(type))
                    return typeNode;
            }
            return null;
        }


        public override String GetName()
		{
            return _namespace;
		}

        public override void GetDetailText()
		{
            base.GetDetailText();
		}

	}

}
