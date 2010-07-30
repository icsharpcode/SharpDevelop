// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 6028 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using CSTokens = ICSharpCode.NRefactory.Parser.CSharp.Tokens;
using VBTokens = ICSharpCode.NRefactory.Parser.VB.Tokens;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	public interface IInsightWindowHandler
	{
		void InitializeOpenedInsightWindow(ITextEditor editor, IInsightWindow insightWindow);
		bool InsightRefreshOnComma(ITextEditor editor, char ch);
	}
}
