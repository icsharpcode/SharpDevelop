// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
	/// Description of ReferenceProjectItem.
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
	}
}
