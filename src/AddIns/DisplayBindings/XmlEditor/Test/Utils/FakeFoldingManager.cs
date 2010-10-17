// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public void Dispose()
		{
		}
	}
}
