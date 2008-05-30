// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 3047 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

using Aga.Controls.Tree;

using Debugger.Util;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.Pads;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// This TreeNodeAdv displays exception data.
	/// </summary>
	public class TreeViewExceptionNode: TreeNodeAdv
	{
		private static Dictionary<string, bool> expandedNodes = new Dictionary<string, bool>();
		
		private TreeViewAdv localVarList;
		
		/// <summary>
		/// The content of the exception
		/// </summary>
		private AbstractNode content;
		
		// TODO: I don't know if I need localVarList
		private TreeViewExceptionNode(TreeViewAdv localVarList, ExceptionNode exception): base(localVarList, new object()) {
			this.localVarList = localVarList;
			SetContentRecursive(exception);
		}
		
		/// <summary>
		/// A simple form of SetContentRecursive that changes the current ChildViewNode to
		/// display the exception parameter passed to it. If the node had any children and is expanded,
		/// it will recureively set those as well.
		/// </summary>
		/// <param name="exception">Contains the exception that will be stored in this particular TreeViewNode.</param>
		public static void SetContentRecursive(ExceptionNode exception) {
			throw new NotImplementedException();
		}
	}
}