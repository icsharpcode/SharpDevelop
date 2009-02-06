// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
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
			imageIndex = ClassBrowserIconService.GetIcon(c);
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			text = ambience.Convert(c);
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.UseFullyQualifiedMemberNames;
			description = ambience.Convert(c);
			InitializePriority(c.DotNetName);
		}
		
		public CodeCompletionData(IMember member)
		{
			this.entity = member;
			imageIndex  = ClassBrowserIconService.GetIcon(member);
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
		
		[Obsolete("Use 'ConvertDocumentation' instead.")]
		public static string GetDocumentation(string doc)
		{
			return ConvertDocumentation(doc);
		}
		
		static readonly Regex whitespace = new Regex(@"\s+");
		
		/// <summary>
		/// Converts the xml documentation string into a plain text string.
		/// </summary>
		public static string ConvertDocumentation(string doc)
		{
			if (string.IsNullOrEmpty(doc))
				return string.Empty;
			
			System.IO.StringReader reader = new System.IO.StringReader("<docroot>" + doc + "</docroot>");
			XmlTextReader xml   = new XmlTextReader(reader);
			StringBuilder ret   = new StringBuilder();
			////Regex whitespace    = new Regex(@"\s+");
			
			try {
				xml.Read();
				do {
					if (xml.NodeType == XmlNodeType.Element) {
						string elname = xml.Name.ToLowerInvariant();
						switch (elname) {
							case "filterpriority":
								xml.Skip();
								break;
							case "remarks":
								ret.Append(Environment.NewLine);
								ret.Append("Remarks:");
								ret.Append(Environment.NewLine);
								break;
							case "example":
								ret.Append(Environment.NewLine);
								ret.Append("Example:");
								ret.Append(Environment.NewLine);
								break;
							case "exception":
								ret.Append(Environment.NewLine);
								ret.Append(GetCref(xml["cref"]));
								ret.Append(": ");
								break;
							case "returns":
								ret.Append(Environment.NewLine);
								ret.Append("Returns: ");
								break;
							case "see":
								ret.Append(GetCref(xml["cref"]));
								ret.Append(xml["langword"]);
								break;
							case "seealso":
								ret.Append(Environment.NewLine);
								ret.Append("See also: ");
								ret.Append(GetCref(xml["cref"]));
								break;
							case "paramref":
								ret.Append(xml["name"]);
								break;
							case "param":
								ret.Append(Environment.NewLine);
								ret.Append(whitespace.Replace(xml["name"].Trim()," "));
								ret.Append(": ");
								break;
							case "value":
								ret.Append(Environment.NewLine);
								ret.Append("Value: ");
								ret.Append(Environment.NewLine);
								break;
							case "br":
							case "para":
								ret.Append(Environment.NewLine);
								break;
						}
					} else if (xml.NodeType == XmlNodeType.Text) {
						ret.Append(whitespace.Replace(xml.Value, " "));
					}
				} while(xml.Read());
			} catch (Exception ex) {
				LoggingService.Debug("Invalid XML documentation: " + ex.Message);
				return doc;
			}
			return ret.ToString();
		}
		
		static string GetCref(string cref)
		{
			if (cref == null || cref.Trim().Length==0) {
				return "";
			}
			if (cref.Length < 2) {
				return cref;
			}
			if (cref.Substring(1, 1) == ":") {
				return cref.Substring(2, cref.Length - 2);
			}
			return cref;
		}
	}
}
