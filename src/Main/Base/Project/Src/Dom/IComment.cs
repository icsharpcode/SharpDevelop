// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IComment
	{
		bool IsBlockComment {
			get;
		}
		
		string CommentTag {
			get;
		}
		
		string CommentText {
			get;
		}
		
		IRegion Region {
			get;
		}
	}
}
