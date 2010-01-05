/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 28.08.2009
 * Zeit: 11:17
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using ICSharpCode.Reports.Core.Exporter;
using System;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of IReportCreator.
	/// </summary>
	public interface IReportCreator
	{
		void BuildExportList ();
		PagesCollection Pages{get;}
		event EventHandler<PageCreatedEventArgs> PageCreated;
		event EventHandler<SectionRenderEventArgs> SectionRendering;
	}
}
