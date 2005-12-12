// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class BaseClass
	{
		
	}
	
	public class ObjectValue: BaseClass
	{
		string privateField = "private";
		public string publicFiled = "public";
		
		public string PublicProperty {
			get {
				return privateField;
			}
		}
		
		public static void Main()
		{
			ObjectValue val = new ObjectValue();
			System.Diagnostics.Debugger.Break();
			val.privateField = "new private";
			System.Diagnostics.Debugger.Break();
		}
	}
}
