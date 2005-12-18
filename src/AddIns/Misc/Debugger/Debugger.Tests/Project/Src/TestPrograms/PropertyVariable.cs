// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class PropertyVariable
	{
		string PrivateProperty {
			get {
				return "private";
			}
		}
		
		public string PublicProperty {
			get {
				return "public";
			}
		}
		
		public string ExceptionProperty {
			get {
				throw new NotSupportedException();
			}
		}
		
		public static string StaticProperty {
			get {
				return "static";
			}
		}
		
		public static void Main()
		{
			PropertyVariable var = new PropertyVariable();
			System.Diagnostics.Debugger.Break();
			System.Diagnostics.Debugger.Break();
		}
	}
}
