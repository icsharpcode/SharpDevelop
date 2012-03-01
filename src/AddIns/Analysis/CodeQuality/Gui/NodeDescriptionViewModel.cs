// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using ICSharpCode.CodeQuality;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.TreeView;

namespace ICSharpCode.CodeQuality.Gui
{
	public class NodeDescriptionViewModel : ViewModelBase
	{
		NodeBase node;
		
		public NodeBase Node {
			get { return node; }
			set {
				node = value;
//				Console.WriteLine("{0} - {1}",node.Uses.Count().ToString(),node.UsedBy.Count().ToString());
				base.RaisePropertyChanged(()=>Node);
				base.RaisePropertyChanged(()=>ClassType);
			}
		}
		
		
		public string ClassType {
			get {
				var n = node.GetType().Name;
				return (n.Substring(0,n.Length -4));
			}
		}
		
		object infoText;
		
		public object InfoText {
			get { return infoText; }
			set {
				infoText = value;
				base.RaisePropertyChanged(()=>InfoText);
			}
		}
		
	}
}
