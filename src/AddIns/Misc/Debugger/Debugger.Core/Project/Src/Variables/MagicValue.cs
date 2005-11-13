// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	/// <summary>
	/// This value may be returned by Variable.Value instead of null to provide some additional information.
	/// </summary>
	public class MagicValue
	{
		object val;
		
		public object Value {
			get {
				return val;
			}
			set {
				this.val = value;
			}
		}
		
		public MagicValue(object val)
		{
			this.val = val;
		}
		
		public override string ToString()
		{
			return val.ToString();
		}
	}
}
