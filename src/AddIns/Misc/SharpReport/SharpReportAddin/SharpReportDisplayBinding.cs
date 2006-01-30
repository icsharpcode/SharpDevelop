
// one line to give the program's name and an idea of what it does.
// Copyright (C) 2005  peter.forstmeier@t-online.de
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.


using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;

using SharpReportCore;
	/// <summary>
	/// Displaybinding for SharpReport
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 11.04.2005 23:36:45
	/// </remarks>
namespace SharpReportAddin {	
	public class SharpReportDisplayBinding : object, ICSharpCode.Core.IDisplayBinding {
		
		public SharpReportDisplayBinding() {
		}
			public virtual ICSharpCode.SharpDevelop.Gui.IViewContent CreateContentForLanguage(string languageName, string content) {
				SharpReportView view = new SharpReportView();
				if (view != null) {
					try {
						ReportGenerator.CreateReport cmd = new ReportGenerator.CreateReport(view.DesignerControl.ReportModel);
						cmd.Run();
						view.FileName = view.DesignerControl.ReportModel.ReportSettings.FileName;
						view.UpdateView(true);
						view.Selected();
						return view;
					} catch (SharpReportException) {
						if (view != null) {
							view.Dispose();
						}
						return null;
					} catch (Exception) {
						if (view != null) {
							view.Dispose();
						}
						throw;
					}
				}
				return null;
			}
			
		/// <summary>
		/// We allways have to check for an installed printer
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public virtual bool CanCreateContentForLanguage(string languageName) {
			// .addin file already does the language name check
			return GlobalValues.IsValidPrinter();
		}
		
		
		public virtual ICSharpCode.SharpDevelop.Gui.IViewContent CreateContentForFile(string fileName) {
			if (GlobalValues.IsValidPrinter() == true) {
				SharpReportView view = new SharpReportView();
				try {
					StatusBarService.SetMessage (String.Format("File : {0}",fileName));
					view.Load (fileName);
					view.UpdateView (false);
					view.Selected();
					return view;
				} catch (Exception) {
					return new SharpReportView();
				}
			} else {
				return null;
			}
		}
		/// <summary>
		/// We allways have to check for an installed printer
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public virtual bool CanCreateContentForFile(string fileName) {
			// .addin file already does the language name check
			return GlobalValues.IsValidPrinter();
		}
	}
}
