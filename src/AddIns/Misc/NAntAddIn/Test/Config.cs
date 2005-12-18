// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Configuration;

namespace ICSharpCode.NAntAddIn.Tests
{
	/// <summary>
	/// Description of Config.
	/// </summary>
	public class Config
	{
		private Config()
		{
		}
		
		/// <summary>
		/// Gets the console app exe we are going to use to test the
		/// ProcessRunner.
		/// </summary>
		public static string ConsoleAppFilename	{
			get	{
				return typeof(ICSharpCode.NAntAddIn.Tests.ConsoleApp.ConsoleApp).Assembly.Location;
			}
		}
	}
}
