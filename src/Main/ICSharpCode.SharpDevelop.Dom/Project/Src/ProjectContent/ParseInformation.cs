// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class ParseInformation
	{
		public ICompilationUnit ValidCompilationUnit { get; private set; }
		public ICompilationUnit DirtyCompilationUnit { get; private set; }
		public ICompilationUnit BestCompilationUnit { get; private set; }
		public ICompilationUnit MostRecentCompilationUnit { get; private set; }
		
		public ParseInformation() {}
		public ParseInformation(ICompilationUnit c)
		{
			SetCompilationUnit(c);
		}
		
		/// <summary>
		/// Uses the specified compilation unit.
		/// If the compilation unit is valid (ErrorsDuringCompile=false), it is used as ValidCompilationUnit,
		/// BestCompilationUnit and MostRecentCompilationUnit, and DirtyCompilationUnit is set to null.
		/// If the compilation unit is dirty (ErrorsDuringCompile=true), it is used as
		/// DirtyCompilationUnit and MostRecentCompilationUnit, (and BestCompilationUnit if there is no ValidCompilationUnit)
		/// ValidCompilationUnit keeps the old value.
		/// </summary>
		public void SetCompilationUnit(ICompilationUnit unit)
		{
			lock (this) {
				MostRecentCompilationUnit = unit;
				if (unit.ErrorsDuringCompile) {
					DirtyCompilationUnit = unit;
					MostRecentCompilationUnit = unit;
					if (ValidCompilationUnit == null)
						BestCompilationUnit = unit;
				} else {
					ValidCompilationUnit = unit;
					BestCompilationUnit = unit;
					DirtyCompilationUnit = null;
				}
			}
		}
	}
}
