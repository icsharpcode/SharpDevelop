// 
// Script.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;

namespace ICSharpCode.TypeScriptBinding.Hosting
{
	public class Script
	{
		string fileName;
		List<int> lineStartPositions = new List<int>();
		
		public Script(string fileName, string source)
		{
			this.fileName = fileName;
			this.Source = source;
			this.Version = 1;
		}
		
		public void Update(string source)
		{
			this.Source = source;
			Version++;
		}
		
		public string Id {
			get { return fileName; }
		}
		
		public string FileName {
			get { return fileName; }
		}
		
		public string Source { get; private set; }
		public int Version { get; private set; }
		
		public int[] GetLineStartPositions()
		{
			if (lineStartPositions.Count == 0) {
				string[] lines = Source.Split('\r');
				lineStartPositions.Add(0);
				int position = 0;
				for (int i = 0; i < lines.Length; ++i) {
					position += lines[i].Length + 2;
					lineStartPositions.Add(position);
				}
			}
			
			return lineStartPositions.ToArray();
		}
		
		public TextChangeRange GetTextChangeRange(IScriptSnapshotShim oldSnapshot)
		{
			return new TextChangeRange(0, oldSnapshot.getLength(), Source.Length);
		}
	}
}
