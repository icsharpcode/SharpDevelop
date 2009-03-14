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
	public class PythonControlPointProperty : PythonControlProperty
	{
		Point defaultPoint;
		
		public PythonControlPointProperty(int x, int y)
		{
			defaultPoint = new Point(x, y);
		}
		
		public override bool IsDefaultValue(object propertyValue)
		{
			if (propertyValue is Point) {
				return (Point)propertyValue == defaultPoint;
			}
			return false;
		}
	}
}
