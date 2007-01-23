/*
 * User: dickon
 * Date: 21/11/2006
 * Time: 22:46
 * 
 */

using System;
using System.Data;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;

namespace SharpDbTools.Forms
{
	/// <summary>
	/// Description of SQLEditorQueryToolViewContent.
	/// </summary>
	public class SQLToolViewContent : AbstractViewContent
	{
		string logicalConnectionName;
		SQLTool sqlTool;
		
		public SQLToolViewContent(string logicalConnectionName)
		{
			this.TitleName = "SQL Tool: " + logicalConnectionName;
			this.logicalConnectionName = logicalConnectionName;
			sqlTool = new SQLTool(this.logicalConnectionName);
		}
		
		public override System.Windows.Forms.Control Control {
			get {
				return this.sqlTool;
			}
		}
		
		public override bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public override bool IsViewOnly {
			get {
				return false;
			}
		}
	}
}
