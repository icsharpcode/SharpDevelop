// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Tests
{
	public class ConsoleAppTestFixtureBase
	{
		public static FileName GetConsoleAppFileName()
		{
			return FileName.Create(typeof(ICSharpCode.NAntAddIn.Tests.ConsoleApp.ConsoleApp).Assembly.Location);
		}
	}
}
