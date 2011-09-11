/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier
 * Datum: 09.04.2007
 * Zeit: 17:01
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Windows.Forms;

namespace ICSharpCode.Reports.Core.Interfaces
{
	/// <summary>
	/// Description of EditorDialog.
	/// </summary>
	/// 
	public interface IStringBasedEditorDialog
	{
		DialogResult ShowDialog();
		string TextValue {get;}
	}
}
