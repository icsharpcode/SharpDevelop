/*
 * Erstellt mit SharpDevelop.
 * Benutzer: grunwald
 * Datum: 27.08.2007
 * Zeit: 14:25
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Dom.VBNet;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace CSharpEditor
{
	/// <summary>
	/// Represents an item in the code completion window.
	/// </summary>
	class CodeCompletionData : DefaultCompletionData, ICompletionData
	{
		IMember member;
		IClass c;
		static VBNetAmbience vbAmbience = new VBNetAmbience();
		static CSharpAmbience csharpAmbience = new CSharpAmbience();
		
		public CodeCompletionData(IMember member)
			: base(member.Name, null, GetMemberImageIndex(member))
		{
			this.member = member;
		}
		
		public CodeCompletionData(IClass c)
			: base(c.Name, null, GetClassImageIndex(c))
		{
			this.c = c;
		}
		
		int overloads = 0;
		
		public void AddOverload()
		{
			overloads++;
		}
		
		static int GetMemberImageIndex(IMember member)
		{
			// Missing: different icons for private/public member
			if (member is IMethod)
				return 1;
			if (member is IProperty)
				return 2;
			if (member is IField)
				return 3;
			if (member is IEvent)
				return 6;
			return 3;
		}
		
		static int GetClassImageIndex(IClass c)
		{
			switch (c.ClassType) {
				case ClassType.Enum:
					return 4;
				default:
					return 0;
			}
		}
		
		string description;
		
		// DefaultCompletionData.Description is not virtual, but we can reimplement
		// the interface to get the same effect as overriding.
		string ICompletionData.Description {
			get {
				if (description == null) {
					IEntity entity = (IEntity)member ?? c;
					description = GetText(entity);
					if (overloads > 1) {
						description += " (+" + overloads + " overloads)";
					}
					description += Environment.NewLine + XmlDocumentationToText(entity.Documentation);
				}
				return description;
			}
		}
		
		/// <summary>
		/// Converts a member to text.
		/// Returns the declaration of the member as C# or VB code, e.g.
		/// "public void MemberName(string parameter)"
		/// </summary>
		static string GetText(IEntity entity)
		{
			IAmbience ambience = MainForm.IsVisualBasic ? (IAmbience)vbAmbience : csharpAmbience;
			if (entity is IMethod)
				return ambience.Convert(entity as IMethod);
			if (entity is IProperty)
				return ambience.Convert(entity as IProperty);
			if (entity is IEvent)
				return ambience.Convert(entity as IEvent);
			if (entity is IField)
				return ambience.Convert(entity as IField);
			if (entity is IClass)
				return ambience.Convert(entity as IClass);
			// unknown entity:
			return entity.ToString();
		}
		
		public static string XmlDocumentationToText(string xmlDoc)
		{
			System.Diagnostics.Debug.WriteLine(xmlDoc);
			StringBuilder b = new StringBuilder();
			try {
				using (XmlTextReader reader = new XmlTextReader(new StringReader("<root>" + xmlDoc + "</root>"))) {
					reader.XmlResolver = null;
					while (reader.Read()) {
						switch (reader.NodeType) {
							case XmlNodeType.Text:
								b.Append(reader.Value);
								break;
							case XmlNodeType.Element:
								switch (reader.Name) {
									case "filterpriority":
										reader.Skip();
										break;
									case "returns":
										b.AppendLine();
										b.Append("Returns: ");
										break;
									case "param":
										b.AppendLine();
										b.Append(reader.GetAttribute("name") + ": ");
										break;
									case "remarks":
										b.AppendLine();
										b.Append("Remarks: ");
										break;
									case "see":
										if (reader.IsEmptyElement) {
											b.Append(reader.GetAttribute("cref"));
										} else {
											reader.MoveToContent();
											if (reader.HasValue) {
												b.Append(reader.Value);
											} else {
												b.Append(reader.GetAttribute("cref"));
											}
										}
										break;
								}
								break;
						}
					}
				}
				return b.ToString();
			} catch (XmlException) {
				return xmlDoc;
			}
		}
	}
}
