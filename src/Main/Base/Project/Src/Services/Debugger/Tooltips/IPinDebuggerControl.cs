// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;

using ICSharpCode.SharpDevelop.Bookmarks;

namespace ICSharpCode.SharpDevelop.Debugging
{
	public interface IPinDebuggerControl
	{
		void Open();
		void Close();
		PinBookmark Mark { get; set; }
		IEnumerable<ITreeNode> ItemsSource { set; }
		Point Location { get; set; }
	}
}
