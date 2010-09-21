// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Reports.Core.Events
{
	
	public class NewPageEventArgs : System.EventArgs {
		
		private ExporterCollection itemsList;
		
		public NewPageEventArgs(ExporterCollection itemsList)
		{
			this.itemsList = itemsList;
		}
		
		public ExporterCollection ItemsList {
			get { return itemsList; }
		}
	}
}
