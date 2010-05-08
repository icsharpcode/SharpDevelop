// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;

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
