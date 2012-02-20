// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.NRefactory.ConsistencyCheck.Xml
{
	public class TextSourceWithVersion : ITextSource
	{
		readonly ITextSource textSource;
		readonly ITextSourceVersion version;
		
		public TextSourceWithVersion(ITextSource textSource, ITextSourceVersion version)
		{
			this.textSource = textSource;
			this.version = version;
		}
		
		ITextSourceVersion ITextSource.Version {
			get { return version; }
		}
		
		int ITextSource.TextLength {
			get { return textSource.TextLength; }
		}
		
		string ITextSource.Text {
			get { return textSource.Text; }
		}
		
		ITextSource ITextSource.CreateSnapshot()
		{
			throw new NotImplementedException();
		}
		
		ITextSource ITextSource.CreateSnapshot(int offset, int length)
		{
			throw new NotImplementedException();
		}
		
		System.IO.TextReader ITextSource.CreateReader()
		{
			throw new NotImplementedException();
		}
		
		System.IO.TextReader ITextSource.CreateReader(int offset, int length)
		{
			throw new NotImplementedException();
		}
		
		char ITextSource.GetCharAt(int offset)
		{
			return textSource.GetCharAt(offset);
		}
		
		string ITextSource.GetText(int offset, int length)
		{
			return textSource.GetText(offset, length);
		}
		
		string ITextSource.GetText(ISegment segment)
		{
			return textSource.GetText(segment);
		}
		
		int ITextSource.IndexOf(char c, int startIndex, int count)
		{
			return textSource.IndexOf(c, startIndex, count);
		}
		
		int ITextSource.IndexOfAny(char[] anyOf, int startIndex, int count)
		{
			return textSource.IndexOfAny(anyOf, startIndex, count);
		}
		
		int ITextSource.IndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
		{
			return textSource.IndexOf(searchText, startIndex, count, comparisonType);
		}
		
		int ITextSource.LastIndexOf(char c, int startIndex, int count)
		{
			return textSource.LastIndexOf(c, startIndex, count);
		}
		
		int ITextSource.LastIndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
		{
			return textSource.LastIndexOf(searchText, startIndex, count, comparisonType);
		}
	}
}
