/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier
 * Datum: 15.05.2007
 * Zeit: 22:17
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;

namespace ICSharpCode.Reports.Core.Exporter.ExportRenderer
{
	/// <summary>
	/// Description of XPSRenderer.
	/// </summary>
	public class XpsRenderer:BaseExportRenderer
	{
		#region Constructor
		
		public static XpsRenderer CreateInstance (PagesCollection pages) {
			if (pages == null) {
				throw new ArgumentNullException("pages");
			}
			XpsRenderer instance = new XpsRenderer(pages);
			return instance;
		}
		
		private XpsRenderer(PagesCollection pages):base(pages)
		{
		}
		
		#endregion
		
		#region overrides
		
		public override void Start()
		{
			base.Start();
		}
		
	
		public override void RenderOutput()
		{
			base.RenderOutput();
		}
		
		
		public override void End()
		{
			base.End();
		}
			
		#endregion
	}
}
