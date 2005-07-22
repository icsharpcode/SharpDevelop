// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionItem
	{
		string    name;
		string    location;
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public string Location {
			get {
				return location;
			}
			set {
				location = value;
			}
		}
		
		public SolutionItem(string name, string location)
		{
			this.name = name;
			this.location = location;
		}
		
		public void AppendItem(StringBuilder sb, string indentString)
		{
			sb.Append(indentString);
			sb.Append(Name);
			sb.Append(" = ");
			sb.Append(Location);
			sb.Append(Environment.NewLine);
		}
		
		
		public override string ToString() 
		{
			return String.Format("[SolutionItem: location = {0}, name = {1}]",
			                     location,
			                     name);
		}
		
	}
}
