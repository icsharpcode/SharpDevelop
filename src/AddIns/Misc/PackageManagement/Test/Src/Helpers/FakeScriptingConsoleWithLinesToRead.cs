// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Scripting.Tests.Utils;

namespace PackageManagement.Tests.Helpers
{
	public class FakeScriptingConsoleWithLinesToRead : FakeScriptingConsole
	{
		public List<string> AllTextToReturnFromReadLine = new List<string>();
		
		public override string ReadLine(int autoIndentSize)
		{
			string line = AllTextToReturnFromReadLine.FirstOrDefault();
			if (AllTextToReturnFromReadLine.Any()) {
				AllTextToReturnFromReadLine.RemoveAt(0);
			}
			return line;
		}
	}
}
