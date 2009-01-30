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
		/// <summary>
		/// The value of IntPtr.Size on the host. The build worker will report an error if its IntPtr.Size
		/// doesn't match. This is a safety feature to prevent compiling half of a solution using 32-bit MSBuild
		/// and the other half using 64-bit MSBuild.
		/// </summary>
		public int IntPtrSize { get; set; }
		
		
		public string ProjectFileName { get; set; }
		public string Target { get; set; }
		
		EventTypes eventMask = EventTypes.All;
		
		/// <summary>
		/// Gets/Sets the mask that controls which events are reported back to the host.
		/// </summary>
		public EventTypes EventMask {
			get { return eventMask; }
			set { eventMask = value; }
		}
		
		Dictionary<string, string> properties = new Dictionary<string, string>();
		
		public Dictionary<string, string> Properties {
			get { return properties; }
		}
		
		List<string> additionalImports = new List<string>();
		
		public IList<string> AdditionalImports {
			get { return additionalImports; }
		}
		
		HashSet<string> interestingTaskNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		
		public ICollection<string> InterestingTaskNames {
			get { return interestingTaskNames; }
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
			b.AppendLine("  " + InterestingTaskNames.Count + " Interesting Task Names:");
			foreach (string name in InterestingTaskNames) {
				b.AppendLine("    " + name);
			}
			return b.ToString();
		}
		
		[NonSerialized]
		internal Action CancelCallback;
		
		public void Cancel()
		{
			if (CancelCallback != null) {
				CancelCallback();
			}
		}
	}
}
