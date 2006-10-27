// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;

namespace NoGoop.ObjBrowser.TreeNodes
{

	internal class ModuleTreeNode : BrowserTreeNode
	{

        protected Module              _module;


        internal ModuleTreeNode(Module module) : base()
		{
            _module = module;
            PostConstructor();
		}


        // Gets the objects to iterate over to make the child nodes
        protected override ICollection GetChildren()
        {
            Array types = _module.GetTypes();
            Hashtable hash = new Hashtable();
            foreach (Type t in types)
            {
                String ns = t.Namespace;
                if (ns == null)
                    ns = "";
                if (hash.ContainsKey(ns))
                    continue;
                hash.Add(ns, null);
            }

            ArrayList outNames = new ArrayList();
            outNames.AddRange(hash.Keys);
            outNames.Sort();
            return outNames;
        }

        // Allocates the correct type of node
        protected override BrowserTreeNode AllocateChildNode(Object obj)
        {
            return new NamespaceTreeNode((String)obj, _module);
        }


        // Determines is this node has children
        protected override bool HasChildren()
        {
            return _module.GetTypes().Length > 0;
        }


        public override String GetName()
		{
            return _module.Name;
		}

        public override void GetDetailText()
		{
            base.GetDetailText();
		}

	}

}
