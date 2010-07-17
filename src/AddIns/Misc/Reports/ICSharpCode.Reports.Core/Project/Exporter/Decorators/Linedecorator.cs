/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 27.09.2009
 * Zeit: 12:02
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace ICSharpCode.Reports.Core.Exporter
{
	public class LineDecorator : GraphicStyleDecorator, ILineDecorator
	{
		public LineDecorator(BaseShape shape) : base(shape)
		{

		}

		public Point From { get; set; }
		public Point To { get; set; }
	}
}


