// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	class PlaceHolderItem: VariableListItem
	{
		public override bool IsValid {
			get {
				return false;
			}
		}
	}
}
