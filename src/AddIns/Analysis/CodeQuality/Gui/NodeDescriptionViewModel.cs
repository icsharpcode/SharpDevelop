// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
				base.OnPropertyChanged();
				base.OnPropertyChanged("ClassType");
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
				base.OnPropertyChanged();
			}
		}
		
	}
}
