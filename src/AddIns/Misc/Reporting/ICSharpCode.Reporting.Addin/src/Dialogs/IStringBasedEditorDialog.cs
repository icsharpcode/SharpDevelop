/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 11.05.2014
 * Time: 18:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace ICSharpCode.Reporting.Addin.Dialogs
{
	/// <summary>
	/// Description of IStringBasedEditorDialog.
	/// </summary>
	public interface IStringBasedEditorDialog
	{
		DialogResult ShowDialog();
		string TextValue {get;}
	}
}
