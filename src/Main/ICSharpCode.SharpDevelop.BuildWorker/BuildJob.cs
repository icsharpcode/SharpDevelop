// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ICSharpCode.SharpDevelop.BuildWorker
{
	/// <summary>
	/// The settings used to start a build.
	/// </summary>
	class BuildJob
	{
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
//			b.AppendLine("  " + AdditionalImports.Count + " Additional Imports:");
//			foreach (string import in AdditionalImports) {
//				b.AppendLine("    " + import);
//			}
			b.AppendLine("  " + InterestingTaskNames.Count + " Interesting Task Names:");
			foreach (string name in InterestingTaskNames) {
				b.AppendLine("    " + name);
			}
			return b.ToString();
		}
		
		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(this.ProjectFileName);
			writer.Write(this.Target);
			writer.WriteInt32((int)eventMask);
			
			writer.WriteInt32(properties.Count);
			foreach (var pair in properties) {
				writer.Write(pair.Key);
				writer.Write(pair.Value);
			}
			
			writer.WriteInt32(interestingTaskNames.Count);
			foreach (string taskName in interestingTaskNames)
				writer.Write(taskName);
		}
		
		public static BuildJob ReadFrom(BinaryReader reader)
		{
			BuildJob job = new BuildJob();
			job.ProjectFileName = reader.ReadString();
			job.Target = reader.ReadString();
			job.EventMask = (EventTypes)reader.ReadInt32();
			int c = reader.ReadInt32();
			for (int i = 0; i < c; i++) {
				job.properties.Add(reader.ReadString(), reader.ReadString());
			}
			c = reader.ReadInt32();
			for (int i = 0; i < c; i++) {
				job.interestingTaskNames.Add(reader.ReadString());
			}
			return job;
		}
	}
}
