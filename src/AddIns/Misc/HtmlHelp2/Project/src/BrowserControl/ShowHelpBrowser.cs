// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2
{
	// With a big "Thank you" to Robert_G (Delphi-PRAXiS)
	
	using System;
	using System.Security.Permissions;
	using ICSharpCode.SharpDevelop.BrowserDisplayBinding;
	using ICSharpCode.SharpDevelop.Gui;
	using MSHelpServices;

	public static class ShowHelpBrowser
	{
		static bool hiliteMatches;
		static IHxTopic lastTopic;

		public static void OpenHelpView(IHxTopic topic)
		{
			if (topic == null)
			{
				throw new ArgumentNullException("topic");
			}
			OpenHelpView(topic.URL, null, false);
		}

		public static void OpenHelpView(IHxTopic topic, bool hiliteMatchingWords)
		{
			if (topic == null)
			{
				throw new ArgumentNullException("topic");
			}
			OpenHelpView(topic.URL, topic, hiliteMatchingWords);
		}

		public static void OpenHelpView(string topicLink)
		{
			OpenHelpView(topicLink, null, false);
		}

		public static void OpenHelpView(string topicLink, bool hiliteMatchingWords)
		{
			OpenHelpView(topicLink, null, hiliteMatchingWords);
		}

		public static void OpenHelpView(string topicLink, IHxTopic topic, bool hiliteMatchingWords)
		{
			hiliteMatches = hiliteMatchingWords;
			lastTopic = topic;
			BrowserPane help2Browser = ActiveHelp2BrowserView();

			if (help2Browser != null)
			{
				help2Browser.Navigate(topicLink);
				help2Browser.WorkbenchWindow.SelectWindow();
			}
		}

		public static BrowserPane ActiveHelp2BrowserView()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window != null)
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

		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		public static void HighlightDocument(HtmlViewPane htmlViewPane)
		{
			if (htmlViewPane == null)
			{
				throw new ArgumentNullException("htmlViewPane");
			}
			if (hiliteMatches && lastTopic != null)
			{
				lastTopic.HighlightDocument(htmlViewPane.WebBrowser.Document.DomDocument);
			}
		}
	}
}
