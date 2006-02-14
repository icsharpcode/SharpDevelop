// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public abstract class Wrapper : MarshalByRefObject
	{
		public abstract object WrappedObject
		{
			get;
		}
		
		//public staticType WrappedObjectType
		//{
		//	get;
		//}
	}
}
