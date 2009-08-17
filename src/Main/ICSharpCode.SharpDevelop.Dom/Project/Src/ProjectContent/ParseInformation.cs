// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Holds the parse information for a file.
	/// This class is immutable and thread-safe.
	/// </summary>
	public class ParseInformation : Immutable
	{
		ICompilationUnit unit;
		
		/// <summary>
		/// Gets the compilation unit.
		/// </summary>
		public ICompilationUnit CompilationUnit {
			get { return unit; }
		}
		
		/// <summary>
		/// Gets the last compilation unit that was valid (=no parse errors).
		/// This property might be null.
		/// </summary>
		public ICompilationUnit ValidCompilationUnit { get { return unit; } }
		
		/// <summary>
		/// Gets the last compilation unit that was invalid (=had parse errors).
		/// This property is null if the most recent compilation unit is valid.
		/// </summary>
		public ICompilationUnit DirtyCompilationUnit { get { return unit; } }
		
		/// <summary>
		/// Gets the best compilation unit.
		/// This returns the ValidCompilationUnit if one exists, otherwise
		/// the DirtyCompilationUnit.
		/// </summary>
		public ICompilationUnit BestCompilationUnit { get { return unit; } }
		
		/// <summary>
		/// Gets the most recent compilation unit. The unit might be valid or invalid.
		/// </summary>
		public ICompilationUnit MostRecentCompilationUnit { get { return unit; } }
		
		public ParseInformation(ICompilationUnit unit)
		{
			if (unit == null)
				throw new ArgumentNullException("unit");
			unit.Freeze();
//			if (!unit.IsFrozen)
//				throw new ArgumentException("unit must be frozen for use in ParseInformation");
			this.unit = unit;
		}
	}
}
