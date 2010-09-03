// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
