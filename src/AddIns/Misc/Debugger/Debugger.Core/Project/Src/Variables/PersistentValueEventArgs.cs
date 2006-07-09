// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	public class PersistentValueEventArgs: EventArgs
	{
		PersistentValue persistentValue;
		
		public PersistentValue PersistentValue {
			get {
				return persistentValue;
			}
		}
		
		public PersistentValueEventArgs(PersistentValue persistentValue)
		{
			this.persistentValue = persistentValue;
		}
	}
}
