// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace ICSharpCode.SharpDevelop.Editor
{
	public class AvalonEditInsightWindow : InsightWindow, IInsightWindow
	{
		public AvalonEditInsightWindow(TextArea textArea) : base(textArea)
		{
		}
		
		ObservableCollection<IInsightItem> items = new ObservableCollection<IInsightItem>();
		
		public IList<IInsightItem> Items {
			get { return items; }
		}
		
		public IInsightItem SelectedItem {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
	}
}
