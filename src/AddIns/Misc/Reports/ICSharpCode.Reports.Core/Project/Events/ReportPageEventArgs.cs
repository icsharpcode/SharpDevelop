// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

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
				throw new ArgumentNullException("pageInfo");
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
