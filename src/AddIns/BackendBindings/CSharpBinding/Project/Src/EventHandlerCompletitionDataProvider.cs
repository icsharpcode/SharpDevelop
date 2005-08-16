// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision: 321 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.Core;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace CSharpBinding
{
	public class EventHandlerCompletitionDataProvider : ICompletionDataProvider
	{
		string expression;
		ResolveResult resolveResult;
		
		public EventHandlerCompletitionDataProvider(string expression, ResolveResult resolveResult)
		{
			this.expression = expression;
			this.resolveResult = resolveResult;
		}
		
		public ImageList ImageList
		{
			get
			{
				return ClassBrowserIconService.ImageList;
			}
		}
		
		public int DefaultIndex
		{
			get
			{
				return -1;
			}
		}
		
		public string PreSelection
		{
			get
			{
				return null;
			}
		}
		
		public ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			ArrayList completionData = new ArrayList();
			completionData.Add(new EventHandlerCompletionData("new " + resolveResult.ResolvedType.FullyQualifiedName + "()", "Event handler"));
			return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
		}
		
		class EventHandlerCompletionData : ICompletionData
		{
			string text;
			string description;
			
			public int ImageIndex
			{
				get
				{
					return ClassBrowserIconService.MethodIndex;
				}
			}
			
			public string Text
			{
				get {
					return text;
				}
				set {
					text = value;
				}
			}
			
			public string Description
			{
				get {
					return description;
				}
			}
			
			public double Priority
			{
				get {
					return 0;
				}
			}
			
			public bool InsertAction(TextArea textArea, char ch)
			{
				textArea.InsertString(text);
				return false;
			}
			
			public EventHandlerCompletionData(string text, string description)
			{
				this.text        = text;
				this.description = description;
			}
			
			#region System.IComparable interface implementation
			public int CompareTo(object obj)
			{
				if (obj == null || !(obj is EventHandlerCompletionData)) {
					return -1;
				}
				return text.CompareTo(((EventHandlerCompletionData)obj).Text);
			}
			#endregion
		}
	}
}
