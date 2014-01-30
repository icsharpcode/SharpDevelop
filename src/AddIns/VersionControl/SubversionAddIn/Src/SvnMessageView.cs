// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Output pad category for subversion.
	/// </summary>
	public static class SvnMessageView
	{
		static MessageViewCategory category;
		
		/// <summary>
		/// Gets the subversion message view category.
		/// </summary>
		public static MessageViewCategory Category {
			get {
				if (category == null) {
					MessageViewCategory.Create(ref category, "Subversion");
				}
				return category;
			}
		}
		
		/// <summary>
		/// Appends a line to the svn message view.
		/// </summary>
		public static void AppendLine(string text)
		{
			Category.AppendLine(text);
		}
		
		public static void HandleNotifications(SvnClientWrapper client)
		{
			client.Notify += delegate(object sender, NotificationEventArgs e) {
				AppendLine(e.Kind + e.Action + " " + e.Path);
			};
			AsynchronousWaitDialog waitDialog = null;
			client.OperationStarted += delegate(object sender, SubversionOperationEventArgs e) {
				if (waitDialog == null) {
					waitDialog = AsynchronousWaitDialog.ShowWaitDialog("svn " + e.Operation);
//					waitDialog.Cancelled += delegate {
//						client.Cancel();
//					};
				}
			};
			client.OperationFinished += delegate {
				if (waitDialog != null) {
					waitDialog.Dispose();
					waitDialog = null;
				}
			};
		}
	}
}
