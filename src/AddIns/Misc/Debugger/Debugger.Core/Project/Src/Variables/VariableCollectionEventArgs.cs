// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger 
{	
	[Serializable]
	public class VariableCollectionEventArgs: DebuggerEventArgs
	{
		VariableCollection variableCollection;
		
		public VariableCollection VariableCollection {
			get {
				return variableCollection;
			}
		}
		
		public VariableCollectionEventArgs(VariableCollection variableCollection): base(variableCollection.Debugger)
		{
			this.variableCollection = variableCollection;
		}
	}
}
