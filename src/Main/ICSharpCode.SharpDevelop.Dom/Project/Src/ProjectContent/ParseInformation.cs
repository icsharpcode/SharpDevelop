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
		/// <summary>
		/// Gets the last compilation unit that was valid (=no parse errors).
		/// This property might be null.
		/// </summary>
		public ICompilationUnit ValidCompilationUnit { get; private set; }
		
		/// <summary>
		/// Gets the last compilation unit that was invalid (=had parse errors).
		/// This property is null if the most recent compilation unit is valid.
		/// </summary>
		public ICompilationUnit DirtyCompilationUnit { get; private set; }
		
		/// <summary>
		/// Gets the best compilation unit.
		/// This returns the ValidCompilationUnit if one exists, otherwise
		/// the DirtyCompilationUnit.
		/// </summary>
		public ICompilationUnit BestCompilationUnit { get; private set; }
		
		/// <summary>
		/// Gets the most recent compilation unit. The unit might be valid or invalid.
		/// </summary>
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
			if (unit == null)
				throw new ArgumentNullException("unit");
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
