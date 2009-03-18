// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop
{
	public interface ICompletionItem
	{
		string Text { get; }
		string Description { get; }
	}
}
