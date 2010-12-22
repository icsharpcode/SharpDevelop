// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn.MyersDiff
{
	public class DocumentSequence : ISequence
	{
		IDocument document;
		List<int> hashCodes;
		
		public DocumentSequence(IDocument document)
		{
			this.document = document;
			this.hashCodes = new List<int>();
		}
		
		public int Size()
		{
			return document.TotalNumberOfLines;
		}
		
		public bool Equals(int i, ISequence other, int j)
		{
			DocumentSequence seq = other as DocumentSequence;
			
			if (seq == null)
				return false;
			
			IDocumentLine thisLine = document.GetLine(i + 1);
			IDocumentLine otherLine = seq.document.GetLine(j + 1);
			
			return thisLine.Text == otherLine.Text;
		}
	}
}
