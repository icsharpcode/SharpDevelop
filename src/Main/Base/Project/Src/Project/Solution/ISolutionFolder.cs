// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ISolutionFolderContainer.
	/// </summary>
	public interface ISolutionFolder
	{
		ISolutionFolderContainer Parent {
			get;
			set;
		}
		
		string TypeGuid {
			get;
			set;
		}
		
		string IdGuid {
			get;
			set;
		}
		
		string Location {
			get;
			set;
		}
		
		string Name {
			get;
			set;
		}
	}
}
