// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public class LookupInfo {
		public LookupInfo() { }
		
		public Dictionary<string, string> XmlnsDefinitions { get; set; }
		public List<string> IgnoredXmlns { get; set; }
		public QualifiedNameWithLocation Active { get; set; }
		public Stack<QualifiedNameWithLocation> Ancestors { get; set; }
		public QualifiedNameWithLocation Parent { get; set; }
		public int ActiveElementStartIndex { get; set; }
		public bool IsRoot { get; set; }
	}
}
