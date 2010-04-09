/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 15.11.2009
 * Zeit: 20:04
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Drawing;

namespace ICSharpCode.Reports.Core.Interfaces
{
	/// <summary>
	/// Description of ILayouter.
	/// </summary>
	public interface ILayouter
	{
		Rectangle Layout (Graphics graphics,BaseSection section);
		Rectangle Layout (Graphics graphics,ISimpleContainer container);
	}
}