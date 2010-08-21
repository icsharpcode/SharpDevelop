// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	public class FakeFoldingManager : IFoldingManager
	{
		public List<NewFolding> NewFoldsPassedToUpdateFoldings = new List<NewFolding>();
		public int FirstErrorOffset;
		public bool IsUpdateFoldingsCalled;
		
		public FakeFoldingManager()
		{
		}
		
		public void UpdateFoldings(IEnumerable<NewFolding> newFoldings, int firstErrorOffset)
		{
			IsUpdateFoldingsCalled = true;
			NewFoldsPassedToUpdateFoldings.AddRange(newFoldings);
			FirstErrorOffset = firstErrorOffset;
		}
	}
}
