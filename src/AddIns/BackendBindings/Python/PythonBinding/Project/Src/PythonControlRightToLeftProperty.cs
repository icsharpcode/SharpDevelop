// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.PythonBinding
{
	public class PythonControlRightToLeftProperty : PythonControlProperty
	{
		public PythonControlRightToLeftProperty()
		{
		}
		
		public override bool IsDefaultValue(object propertyValue)
		{
			if (propertyValue is RightToLeft) {
				return (RightToLeft)propertyValue == RightToLeft.No;
			}
			return false;
		} 
	}
}
