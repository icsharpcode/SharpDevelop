// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// An &lt;Import&gt;-node in an MSBuild file.
	/// </summary>
	public class MSBuildImport : ICloneable, IEquatable<MSBuildImport>
	{
		string project;
		string condition;
		
		public string Project {
			get {
				return project;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				project = value;
			}
		}
		
		public string Condition {
			get {
				return condition;
			}
			set {
				condition = value;
			}
		}
		
		public MSBuildImport(string project)
		{
			this.Project = project;
		}
		
		public object Clone()
		{
			MSBuildImport n = new MSBuildImport(this.Project);
			n.Condition = this.Condition;
			return n;
		}
		
		public sealed override bool Equals(object obj)
		{
			return Equals(obj as MSBuildImport);
		}
		
		public override int GetHashCode()
		{
			return this.Project.GetHashCode() ^ (this.Condition ?? "").GetHashCode();
		}
		
		public virtual bool Equals(MSBuildImport obj)
		{
			if (obj == null)
				return false;
			else
				return obj.Condition == this.Condition && obj.Project == this.Project;
		}
	}
}
