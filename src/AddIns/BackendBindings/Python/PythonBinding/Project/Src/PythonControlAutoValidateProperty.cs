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
	/// <summary>
	/// Represents the AutoValidate property for a Form or ContainerControl.
	/// </summary>
	public class PythonControlAutoValidateProperty : PythonControlProperty
	{
		public PythonControlAutoValidateProperty()
		{
		}
		
		public override bool IsDefaultValue(object propertyValue)
		{
			if (propertyValue is AutoValidate) {
				return (AutoValidate)propertyValue == AutoValidate.EnablePreventFocusChange;
			}
			return false;
		}
	}
}
