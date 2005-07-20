// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface ICompilationUnit
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
		
		List<IUsing> Usings {
			get;
		}
		
		List<IAttribute> Attributes {
			get;
		}
		
		List<IClass> Classes {
			get;
		}
		
		List<IComment> MiscComments {
			get;
		}
		
		List<IComment> DokuComments {
			get;
		}
		
		List<Tag> TagComments {
			get;
		}
		
		List<FoldingRegion> FoldingRegions {
			get;
		}
		
		
		/// <summary>
		/// Returns the innerst class in which the carret currently is, returns null
		/// if the carret is outside any class boundaries.
		/// </summary>
		IClass GetInnermostClass(int caretLine, int caretColumn);
		
		List<IClass> GetOuterClasses(int caretLine, int caretColumn);
	}
}
