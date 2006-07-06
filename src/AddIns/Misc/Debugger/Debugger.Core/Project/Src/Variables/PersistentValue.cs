// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// PersistentValue is a container used to obtain the value of a given object even after continue.
	/// </summary>
	public class PersistentValue
	{
		/// <summary>
		/// Delegate that is used to get value. This delegate may be called at any time and should never return null.
		/// </summary>
		public delegate Value ValueGetter();
		
		
		ValueGetter getter;
		
		public Value Value {
			get {
				return getter();
			}
		}
		
		public PersistentValue(ValueGetter getter)
		{
			this.getter = getter;
		}
	}
}
