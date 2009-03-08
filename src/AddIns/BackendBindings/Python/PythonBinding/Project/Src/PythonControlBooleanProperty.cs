// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.PythonBinding
{
	public class PythonControlBooleanProperty : PythonControlProperty
	{
		bool defaultValue;
		
		public PythonControlBooleanProperty(bool defaultValue)
		{
			this.defaultValue = defaultValue;
		}
		
		public override bool IsDefaultValue(object propertyValue)
		{
			return (bool)propertyValue == defaultValue;
		}
	}
}
