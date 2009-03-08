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
	public class PythonControlAutoScaleModeProperty : PythonControlProperty
	{
		public PythonControlAutoScaleModeProperty()
		{
		}
		
		public override bool IsDefaultValue(object propertyValue)
		{
			if (propertyValue is AutoScaleMode) {
				return (AutoScaleMode)propertyValue == AutoScaleMode.Inherit;
			}
			return false;
		}
	}
}
