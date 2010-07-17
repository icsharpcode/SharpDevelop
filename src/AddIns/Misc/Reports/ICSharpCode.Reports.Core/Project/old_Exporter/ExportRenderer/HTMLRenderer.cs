/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier
 * Datum: 15.05.2007
 * Zeit: 22:08
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;

namespace ICSharpCode.Reports.Core.Exporter.ExportRenderer
{
	/// <summary>
	/// Description of HTMLRenderer.
	/// </summary>
	public class HtmlRenderer:BaseExportRenderer
	{
		#region Constructor
		
		
		public static HtmlRenderer CreateInstance (PagesCollection pages) {
			if (pages == null) {
				throw new ArgumentNullException("pages");
			}
		
			HtmlRenderer instance = new HtmlRenderer(pages);
			return instance;
		}
		
		private HtmlRenderer(PagesCollection pages):base(pages)
		{
		}
		
		#endregion
		
		#region overrides
		/// <summary>
		/// Setup
		/// </summary>
		public override void Start()
		{
			base.Start();
		}
		
		/// <summary>
		/// Loop over all pages
		///  <see cref="BaseExportColumn"></see> how to Draw the items
		/// </summary>
		public override void RenderOutput()
		{
			base.RenderOutput();
		}
		
		/// <summary>
		/// Cleanup (Save Document etc)
		/// </summary>
		public override void End()
		{
			base.End();
		}
		
		#endregion
	}
}
