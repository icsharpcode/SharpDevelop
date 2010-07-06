// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;

namespace ICSharpCode.SharpDevelop.Dom.VBNet
{
	/// <summary>
	/// Description of VBNetExpressionFinder.
	/// </summary>
	public class VBNetExpressionFinder : IExpressionFinder
	{
		ParseInformation parseInformation;
		IProjectContent projectContent;
		ILexer lexer;
		Location targetPosition;
		List<int> lineOffsets;
		
		int LocationToOffset(Location location)
		{
			if (location.Line <= 0) return -1;
			return lineOffsets[location.Line - 1] + location.Column - 1;
		}
		
		Location OffsetToLocation(int offset)
		{
			int lineNumber = lineOffsets.BinarySearch(offset);
			if (lineNumber < 0) {
				lineNumber = (~lineNumber) - 1;
			}
			return new Location(offset - lineOffsets[lineNumber] + 1, lineNumber + 1);
		}
		
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
			Init(text, offset);
			
			ExpressionFinder p = new ExpressionFinder();
			lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new StringReader(text));
			Token t;
			do {
				t = lexer.NextToken();
				p.InformToken(t);
			} while (t.Location < targetPosition);
			
			Block block = p.CurrentBlock;
			
			int tokenOffset;
			if (t == null || t.Kind == Tokens.EOF)
				tokenOffset = text.Length;
			else
				tokenOffset = LocationToOffset(t.Location);
			int lastExpressionStartOffset = LocationToOffset(block.lastExpressionStart);
			if (lastExpressionStartOffset >= 0) {
				if (offset < tokenOffset) {
					// offset is in front of this token
					return MakeResult(text, lastExpressionStartOffset, tokenOffset, GetContext(block));
				} else {
					// offset is IN this token
					return MakeResult(text, lastExpressionStartOffset, offset, GetContext(block));
				}
			} else {
				return new ExpressionResult(null, GetContext(block));
			}
		}

		ExpressionResult MakeResult(string text, int startOffset, int endOffset, ExpressionContext context)
		{
			return new ExpressionResult(text.Substring(startOffset, endOffset - startOffset).Trim(),
			                            DomRegion.FromLocation(OffsetToLocation(startOffset), OffsetToLocation(endOffset)),
			                            context, null);
		}
		
		void Init(string text, int offset)
		{
			lineOffsets = new List<int>();
			lineOffsets.Add(0);
			for (int i = 0; i < text.Length; i++) {
				if (i == offset) {
					targetPosition = new Location(offset - lineOffsets[lineOffsets.Count - 1] + 1, lineOffsets.Count);
				}
				if (text[i] == '\n') {
					lineOffsets.Add(i + 1);
				} else if (text[i] == '\r') {
					if (i + 1 < text.Length && text[i + 1] != '\n') {
						lineOffsets.Add(i + 1);
					}
				}
			}
			if (offset == text.Length) {
				targetPosition = new Location(offset - lineOffsets[lineOffsets.Count - 1] + 1, lineOffsets.Count);
			}
		}
		
		ExpressionContext GetContext(Block block)
		{
			switch (block.context) {
				case Context.Global:
					return ExpressionContext.Global;
				case Context.IdentifierExpected:
					return ExpressionContext.IdentifierExpected;
				case Context.Type:
					return ExpressionContext.TypeDeclaration;
			}
			
			return ExpressionContext.Default;
		}
		
		public ExpressionResult FindFullExpression(string text, int offset)
		{
			return FindExpression(text, offset);
		}
		
		public string RemoveLastPart(string expression)
		{
			return expression;
		}
		
		#region Helpers
		
		#endregion
	}
}
