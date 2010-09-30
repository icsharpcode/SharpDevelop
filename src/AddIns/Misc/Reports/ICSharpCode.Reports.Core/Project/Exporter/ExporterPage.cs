// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Reports.Core.BaseClasses;

namespace ICSharpCode.Reports.Core.Exporter
{
		
	public class ExporterPage:SinglePage
	{
		private ExporterCollection items;

		#region Constructor
		
		public static ExporterPage CreateInstance (SectionBounds sectionBounds,int pageNumber)
		{
			if (sectionBounds == null) {
				throw new ArgumentNullException("sectionBounds");
			}
			ExporterPage instance = new ExporterPage(sectionBounds,pageNumber);
			return instance;
		}
		
		private ExporterPage (SectionBounds sectionBounds,int pageNumber):base(sectionBounds,pageNumber)
		{	
		}
		
		#endregion
		
		
		public ExporterCollection Items
		{
			get {
				if (this.items == null) {
					items = new ExporterCollection();
				}
				return items;
			}
		}
	}
}
