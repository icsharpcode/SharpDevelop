using System;
using System.IO;
using System.Reflection;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ReferenceProjectItem.
	/// </summary>
	public class UnknownProjectItem : ProjectItem
	{
		string tag;
		
		public override string Tag {
			get {
				return ItemType.ToString();
			}
		}
		
		public override ItemType ItemType {
			get {
				return ItemType.Unknown;
			}
		}
		
		public UnknownProjectItem(IProject project, string tag) : base(project)
		{
			this.tag = tag;
		}
	}
}
