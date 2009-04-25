// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using ICSharpCode.Core.Presentation;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop;
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
		ProfilerView dataView;
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the view
		/// </summary>
		public override object Content {
			get {
				return dataView;
			}
		}
		
		/// <summary>
		/// Creates a new WpfViewer object
		/// </summary>
		public WpfViewer(OpenedFile file)
		{
			this.Files.Add(file);
			this.provider = ProfilingDataSQLiteProvider.FromFile(file.FileName);
			this.TabPageText = Path.GetFileName(file.FileName);
			this.TitleName = this.TabPageText;
			dataView = new ProfilerView(this.provider);
		}

		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			this.dataView.SaveUserState();
			this.provider.Close();
			base.Dispose();
		}
	}
}
