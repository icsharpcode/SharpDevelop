// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom.VBNet
{
	/// <summary>
	/// Description of VBNetExpressionFinder.
	/// </summary>
	public class VBNetExpressionFinder : IExpressionFinder
	{
		ParseInformation parseInformation;
		IProjectContent projectContent;
		
		public VBNetExpressionFinder(ParseInformation parseInformation)
		{
			this.parseInformation = parseInformation;
			if (parseInformation != null && parseInformation.CompilationUnit != null) {
				projectContent = parseInformation.CompilationUnit.ProjectContent;
			} else {
				projectContent = DefaultProjectContent.DummyProjectContent;
			}
		}
		
		public ExpressionResult FindExpression(string text, int offset)
		{
			throw new NotImplementedException();
		}
		
		public ExpressionResult FindFullExpression(string text, int offset)
		{
			throw new NotImplementedException();
		}
		
		public string RemoveLastPart(string expression)
		{
			throw new NotImplementedException();
		}
	}
}
