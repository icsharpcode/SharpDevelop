// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
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
		string   completionString;
		IClass   c;
		bool     convertedDocumentation = false;
		
		
		public int Overloads {
			get {
				return overloads;
			}
			set {
				overloads = value;
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
		
		public string[] Text {
			get {
				return new string[] { text };
			}
			set {
				text = value[0];
			}
		}
		
		public string Description {
			get {
				// get correct delegate description (when description is requested)
				// in the classproxies aren't methods saved, therefore delegate methods
				// must be get through the real class instead out of the proxy
				//
				// Mike
//		TODO: Still useful ? 		
//				if (c is ClassProxy && c.ClassType == ClassType.Delegate) {
//					description = ambience.Convert(ParserService.GetClass(c.FullyQualifiedName));
//					c = null;
//				}
				
				// don't give a description string, if no documentation or description is provided
				if (description.Length + documentation.Length == 0) {
					return null;
				}
				if (!convertedDocumentation) {
					convertedDocumentation = true;
					try {
						documentation = GetDocumentation(documentation);
						// new (by G.B.)
						// XmlDocument doc = new XmlDocument();
						// doc.LoadXml("<doc>" + documentation + "</doc>");
						// XmlNode root      = doc.DocumentElement;
						// XmlNode paramDocu = root.SelectSingleNode("summary");
						// documentation = paramDocu.InnerXml;
					} catch (Exception e) {
						Console.WriteLine(e.ToString());
					}
				}
				
				return description + (overloads > 0 ? " " + StringParser.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.CodeCompletionData.OverloadsCounter}", new string[,] {{"NumOverloads", overloads.ToString()}}) : String.Empty) + "\n" + documentation;
			}
			set {
				description = value;
			}
		}
		
		public CodeCompletionData(string s, int imageIndex)
		{
			
			ambience = AmbienceService.CurrentAmbience;
			description = documentation = String.Empty;
			text = s;
			completionString = s;
			this.imageIndex = imageIndex;
		}
		
		public CodeCompletionData(IClass c)
		{
			
			ambience = AmbienceService.CurrentAmbience;
			// save class (for the delegate description shortcut
			this.c = c;
			imageIndex = ClassBrowserIconService.GetIcon(c);
			text = c.Name;
			completionString = c.Name;
			ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedNames | ConversionFlags.ShowReturnType | ConversionFlags.ShowModifiers;
//			Console.WriteLine("Convert : " + c);
			description = ambience.Convert(c);
			documentation = c.Documentation;
		}
		
		public CodeCompletionData(IMethod method)
		{
			
			ambience = AmbienceService.CurrentAmbience;
			ambience.ConversionFlags |= ConversionFlags.ShowReturnType;
			imageIndex  = ClassBrowserIconService.GetIcon(method);
			text        = method.Name;
			description = ambience.Convert(method);
			completionString = method.Name;
			documentation = method.Documentation;
		}
		
		public CodeCompletionData(IField field)
		{
			
			ambience = AmbienceService.CurrentAmbience;
			ambience.ConversionFlags |= ConversionFlags.ShowReturnType;
			imageIndex  = ClassBrowserIconService.GetIcon(field);
			text        = field.Name;
			description = ambience.Convert(field);
			completionString = field.Name;
			documentation = field.Documentation;
		}
		
		public CodeCompletionData(IProperty property)
		{
			
			ambience = AmbienceService.CurrentAmbience;
			ambience.ConversionFlags |= ConversionFlags.ShowReturnType;
			imageIndex  = ClassBrowserIconService.GetIcon(property);
			text        = property.Name;
			description = ambience.Convert(property);
			completionString = property.Name;
			documentation = property.Documentation;
		}
		
		public CodeCompletionData(IEvent e)
		{
			
			ambience = AmbienceService.CurrentAmbience;
			ambience.ConversionFlags |= ConversionFlags.ShowReturnType;
			imageIndex  = ClassBrowserIconService.GetIcon(e);
			text        = e.Name;
			description = ambience.Convert(e);
			completionString = e.Name;
			documentation = e.Documentation;
		}
		
		public void InsertAction(TextEditorControl control)
		{
			((SharpDevelopTextAreaControl)control).ActiveTextAreaControl.TextArea.InsertString(completionString);
		}
		
		internal static Regex whitespace = new Regex(@"\s+");
		public static string GetDocumentation(string doc)
		{
			System.IO.StringReader reader = new System.IO.StringReader("<docroot>" + doc + "</docroot>");
			XmlTextReader xml   = new XmlTextReader(reader);
			StringBuilder ret   = new StringBuilder();
			////Regex whitespace    = new Regex(@"\s+");
			
			try {
				xml.Read();
				bool appendText = true;
				bool inPara = false;
				do {
					if (xml.NodeType == XmlNodeType.Element) {
						string elname = xml.Name.ToLower();
						if (elname == "remarks") {
							ret.Append(Environment.NewLine);
							ret.Append("Remarks:");
							ret.Append(Environment.NewLine);
						} else if (elname == "example") {
							ret.Append(Environment.NewLine);
							ret.Append("Example:");
							ret.Append(Environment.NewLine);
						} else if (elname == "exception") {
							ret.Append(Environment.NewLine);
							ret.Append("Exception: ");
							ret.Append(Environment.NewLine);
							ret.Append(GetCref(xml.Value));
						} else if (elname == "returns") {
							ret.Append(GetCref(xml["cref"]));
							ret.Append(xml["langword"]);
						} else if (elname == "see") {
							ret.Append(GetCref(xml["cref"]));
							ret.Append(xml["langword"]);
						} else if (elname == "seealso") {
							ret.Append("See also: ");
							ret.Append(GetCref(xml["cref"]));
						} else if (elname == "paramref") {
							if (!inPara) ret.Append(Environment.NewLine);
							ret.Append(xml["name"]);
							if (!inPara) ret.Append(": ");
						} else if (elname == "param") {
							if (!inPara) {
								ret.Append(Environment.NewLine);
							}
							ret.Append(whitespace.Replace(xml["name"].Trim()," "));
							if (!inPara) {
								ret.Append(": ");
							}
						} else if (elname == "value") {
							////appendText = false;
							ret.Append(Environment.NewLine);
							ret.Append("Value: ");
						} else if (elname == "para") {
							inPara = true;
						}
					} else if (xml.NodeType == XmlNodeType.EndElement) {
						string elname = xml.Name.ToLower();
//						if (elname == "para" || elname == "param") {
//							ret.Append(Environment.NewLine);
//						}
						if (elname == "para") {
							inPara = false;
						}
					} else if (xml.NodeType == XmlNodeType.Text) {
						if (appendText) {
							ret.Append(whitespace.Replace(xml.Value, " "));
						}
						appendText = true;
					}
				} while(xml.Read());
			} catch {
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
