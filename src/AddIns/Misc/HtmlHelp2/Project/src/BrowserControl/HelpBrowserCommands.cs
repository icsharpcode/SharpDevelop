// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Security.Permissions;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;
using ICSharpCode.SharpDevelop.Gui;

namespace HtmlHelp2
{
	public abstract class HelpToolbarCommand : AbstractCommand
	{
		public static HtmlHelp2TocPad TocPad
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
		
		public static void BringTocPadToFront()
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2TocPad)).BringPadToFront();
		}
	}
	
	[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name="Execution")]
	public class SyncTocCommand : HelpToolbarCommand
	{
		public override void Run()
		{
			TocPad.SyncToc(Browser.Url.ToString());
			BringTocPadToFront();
		}
	}
	
	[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name="Execution")]
	public class PreviousTopicCommand : HelpToolbarCommand
	{
		public override void Run()
		{
			try
			{
				TocPad.GetPrevFromNode();
			}
			catch (System.ArgumentException)
			{
				TocPad.GetPrevFromUrl(Browser.Url.ToString());
			}
			BringTocPadToFront();
		}
	}
	
	[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name="Execution")]
	public class NextTopicCommand : HelpToolbarCommand
	{
		public override void Run()
		{
			try
			{
				TocPad.GetNextFromNode();
			}
			catch (System.ArgumentException)
			{
				TocPad.GetNextFromUrl(Browser.Url.ToString());
			}
			BringTocPadToFront();
		}
	}
}
