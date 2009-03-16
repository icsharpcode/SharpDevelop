// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Profiler.Controller.Data;
using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using ICSharpCode.Profiler.Controller;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Profiler.AddIn.Views
{
	using ICSharpCode.Profiler.Controller;
	
	/// <summary>
	/// Description of the view content
	/// </summary>
	public class WpfViewer : AbstractViewContent
	{
		ProfilingDataProvider provider;
		ElementHost host;
		ProfilerView dataView;
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the view
		/// </summary>
		public override Control Control {
			get {
				return this.host;
			}
		}
		
		/// <summary>
		/// Creates a new WpfViewer object
		/// </summary>
		public WpfViewer(ProfilingDataProvider provider, string title)
		{
			this.provider = provider;
			this.TabPageText = title;
			this.TitleName = this.TabPageText;
			this.host = new ElementHost();
			this.host.Child = dataView = new ProfilerView(this.provider);
			this.host.Dock = DockStyle.Fill;
		}

		/// <summary>
		/// Refreshes the view
		/// </summary>
		public override void RedrawContent()
		{
			// TODO: Refresh the whole view control here, renew all resource strings
			//       Note that you do not need to recreate the control.
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			this.dataView.SaveUserState();
			this.host.Dispose();
			this.provider.Close();
			base.Dispose();
		}
	}
	
}
