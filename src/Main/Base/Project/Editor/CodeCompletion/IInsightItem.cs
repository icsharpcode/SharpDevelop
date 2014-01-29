// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// An item in the insight window.
	/// </summary>
	public interface IInsightItem : INotifyPropertyChanged
	{
		object Header { get; }
		object Content { get; }
	}
}
