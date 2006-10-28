/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier Helmut
 * Datum: 27.10.2006
 * Zeit: 22:43
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;

namespace SharpReportCore.Exporters
{
	/// <summary>
	/// Description of ExportGraphic.
	/// </summary>
	public class ExportGraphic:BaseExportColumn,IPerformLine
	{
		public ExportGraphic():base()
		{
		}
		public ExportGraphic (BaseStyleDecorator itemStyle,bool isContainer):base(itemStyle,isContainer){
			
		}
		
		public override BaseStyleDecorator StyleDecorator {
			get {
				return base.StyleDecorator;
			}
			set {
				base.StyleDecorator = value;
			}
		}
		
	}
}
