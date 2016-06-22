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

namespace UnitTesting.Tests.Utils
{
	public class JumpedToFile
	{
		string fileName = String.Empty;
		int line = -2;
		int col = -2;
		
		public JumpedToFile(string fileName, int line, int col)
		{
			this.fileName = fileName;
			this.line = line;
			this.col = col;
		}
		
		public override string ToString()
		{
			return String.Format("File: {0} Line: {1} Col: {2}", fileName, line, col);
		}
		
		public override bool Equals(object obj)
		{
			JumpedToFile rhs = obj as JumpedToFile;
			if (rhs != null) {
				return (fileName == rhs.fileName) &&
					(line == rhs.line) &&
					(col == rhs.col);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
