// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface ICompilationUnit : IFreezable
	{
		string FileName {
			get;
			set;
		}
		
		bool ErrorsDuringCompile {
			get;
			set;
		}
		
		object Tag {
			get;
			set;
		}
		
		IProjectContent ProjectContent {
			get;
		}
		
		/// <summary>
		/// Gets the main using scope of the compilation unit.
		/// That scope usually represents the root namespace.
		/// </summary>
		IUsingScope UsingScope {
			get;
		}
		
		IList<IAttribute> Attributes {
			get;
		}
		
		IList<IClass> Classes {
			get;
		}
		
		IList<IComment> MiscComments {
			get;
		}
		
		IList<IComment> DokuComments {
			get;
		}
		
		IList<TagComment> TagComments {
			get;
		}
		
		IList<FoldingRegion> FoldingRegions {
			get;
		}
		
		/// <summary>
		/// Returns the innerst class in which the carret currently is, returns null
		/// if the carret is outside any class boundaries.
		/// </summary>
		IClass GetInnermostClass(int caretLine, int caretColumn);
	}
}
