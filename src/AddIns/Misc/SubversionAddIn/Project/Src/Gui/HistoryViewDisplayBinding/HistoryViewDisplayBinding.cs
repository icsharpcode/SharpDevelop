// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
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
using NSvn.Common;
using NSvn.Core;

namespace ICSharpCode.Svn
{
	public class HistoryViewDisplayBinding : ISecondaryDisplayBinding
	{
		public ICSharpCode.SharpDevelop.Gui.ISecondaryViewContent[] CreateSecondaryViewContent(ICSharpCode.SharpDevelop.Gui.IViewContent viewContent)
		{
			return new ICSharpCode.SharpDevelop.Gui.ISecondaryViewContent[] { new HistoryView(viewContent) };
		}
		
		private class SvnHelper {
			// load NSvn.Core only when a directory actually contains the ".svn" folder
			static Client client;
			
			internal static bool IsVersionControlled(string fileName)
			{
				if (client == null) {
					LoggingService.Info("SVN: HistoryViewDisplayBinding initializes client");
					client = new Client();
				}
				Status status = client.SingleStatus(Path.GetFullPath(fileName));
				return status != null && status.Entry != null;
			}
		}
		
		public bool CanAttachTo(ICSharpCode.SharpDevelop.Gui.IViewContent content)
		{
			if (content.IsUntitled || content.FileName == null || !File.Exists(content.FileName)) {
				return false;
			}
			string svnDir = Path.Combine(Path.GetDirectoryName(content.FileName), ".svn");
			if (!Directory.Exists(svnDir))
				return false;
			return SvnHelper.IsVersionControlled(content.FileName);
		}
	}
}
