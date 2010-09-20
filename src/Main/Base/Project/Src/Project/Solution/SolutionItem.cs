// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;

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
