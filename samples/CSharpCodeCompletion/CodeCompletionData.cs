/*
 * Erstellt mit SharpDevelop.
 * Benutzer: grunwald
 * Datum: 27.08.2007
 * Zeit: 14:25
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;

namespace CSharpEditor
{
	/// <summary>
	/// Represents an item in the code completion window.
	/// </summary>
	class CodeCompletionData : DefaultCompletionData, ICompletionData
	{
		IMember member;
		IClass c;
		
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
					IDecoration entity = (IDecoration)member ?? c;
					description = GetCSharpText(entity);
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
		/// Returns the declaration of the member as C# code, e.g.
		/// "public void MemberName(string parameter)"
		/// </summary>
		static string GetCSharpText(IDecoration entity)
		{
			if (entity is IMethod)
				return CSharpAmbience.Instance.Convert(entity as IMethod);
			if (entity is IProperty)
				return CSharpAmbience.Instance.Convert(entity as IProperty);
			if (entity is IEvent)
				return CSharpAmbience.Instance.Convert(entity as IEvent);
			if (entity is IField)
				return CSharpAmbience.Instance.Convert(entity as IField);
			if (entity is IClass)
				return CSharpAmbience.Instance.Convert(entity as IClass);
			// unknown entity:
			return entity.ToString();
		}
		
		public static string XmlDocumentationToText(string xmlDoc)
		{
			return xmlDoc;
		}
	}
}
