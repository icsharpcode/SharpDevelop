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
	public sealed class SettingsDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new SettingsViewContent(file);
		}
	}
}
