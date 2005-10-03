// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;

namespace HtmlHelp2
{
	public abstract class HelpToolbarCommand : AbstractCommand
	{
		public HtmlHelp2TocPad TocPad
		{
			get
			{
				return (HtmlHelp2TocPad)WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2TocPad)).PadContent;
			}
		}
		
		public WebBrowser Browser
		{
			get
			{
				return ((HtmlViewPane)Owner).WebBrowser;
			}
		}
		
		public void BringTocPadToFront()
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2TocPad)).BringPadToFront();
		}
	}
	
	public class SyncTocCommand : HelpToolbarCommand
	{
		public override void Run()
		{
			TocPad.SyncToc(Browser.Url.ToString());
			BringTocPadToFront();
		}
	}
	
	public class PreviousTopicCommand : HelpToolbarCommand
	{
		public override void Run()
		{
			try
			{
				TocPad.GetPrevFromNode();
			}
			catch
			{
				TocPad.GetPrevFromUrl(Browser.Url.ToString());
			}
			BringTocPadToFront();
		}
	}
	
	public class NextTopicCommand : HelpToolbarCommand
	{
		public override void Run()
		{
			try
			{
				TocPad.GetNextFromNode();
			}
			catch
			{
				TocPad.GetNextFromUrl(Browser.Url.ToString());
			}
			BringTocPadToFront();
		}
	}
}
