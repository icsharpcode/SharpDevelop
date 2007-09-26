// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.SharpDevelop.BuildWorker
{
	/// <summary>
	/// The settings used to start a build.
	/// </summary>
	[Serializable]
	public class BuildJob
	{
		public string ProjectFileName { get; set; }
		public string Target { get; set; }
		
		Dictionary<string, string> properties = new Dictionary<string, string>();
		
		public Dictionary<string, string> Properties {
			get { return properties; }
		}
		
		List<string> additionalImports = new List<string>();
		
		public List<string> AdditionalImports {
			get { return additionalImports; }
		}
		
		public override string ToString()
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine("BuildJob:");
			b.AppendLine("  ProjectFileName = " +  ProjectFileName);
			b.AppendLine("  Target = " + Target);
			b.AppendLine("  " + Properties.Count + " Properties:");
			foreach (KeyValuePair<string, string> pair in Properties) {
				b.AppendLine("    " + pair.Key + " = " + pair.Value);
			}
			b.AppendLine("  " + AdditionalImports.Count + " Additional Imports:");
			foreach (string import in AdditionalImports) {
				b.AppendLine("    " + import);
			}
			return b.ToString();
		}
	}
}
