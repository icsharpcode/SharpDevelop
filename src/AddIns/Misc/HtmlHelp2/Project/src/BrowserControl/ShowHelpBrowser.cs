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
	using ICSharpCode.SharpDevelop.BrowserDisplayBinding;
	using MSHelpServices;

	public static class ShowHelpBrowser
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

		public static void OpenHelpView(string topicUrl, bool hiliteMatchingWords)
		{
			OpenHelpView(topicUrl, null, hiliteMatchingWords);
		}

		public static void OpenHelpView(string topicUrl, IHxTopic topic, bool hiliteMatchingWords)
		{
			hiliteMatches = hiliteMatchingWords;
			lastTopic = topic;
			BrowserPane help2Browser = GetActiveHelp2BrowserView();

			if(help2Browser != null)
			{
				help2Browser.Load(topicUrl);
				help2Browser.WorkbenchWindow.SelectWindow();
			}
		}

		public static BrowserPane GetActiveHelp2BrowserView()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if(window != null)
			{
				BrowserPane browserPane = window.ActiveViewContent as BrowserPane;
				if (browserPane != null && browserPane.Url.Scheme == "ms-help")
					return browserPane;
			}

			foreach(IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection)
			{
				BrowserPane browserPane = view as BrowserPane;
				if (browserPane != null && browserPane.Url.Scheme == "ms-help")
					return browserPane;
			}
			return CreateNewHelp2BrowserView();
		}
		
		public static BrowserPane CreateNewHelp2BrowserView()
		{
			BrowserPane tempPane = new BrowserPane();
			WorkbenchSingleton.Workbench.ShowView(tempPane);
			return tempPane;
		}
		
		public static void HighlightDocument(HtmlViewPane htmlViewPane)
		{
			if (hiliteMatches && lastTopic != null)
			{
				lastTopic.HighlightDocument(htmlViewPane.WebBrowser.Document.DomDocument);
			}
		}
	}
}
