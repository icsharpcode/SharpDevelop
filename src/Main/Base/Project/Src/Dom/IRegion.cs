// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IRegion: IComparable
	{
		int BeginLine {
			get;
		}

		int BeginColumn {
			get;
		}

		int EndColumn {
			get;
			set;
		}

		int EndLine {
			get;
			set;
		}

		bool IsInside(int row, int column);
	}
}
