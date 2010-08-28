// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Folding;

namespace ICSharpCode.XmlEditor
{
	public interface IFoldingManager
	{
		void UpdateFoldings(IEnumerable<NewFolding> newFoldings, int firstErrorOffset);
	}
}
