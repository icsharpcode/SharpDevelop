// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Description of SvnMessageView.
	/// </summary>
	public static class SvnMessageView
	{
		static MessageViewCategory category;
		
		public static MessageViewCategory Category {
			get {
				if (category == null) {
					category = new MessageViewCategory("Subversion");
					CompilerMessageView compilerMessageView = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).PadContent;
					compilerMessageView.AddCategory(category);
				}
				return category;
			}
		}
		
		public static void AppendLine(string text)
		{
			Category.AppendLine(text);
		}
		
		public static void HandleNotifications(SvnClientWrapper client)
		{
			client.Notify += delegate(object sender, NotificationEventArgs e) {
				AppendLine(e.Kind + e.Action + " " + e.Path);
			};
		}
	}
}
