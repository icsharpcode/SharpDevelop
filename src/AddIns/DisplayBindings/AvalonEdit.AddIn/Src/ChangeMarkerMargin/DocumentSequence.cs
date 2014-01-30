// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Widgets.MyersDiff;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class DocumentSequence : ISequence
	{
		int[] hashes;
		
		public DocumentSequence(IDocument document, Dictionary<string, int> hashDict)
		{
			this.hashes = new int[document.LineCount];
			
			// Construct a perfect hash for the document lines, and store the 'hash code'
			// (really just a unique identifier for each line content) in our array.
			for (int i = 1; i <= document.LineCount; i++) {
				string text = document.GetText(document.GetLineByNumber(i));
				int hash;
				if (!hashDict.TryGetValue(text, out hash)) {
					hash = hashDict.Count;
					hashDict.Add(text, hash);
				}
				hashes[i - 1] = hash;
			}
		}
		
		public int Size()
		{
			return hashes.Length;
		}
		
		public bool Equals(int thisIdx, ISequence other, int otherIdx)
		{
			DocumentSequence seq = other as DocumentSequence;
			
			if (seq == null)
				return false;
			
			return hashes[thisIdx] == seq.hashes[otherIdx];
		}
	}
}
