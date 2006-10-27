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

using NoGoop.ObjBrowser.TreeNodes;

namespace NoGoop.ObjBrowser
{

    // Handles the tree of Controls which is kepted in sync with 
    // the assembly tree
	internal class ControlTree : BrowserTree
	{
        protected static ControlTree _controlTree;
        protected static Hashtable _assemblies = new Hashtable();
        protected static bool _uptodate = true;

        internal ControlTree()
        {
            _controlTree = this;
        }

        // Get the control tab up to date
        internal static void SetupControlTree()
        {
            lock (typeof(AssemblySupport)) {
                if (!_uptodate) {
                    foreach (TreeNode n in AssemblySupport.AssyRootNode.Nodes) {
						if (n is AssemblyTreeNode) {
							AssemblyTreeNode node = (AssemblyTreeNode)n;
							AddAssy(node.Assembly, null);
						}
					}
                    _uptodate = true;
                }
            }
        }

        internal delegate void AddAssyInvoker(Assembly assy, ICollection types);

        internal static void AddAssy(Assembly assy, ICollection types)
        {
            lock (_assemblies) {
                // Don't add unless the tab is visible, because
                // the GetTypes() can take a long time (up to 10
                // seconds in some cases)
                bool controlTreeShowing = ComponentInspectorProperties.ShowControlPanel;
                if (!controlTreeShowing) {
                    _uptodate = false;
                    return;
                }

                // Keep track of the assemblies we know about, don't
                // add the same one twice
                if (_assemblies[assy] == null)
                    _assemblies.Add(assy, assy);
                else
                    return;
            }

            if (types == null)
                types = AssemblySupport.GetAssyTypes(assy);

            foreach (Type t in types) {
                if (!(typeof(Control).IsAssignableFrom(t)))
                    continue;
                
                String ns = t.Namespace;
                if (ns == null)
                    ns = "";
                
                bool found = false;
                NamespaceTreeNode n = null;
                for (int i = 0; i < _controlTree.Nodes.Count; i++) {
                    n = (NamespaceTreeNode)_controlTree.Nodes[i];
                    if (n.Namespace.Equals(ns)) {
                        found = true;
                        n.InvalidateNode();
                        break;
                    }
                }

                if (!found) {
                    n = new NamespaceTreeNode(ns);
                    _controlTree.Nodes.Add(n);
                }

                n.AddAssy(assy);
            }
        }

        internal static void RemoveAssy(Assembly assy)
        {
            // Remove any empty namespaces
            for (int i = 0; i < _controlTree.Nodes.Count; ) {
                NamespaceTreeNode n = (NamespaceTreeNode)_controlTree.Nodes[i];
                if (!n.RemoveAssy(assy))
                    _controlTree.Nodes.Remove(n);
                else
                    i++;
            }

            lock (_assemblies) {
                if (_assemblies[assy] != null)
                    _assemblies.Remove(assy);
            }
        }
	}
}
