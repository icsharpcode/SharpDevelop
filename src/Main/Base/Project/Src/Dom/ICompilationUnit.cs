// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface ICompilationUnit : ICompilationUnitBase
	{
		List<IUsing> Usings {
			get;
		}
		
		List<IAttributeSection> Attributes {
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
	}
}
