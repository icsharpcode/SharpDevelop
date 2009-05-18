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
		OpenedFile file;
		
		/// <summary>
		/// Returns the ProfilerView of this instance.
		/// </summary>
		public ProfilerView DataView {
			get { return dataView; }
		}
		
		public override object Control {
			get {
				return dataView;
			}
		}
		
		public override string PrimaryFileName {
			get { return file.FileName; }
		}
		
		/// <summary>
		/// Creates a new WpfViewer object
		/// </summary>
		public WpfViewer(OpenedFile file)
		{
			//this.Files.Add(file);
			this.file = file;
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
