// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	[Obsolete]
	public class CodeCompletionData : ICompletionData
	{
		IEntity entity;
		int      imageIndex;
		int      overloads;
		string   text;
		string description;
		string documentation;
		double priority;
		
		/// <summary>
		/// Gets the class this CodeCompletionData object was created for.
		/// Returns null if the CodeCompletionData object was created for a method/property etc.
		/// </summary>
		public IClass Class {
			get {
				return entity as IClass;
			}
		}
		
		/// <summary>
		/// Gets the member this CodeCompletionData object was created for.
		/// Returns null if the CodeCompletionData object was created for a class or namespace.
		/// </summary>
		public IMember Member {
			get {
				return entity as IMember;
			}
		}
		
		/// <summary>
		/// Gets the class or member this CodeCompletionData object was created for.
		/// </summary>
		public IEntity Entity {
			get {
				return entity;
			}
		}
		
		public int Overloads {
			get {
				return overloads;
			}
			set {
				overloads = value;
			}
		}
		
		public double Priority {
			get {
				return priority;
			}
			set {
				priority = value;
			}
		}
		
		public int ImageIndex {
			get {
				return imageIndex;
			}
			set {
				imageIndex = value;
			}
		}
		
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		
		public string Description {
			get {
				// don't give a description string, if no documentation or description is provided
				if (description.Length == 0 && (documentation == null || documentation.Length == 0)) {
					return "";
				}
				if (documentation == null) {
					if (entity != null)
						documentation = ConvertDocumentation(entity.Documentation);
					else
						documentation = "";
				}
				
				return description + (overloads > 0 ? " " + StringParser.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.CodeCompletionData.OverloadsCounter}", new string[,] {{"NumOverloads", overloads.ToString()}}) : String.Empty) + "\n" + documentation;
			}
			set {
				description = value;
			}
		}
		
		string dotnetName;
		
		void InitializePriority(string dotnetName)
		{
			this.dotnetName = dotnetName;
			priority = CodeCompletionDataUsageCache.GetPriority(dotnetName, true);
		}
		
		public CodeCompletionData(string s, int imageIndex)
		{
			description = documentation = String.Empty;
			text = s;
			this.imageIndex = imageIndex;
			InitializePriority(s);
		}
		
		public CodeCompletionData(IClass c)
		{
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			// save class (for the delegate description shortcut)
			this.entity = c;
			imageIndex = ClassBrowserIconService.GetIcon(c).ImageIndex;
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			text = ambience.Convert(c);
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.UseFullyQualifiedMemberNames;
			description = ambience.Convert(c);
			InitializePriority(c.DotNetName);
		}
		
		public CodeCompletionData(IMember member)
		{
			this.entity = member;
			imageIndex  = ClassBrowserIconService.GetIcon(member).ImageIndex;
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.None;
			text = ambience.Convert(member);
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
			description = ambience.Convert(member);
			InitializePriority(member.DotNetName);
		}
		
		public CodeCompletionData(IMethod method) : this((IMember)method)
		{
		}
		
		public CodeCompletionData(IField field) : this((IMember)field)
		{
		}
		
		public CodeCompletionData(IProperty property) : this((IMember)property)
		{
		}
		
		public CodeCompletionData(IEvent e) : this((IMember)e)
		{
		}
		
		public bool InsertAction(TextArea textArea, char ch)
		{
			if (dotnetName != null) {
				CodeCompletionDataUsageCache.IncrementUsage(dotnetName);
			}
			IClass c = this.Class;
			if (c != null && text.Length > c.Name.Length) {
				textArea.InsertString(text.Substring(0, c.Name.Length + 1));
				TextLocation start = textArea.Caret.Position;
				TextLocation end;
				int pos = text.IndexOf(',');
				if (pos < 0) {
					textArea.InsertString(text.Substring(c.Name.Length + 1));
					end = textArea.Caret.Position;
					end.X -= 1;
				} else {
					textArea.InsertString(text.Substring(c.Name.Length + 1, pos - c.Name.Length - 1));
					end = textArea.Caret.Position;
					textArea.InsertString(text.Substring(pos));
				}
				textArea.Caret.Position = start;
				textArea.SelectionManager.SetSelection(start, end);
				if (!char.IsLetterOrDigit(ch)) {
					return true;
				}
			} else {
				textArea.InsertString(text);
			}
			return false;
		}
		
		
		/// <summary>
		/// Converts the xml documentation string into a plain text string.
		/// </summary>
		public static string ConvertDocumentation(string doc)
		{
			return CodeCompletionItem.ConvertDocumentation(doc);
		}
	}
}
