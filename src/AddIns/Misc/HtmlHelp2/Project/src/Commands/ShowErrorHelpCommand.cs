// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3555 $</version>
// </file>
using MSHelpServices;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace HtmlHelp2.Commands
{
	/// <summary>
	/// Description of ShowErrorHelpCommand
	/// </summary>
	public class ShowErrorHelpCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			ICSharpCode.SharpDevelop.Gui.TaskView view = (ICSharpCode.SharpDevelop.Gui.TaskView)Owner;
			
			// Search all selected tasks
			foreach (Task t in new List<Task>(view.SelectedTasks))
			{
				string code = t.BuildError.ErrorCode;
				
				if (string.IsNullOrEmpty(code))
					return;
				
				// Get help content
				MSHelpServices.IHxTopic topic;
				
				// If HtmlHelp2 AddIn is initialised correctly we can start!
				if (HtmlHelp2.Environment.HtmlHelp2Environment.SessionIsInitialized)
				{
					// Get the topic
					IHxIndex index = HtmlHelp2.Environment.HtmlHelp2Environment.GetIndex("");
					if (index == null) {
						MessageService.ShowErrorFormatted("No help available for {0}!", code);
						return;
					}
					int indexSlot = index.GetSlotFromString(code);
					IHxTopicList list = index.GetTopicsFromSlot(indexSlot);
					if (list == null) {
						MessageService.ShowErrorFormatted("No help available for {0}!", code);
						return;
					}
					topic = list.ItemAt(1);
					if (topic == null) {
						MessageService.ShowErrorFormatted("No help available for {0}!", code);
						return;
					}
					string topicTitle = topic.get_Title(HxTopicGetTitleType.HxTopicGetTOCTitle, HxTopicGetTitleDefVal.HxTopicGetTitleFileName);
					if (topicTitle == null || !topicTitle.Contains(code)) {
						MessageService.ShowErrorFormatted("No help available for {0}!", code);
						return;
					}
				}
				else // Otherwise we have to show an Error message ...
				{
					LoggingService.Error("Couldn't initialize help database");
					return;
				}

				// Show Browser window
				HtmlHelp2.ShowHelpBrowser.OpenHelpView(topic);
			}
		}
	}
}
