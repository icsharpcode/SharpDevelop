// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision: 1766 $</version>
// </file>

/*
 * User: Dickon Field
 * Date: 12/06/2006
 * Time: 06:25
 */

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace SharpServerTools.Forms
{
	/// <summary>
	/// Enables a user to browse metadata associated with a db server and to open resources
	/// referenced therein. The intention is to extend this to other server processes over
	/// time.
	/// </summary>
	public class ServerBrowserTool : AbstractPadContent
	{
		Panel ctl;
		ServerToolTreeView dbTree;
		
		/// <summary>
		/// ServerBrowserTool hosts one or more TreeViews providing views of types
		/// of server. Currently it shows only relational database servers.
		/// </summary>
		public ServerBrowserTool()
		{
			LoggingService.Debug("Loading ServerBrowserTool");
			dbTree = new ServerToolTreeView();
			dbTree.Dock = DockStyle.Fill;
			ctl = new Panel();
			ctl.Controls.Add(dbTree);
			dbTree.Rebuild();
		}
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override Control Control {
			get {
				return ctl;
			}
		}
		
		/// <summary>
		/// Rebuildes the pad
		/// </summary>
		public override void RedrawContent()
		{
			
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			ctl.Dispose();
		}
	}
}





