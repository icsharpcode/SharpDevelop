// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class ResourceNode : AssemblyTreeNode
	{
		bool isTopLevel;
		
		public ResourceNode(string name, object attribute, bool isTopLevel) 
			: base(name, attribute, isTopLevel ? NodeType.Resource : NodeType.SingleResource)
		{
			this.isTopLevel = isTopLevel;
		}
		
		public override void Populate(ShowOptions Private, ShowOptions Internal)
		{
			if (name.ToLower().EndsWith(".resources")) {
				SA.SharpAssembly assembly = (SA.SharpAssembly)attribute;
				byte[] res = assembly.GetManifestResource(name);
				ResourceReader resreader = new ResourceReader(new MemoryStream(res));
				
				IDictionaryEnumerator en = resreader.GetEnumerator();
			      
			    ArrayList newNodes = new ArrayList();
			    
			    while (en.MoveNext()) {
			    	string nodename = (string)en.Key;
			    	if (en.Value != null) nodename += " : " + en.Value.GetType().Name;
			    	newNodes.Add(new ResourceNode(nodename, en.Value, false));
			    }
			    resreader.Close();
				
				newNodes.Sort(new TreeNodeComparer());
				foreach (ResourceNode resnode in newNodes) {
			    	Nodes.Add(resnode);
				}
			}
			populated = true;
		}
		
		
	}
}
