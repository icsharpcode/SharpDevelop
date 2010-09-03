// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ContextActionsViewModel.
	/// </summary>
	public class ContextActionsViewModel
	{
		public ImageSource Image { get; set; }
		
		public string Title { get; set; }
		
		public ObservableCollection<ContextActionViewModel> Actions { get; set; }		
	}
}
