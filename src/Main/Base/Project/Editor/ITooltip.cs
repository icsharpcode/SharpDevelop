// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;

namespace ICSharpCode.SharpDevelop.Editor
{
	public interface ITooltip
	{
		/// <summary> Should the tooltip close when the mouse moves away? </summary>
		bool CloseWhenMouseMovesAway { get; }
	}
}
