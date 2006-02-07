// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
