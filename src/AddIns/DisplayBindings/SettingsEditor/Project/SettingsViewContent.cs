/*
 * Created by SharpDevelop.
 * User: tfssetup
 * Date: 10/28/2006
 * Time: 5:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System.Windows.Forms;

namespace ICSharpCode.SettingsEditor
{
	public class SettingsViewContent : AbstractViewContent
	{
		SettingsView view = new SettingsView();
		
		public override Control Control {
			get {
				return view;
			}
		}
		
		public override void Load(string fileName)
		{
		}
		
		public override void Save()
		{
		}
	}
}
