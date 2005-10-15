// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
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
				return ConfigurationManager.AppSettings["ConsoleApp"];
			}
		}
	}
}
