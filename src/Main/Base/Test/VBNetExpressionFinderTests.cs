// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.VBNet;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class VBNetExpressionFinderTests
	{
		#region init
		VBNetExpressionFinder ef;
		
		[SetUp]
		public void Init()
		{
			HostCallback.GetCurrentProjectContent = delegate {
				return ParserService.CurrentProjectContent;
			};
			
			ef = new VBNetExpressionFinder(null);
		}
		#endregion
		
		#region helpers
		void RunTestFull(string program, string lookingFor, string expectedExpression, ExpressionContext expectedContext, object expectedTag = null)
		{
			int offset = program.IndexOf(lookingFor);
			if (offset < 0) Assert.Fail("lookingFor not found!");
			ExpressionResult result = ef.FindFullExpression(program, offset);
			
			Assert.AreEqual(expectedExpression, result.Expression);
			Assert.AreEqual(expectedTag, result.Tag);
			Assert.AreEqual(expectedExpression, ExtractRegion(program, result.Region));
			Assert.AreEqual(expectedContext, result.Context);
		}
		
		void RunTest(string program, string lookingFor, string expectedExpression, ExpressionContext expectedContext, object expectedTag = null)
		{
			int offset = program.IndexOf(lookingFor);
			if (offset < 0) Assert.Fail("lookingFor not found!");
			ExpressionResult result = ef.FindExpression(program, offset);
			
			Assert.AreEqual(expectedExpression, result.Expression);
			Assert.AreEqual(expectedTag, result.Tag);
			Assert.AreEqual(expectedExpression, ExtractRegion(program, result.Region));
			Assert.AreEqual(expectedContext, result.Context);
		}
		
		static string ExtractRegion(string text, DomRegion region)
		{
			if (region.IsEmpty)
				return null;
			int start = GetOffsetByPosition(text, region.BeginLine, region.BeginColumn);
			int end = GetOffsetByPosition(text, region.EndLine, region.EndColumn);
			return text.Substring(start, end - start);
		}
		
		static int GetOffsetByPosition(string text, int line, int column)
		{
			if (line < 1)
				throw new ArgumentOutOfRangeException("line");
			if (line == 1)
				return column - 1;
			for (int i = 0; i < text.Length; i++) {
				if (text[i] == '\n') {
					if (--line == 1) {
						return i + column;
					}
				}
			}
			throw new ArgumentOutOfRangeException("line");
		}
		#endregion
		
		#region context1
		const string context1 = @"
Public Class Test
	Public Sub New()
		
	End Sub
End Class
";
		#endregion
	}
}
