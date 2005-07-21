// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.Core;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class CodeCompletionData : ICompletionData
	{
		IAmbience ambience;
		int      imageIndex;
		int      overloads;
		string   text;
		string   description;
		string   documentation;
		IClass   c;
		bool     convertedDocumentation = false;
		double priority;
		
		/// <summary>
		/// Gets the class this CodeCompletionData object was created for.
		/// Returns null if the CodeCompletionData object was created for a method/property etc.
		/// </summary>
		public IClass Class {
			get {
				return c;
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
				if (!convertedDocumentation && documentation != null) {
					convertedDocumentation = true;
					documentation = GetDocumentation(documentation);
				}
				
				return description + (overloads > 0 ? " " + StringParser.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.CodeCompletionData.OverloadsCounter}", new string[,] {{"NumOverloads", overloads.ToString()}}) : String.Empty) + "\n" + documentation;
			}
			set {
				description = value;
			}
		}
		
		string dotnetName;
		
		void GetPriority(string dotnetName)
		{
			this.dotnetName = dotnetName;
			priority = CodeCompletionDataUsageCache.GetPriority(dotnetName, true);
		}
		
		public CodeCompletionData(string s, int imageIndex)
		{
			ambience = AmbienceService.CurrentAmbience;
			description = documentation = String.Empty;
			text = s;
			this.imageIndex = imageIndex;
			GetPriority(s);
		}
		
		public CodeCompletionData(IClass c)
		{
			ambience = AmbienceService.CurrentAmbience;
			// save class (for the delegate description shortcut)
			this.c = c;
			imageIndex = ClassBrowserIconService.GetIcon(c);
			ambience.ConversionFlags = ConversionFlags.None;
			text = ambience.Convert(c);
			ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedNames | ConversionFlags.ShowReturnType | ConversionFlags.ShowModifiers;
//			Console.WriteLine("Convert : " + c);
			description = ambience.Convert(c);
			documentation = c.Documentation;
			GetPriority(c.DotNetName);
		}
		
		public CodeCompletionData(IMethod method)
		{
			
			ambience = AmbienceService.CurrentAmbience;
			ambience.ConversionFlags |= ConversionFlags.ShowReturnType;
			imageIndex  = ClassBrowserIconService.GetIcon(method);
			text        = method.Name;
			description = ambience.Convert(method);
			documentation = method.Documentation;
			GetPriority(method.DotNetName);
		}
		
		public CodeCompletionData(IField field)
		{
			
			ambience = AmbienceService.CurrentAmbience;
			ambience.ConversionFlags |= ConversionFlags.ShowReturnType;
			imageIndex  = ClassBrowserIconService.GetIcon(field);
			text        = field.Name;
			description = ambience.Convert(field);
			documentation = field.Documentation;
			GetPriority(field.DotNetName);
		}
		
		public CodeCompletionData(IProperty property)
		{
			
			ambience = AmbienceService.CurrentAmbience;
			ambience.ConversionFlags |= ConversionFlags.ShowReturnType;
			imageIndex  = ClassBrowserIconService.GetIcon(property);
			text        = property.Name;
			description = ambience.Convert(property);
			documentation = property.Documentation;
			GetPriority(property.DotNetName);
		}
		
		public CodeCompletionData(IEvent e)
		{
			
			ambience = AmbienceService.CurrentAmbience;
			ambience.ConversionFlags |= ConversionFlags.ShowReturnType;
			imageIndex  = ClassBrowserIconService.GetIcon(e);
			text        = e.Name;
			description = ambience.Convert(e);
			documentation = e.Documentation;
			GetPriority(e.DotNetName);
		}
		
		public bool InsertAction(TextArea textArea, char ch)
		{
			if (dotnetName != null) {
				CodeCompletionDataUsageCache.IncrementUsage(dotnetName);
			}
			if (c != null && text.Length > c.Name.Length) {
				textArea.InsertString(text.Substring(0, c.Name.Length + 1));
				Point start = textArea.Caret.Position;
				Point end;
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
		
		internal static Regex whitespace = new Regex(@"\s+");
		
		/// <summary>
		/// Converts the xml documentation string into a plain text string.
		/// </summary>
		public static string GetDocumentation(string doc)
		{
			System.IO.StringReader reader = new System.IO.StringReader("<docroot>" + doc + "</docroot>");
			XmlTextReader xml   = new XmlTextReader(reader);
			StringBuilder ret   = new StringBuilder();
			////Regex whitespace    = new Regex(@"\s+");
			
			try {
				xml.Read();
				do {
					if (xml.NodeType == XmlNodeType.Element) {
						string elname = xml.Name.ToLower();
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
				Console.WriteLine(ex);
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
		
		#region System.IComparable interface implementation
		public int CompareTo(object obj)
		{
			if (obj == null || !(obj is CodeCompletionData)) {
				return -1;
			}
			return text.CompareTo(((CodeCompletionData)obj).text);
		}
		#endregion
	}
}
