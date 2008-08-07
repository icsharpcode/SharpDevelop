// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.ServerTools;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// Enables a user to browse metadata associated with a db server and to open resources
	/// referenced therein. The intention is to extend this to other server processes over
	/// time.
	/// </summary>
	public class ServerBrowserHost : AbstractPadContent
	{
		ServerControl serverControl;

		/// <summary>
		/// ServerBrowserTool hosts one or more TreeViews providing views of types
		/// of server. Currently it shows only relational database servers.
		/// </summary>
		public ServerBrowserHost()
		{
			LoggingService.Debug("Loading ServerBrowserHost");
			serverControl = new ServerControl();
		}
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override object Content {
			get {
				return serverControl;
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
		}
	}
}
