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
	public class PythonControlCursorProperty : PythonControlProperty
	{
		public PythonControlCursorProperty()
		{
		}
		
		public override bool IsDefaultValue(object propertyValue)
		{
			Cursor cursor = propertyValue as Cursor;
			if (cursor != null) {
				return cursor == Cursors.Default;
			}
			return false;
		}
	}
}
