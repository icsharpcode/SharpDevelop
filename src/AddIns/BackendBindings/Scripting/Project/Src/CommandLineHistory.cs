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

namespace ICSharpCode.Scripting
{
	public class CommandLineHistory
	{
		List<string> lines = new List<string>();
		int position;
		
		public CommandLineHistory()
		{
		}
		
		/// <summary>
		/// Adds the command line to the history.
		/// </summary>
		public void Add(string line)
		{
			if (!String.IsNullOrEmpty(line)) {
				int index = lines.Count - 1;
				if (index >= 0) {
					if (lines[index] != line) {
						lines.Add(line);
					}
				} else {
					lines.Add(line);
				}
			}
			position = lines.Count;
		}
		
		/// <summary>
		/// Gets the current command line. By default this will be the last command line entered.
		/// </summary>
		public string Current {
			get { 
				if ((position >= 0) && (position < lines.Count)) {
					return lines[position];
				}
				return null;
			}
		}
		
		/// <summary>
		/// Moves to the next command line.
		/// </summary>
		/// <returns>False if the current position is at the end of the command line history.</returns>
		public bool MoveNext()
		{
			int nextPosition = position + 1;
			if (nextPosition < lines.Count) {
				++position;
			}
			return nextPosition < lines.Count;
		}
		
		/// <summary>
		/// Moves to the previous command line.
		/// </summary>
		/// <returns>False if the current position is at the start of the command line history.</returns>
		public bool MovePrevious()
		{
			if (position >= 0) {
				if (position == 0) {
					return false;
				}
				--position;
			}
			return position >= 0;
		}
	}
}
