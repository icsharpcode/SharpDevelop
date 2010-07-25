// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 6084 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser
{
	public class SavepointEventArgs : EventArgs
	{
		public Location SavepointLocation { get; private set; }
		public LexerMemento State { get; private set; }
		
		public SavepointEventArgs(Location savepointLocation, LexerMemento state)
		{
			this.SavepointLocation = savepointLocation;
			this.State = state;
		}
	}
}
