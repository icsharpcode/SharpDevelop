// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;

namespace ICSharpCode.PythonBinding
{
	public class PythonControlSizeProperty : PythonControlProperty
	{
		Size defaultSize;
		
		public PythonControlSizeProperty(int width, int height)
		{
			defaultSize = new Size(width, height);
		}
		
		public override bool IsDefaultValue(object propertyValue)
		{
			if (propertyValue is Size) {
				return (Size)propertyValue == defaultSize;
			}
			return false;
		}
	}
}
