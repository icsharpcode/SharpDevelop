// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Is thrown when the AddInTree could not find the requested path.
	/// </summary>
	public class TreePathNotFoundException : CoreException
	{
		/// <summary>
		/// Constructs a new <see cref="TreePathNotFoundException"/>
		/// </summary>
		public TreePathNotFoundException(string path) : base("Treepath not found: " + path)
		{
		}
	}
}
