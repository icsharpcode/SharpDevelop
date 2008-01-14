// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class BaseClass2
	{
		public string basePublic = "a";
		string basePrivate = "b";
	}
	
	public class ObjectValue: BaseClass2
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
