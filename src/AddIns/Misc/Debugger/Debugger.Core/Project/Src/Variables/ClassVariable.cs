// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger 
{
	public class ClassVariable: Variable
	{
		bool isStatic;
		bool isPublic;
		
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
		
		public ClassVariable(NDebugger debugger, string name, bool isStatic, bool isPublic, ValueGetter valueGetter): base(debugger, name, valueGetter)
		{
			this.isStatic = isStatic;
			this.isPublic = isPublic;
		}
		
	}
}
