// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Widgets.MyersDiff
{
	public class StringSequence : ISequence
	{
		string content;
		
		public StringSequence(string content)
		{
			this.content = content;
		}
		
		public int Size()
		{
			return content.Length;
		}
		
		public bool Equals(int i, ISequence other, int j)
		{
			StringSequence seq = other as StringSequence;
			
			if (seq == null)
				return false;
			
			return content[i] == seq.content[j];
		}
	}
}
