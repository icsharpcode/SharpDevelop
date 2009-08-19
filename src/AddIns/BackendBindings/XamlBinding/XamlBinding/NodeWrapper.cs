// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.XamlBinding
{
	public class NodeWrapper {
		public string ElementName { get; set; }
		public string Name { get; set; }
		
		public int StartOffset { get; set; }
		public int EndOffset { get; set; }
		
		public IList<NodeWrapper> Children { get; set; }
	}
}
