/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 12/22/2006
 * Time: 5:34 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ClassWizard
{
	/// <summary>
	/// Description of AddNewClassCommand
	/// </summary>
	public class AddNewClassCommand : AbstractCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			using (AddClassForm form = new AddClassForm())
			{
				form.Owner = (Form) WorkbenchSingleton.Workbench;
				form.ShowDialog(WorkbenchSingleton.MainForm);
			}
		}
	}
}
