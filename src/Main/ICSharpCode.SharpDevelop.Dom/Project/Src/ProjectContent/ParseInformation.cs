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
		ICompilationUnit validCompilationUnit;
		ICompilationUnit dirtyCompilationUnit;
		
		public ICompilationUnit ValidCompilationUnit {
			get {
				return validCompilationUnit;
			}
			set {
				validCompilationUnit = value;
			}
		}
		
		public ICompilationUnit DirtyCompilationUnit {
			get {
				return dirtyCompilationUnit;
			}
			set {
				dirtyCompilationUnit = value;
			}
		}
		
		public ICompilationUnit BestCompilationUnit {
			get {
				return validCompilationUnit == null ? dirtyCompilationUnit : validCompilationUnit;
			}
		}
		
		public ICompilationUnit MostRecentCompilationUnit {
			get {
				return dirtyCompilationUnit == null ? validCompilationUnit : dirtyCompilationUnit;
			}
		}
	}
}
