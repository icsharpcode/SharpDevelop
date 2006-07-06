// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger 
{
	public class ClassVariable: Variable
	{
		internal bool isStatic;
		internal bool isPublic;
		
		public bool IsStatic {
			get {
				return isStatic;
			}
		}
		
		public bool IsPublic {
			get {
				return isPublic;
			}
		}
		
		public ClassVariable(NDebugger debugger, string name, bool isStatic, bool isPublic, PersistentValue pValue): base(debugger, name, pValue)
		{
			this.isStatic = isStatic;
			this.isPublic = isPublic;
		}
		
	}
}
