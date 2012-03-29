// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Widgets.MyersDiff;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class DocumentSequence : ISequence
	{
		int[] hashes;
		
		public DocumentSequence(IDocument document, Dictionary<string, int> hashDict)
		{
			this.hashes = new int[document.TotalNumberOfLines];
			
			// Construct a perfect hash for the document lines, and store the 'hash code'
			// (really just a unique identifier for each line content) in our array.
			for (int i = 1; i <= document.TotalNumberOfLines; i++) {
				string text = document.GetLine(i).Text;
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
