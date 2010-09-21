// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
