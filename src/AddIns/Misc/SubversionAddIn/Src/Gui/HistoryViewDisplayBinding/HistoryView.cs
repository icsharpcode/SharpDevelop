// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Undo;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace ICSharpCode.Svn
{
	public class HistoryView : AbstractSecondaryViewContent
	{
		HistoryViewPanel historyViewPanel;
		
		#region ICSharpCode.SharpDevelop.Gui.AbstractSecondaryViewContent abstract class implementation
		public override Control Control {
			get {
				return historyViewPanel;
			}
		}
		
		public override string TabPageText {
			get {
				return "History";
			}
		}
		#endregion
		
		public HistoryView(IViewContent viewContent)
		{
			this.historyViewPanel = new HistoryViewPanel(viewContent);
		}
	}
}
