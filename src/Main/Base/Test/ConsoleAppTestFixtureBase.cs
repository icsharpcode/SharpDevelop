// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Tests
{
	public class ConsoleAppTestFixtureBase
	{
		public static string GetConsoleAppFileName()
		{
			return typeof(ICSharpCode.NAntAddIn.Tests.ConsoleApp.ConsoleApp).Assembly.Location;
		}
	}
}
