// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Editor.ContextActions
{
	/// <summary>
	/// ViewModel for a ContextActionsPopup.
	/// </summary>
	public class ContextActionsPopupViewModel
	{
		public ImageSource Image { get; set; }
		
		public string Title { get; set; }
		
		public ObservableCollection<ContextActionViewModel> Actions { get; set; }		
	}
}