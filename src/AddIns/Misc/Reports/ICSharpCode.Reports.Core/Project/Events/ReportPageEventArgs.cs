// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Printing;

using ICSharpCode.Reports.Core.Interfaces;

	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 21.11.2004 14:59:06
	/// </remarks>
namespace ICSharpCode.Reports.Core {	
	
		public class ReportPageEventArgs : System.EventArgs {
		
		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		private PrintPageEventArgs printEventArgs;
		private bool forceNewPage;
		private ISinglePage singlePage;
		private Point locationAfterDraw;
		
		public ReportPageEventArgs(PrintPageEventArgs e,
		                           ISinglePage singlePage,
		                           bool forceNewPage,
		                           Point locationAfterDraw){
			
			if (singlePage == null) {
				throw new ArgumentNullException("singlePage");
			}
			this.printEventArgs = e;
			this.singlePage = singlePage;
			this.forceNewPage = forceNewPage;
			this.locationAfterDraw = locationAfterDraw;
		}
		
		
		public ISinglePage SinglePage {
				get { return singlePage; }
				set { singlePage = value; }
		}
		
		
		public PrintPageEventArgs PrintPageEventArgs {
			get {
				return printEventArgs;
			}
		}
		
		
		public bool ForceNewPage {
			get {
				return forceNewPage;
			}
			set {
				forceNewPage = value;
			}
		}
		
		public Point LocationAfterDraw {
			get {
				return locationAfterDraw;
			}
			set {
				locationAfterDraw = value;
			}
		}
	}
}
