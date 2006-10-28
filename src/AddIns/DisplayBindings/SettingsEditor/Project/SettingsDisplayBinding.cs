/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 10/28/2006
 * Time: 5:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SettingsEditor
{
	public class SettingsDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public IViewContent CreateContentForFile(string fileName)
		{
			SettingsViewContent vc = new SettingsViewContent();
			vc.Load(fileName);
			return vc;
		}
		
		public bool CanCreateContentForLanguage(string languageName)
		{
			return false;
		}
		
		public IViewContent CreateContentForLanguage(string languageName, string content)
		{
			throw new NotImplementedException();
		}
	}
}
