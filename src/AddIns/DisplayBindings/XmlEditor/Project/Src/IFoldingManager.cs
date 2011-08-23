// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Folding;

namespace ICSharpCode.XmlEditor
{
	public interface IFoldingManager : IDisposable
	{
		void UpdateFoldings(IEnumerable<NewFolding> newFoldings, int firstErrorOffset);
	}
}
