// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
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
		
		public ObservableCollection<ContextActionViewModel> Actions { get; set; }
		
		public string Title { get; set; }
	}
}
