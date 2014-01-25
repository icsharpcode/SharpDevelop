// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
