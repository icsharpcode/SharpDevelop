// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Reflection;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// A project item whose type is not known by SharpDevelop.
	/// </summary>
	public class UnknownProjectItem : ProjectItem
	{
		string tag;
		
		public override string Tag {
			get {
				return tag;
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
		
		public override ProjectItem Clone()
		{
			ProjectItem n = new UnknownProjectItem(this.Project, this.Tag);
			n.Include = this.Include;
			this.CopyExtraPropertiesTo(n);
			return n;
		}
	}
}
