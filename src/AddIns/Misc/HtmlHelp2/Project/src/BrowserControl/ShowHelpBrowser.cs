/* ***********************************************************
 * 
 * Help 2.0 Environment for SharpDevelop
 * ShowHelpBrowser Class
 * Copyright (c) 2005, Mathias Simmack. All rights reserved.
 * 
 * With a big "Thank you" to Robert_G (Delphi-PRAXiS)
 * 
 * ********************************************************* */
namespace HtmlHelp2
{
	using System;
	using ICSharpCode.SharpDevelop.Gui;
	using MSHelpServices;
	using HtmlHelp2Browser;


	public sealed class ShowHelpBrowser
	{
		static bool hiliteMatches = false;
		static IHxTopic lastTopic = null;

		public static void OpenHelpView(IHxTopic topic)
		{
			OpenHelpView(topic.URL, null, false);
		}

		public static void OpenHelpView(IHxTopic topic, bool hiliteMatchingWords)
		{
			OpenHelpView(topic.URL, topic, hiliteMatchingWords);
		}

		public static void OpenHelpView(string topicUrl)
		{
			OpenHelpView(topicUrl, null, false);
		}

		public static void OpenHelpView(string topicUrl, IHxTopic topic, bool hiliteMatchingWords)
		{
			hiliteMatches = hiliteMatchingWords;
			lastTopic = topic;
			HtmlHelp2BrowserPane help2Browser = GetActiveHelp2BrowserView();

			if(help2Browser != null) {
				help2Browser.Load(topicUrl);
				help2Browser.WorkbenchWindow.SelectWindow();
			}
		}

		public static HtmlHelp2BrowserPane GetActiveHelp2BrowserView()
		{
			HtmlHelp2BrowserPane tempPane = null;

			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if(window != null && window.ActiveViewContent is HtmlHelp2BrowserPane) {
				tempPane = (HtmlHelp2BrowserPane)window.ActiveViewContent;
				return tempPane;
			}

			foreach(IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if(view is HtmlHelp2BrowserPane) {
					tempPane = (HtmlHelp2BrowserPane)view;
					return tempPane;
				}
			}

			tempPane = CreateNewHelp2BrowserView();
			return tempPane;
		}

		public static HtmlHelp2BrowserPane CreateNewHelp2BrowserView()
		{
			HtmlHelp2BrowserPane tempPane = new HtmlHelp2BrowserPane("");
			WorkbenchSingleton.Workbench.ShowView(tempPane);
			return tempPane;
		}

		public static void HighlightDocument()
		{
			if(hiliteMatches && lastTopic != null) {
				try {
					IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
					if(window != null && window.ActiveViewContent is HtmlHelp2BrowserPane) {
						HtmlHelp2BrowserPane help2Browser = (HtmlHelp2BrowserPane)window.ActiveViewContent;
						HtmlHelp2BrowserControl browserControl = (HtmlHelp2BrowserControl)help2Browser.Control;
						lastTopic.HighlightDocument(browserControl.AxWebBrowser.Document);
					}
				}
				catch {
				}
			}
		}

		ShowHelpBrowser()
		{
		}
	}
}
