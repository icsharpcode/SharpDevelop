// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XamlBinding
{
	class XamlCodeCompletionItem : CodeCompletionItem
	{
		public XamlCodeCompletionItem(IEntity entity, string prefix, bool addOpeningBrace)
			: base(entity)
		{
			if (string.IsNullOrEmpty(prefix))
				this.Text = entity.Name;
			else
				this.Text = prefix + ":" + entity.Name;
			
			if (addOpeningBrace)
				this.Text = "<" + this.Text;
		}
		
		public XamlCodeCompletionItem(IEntity entity)
			: base(entity)
		{
			this.Text = entity.Name;
		}
		
		public XamlCodeCompletionItem(IEntity entity, string text)
			: base(entity)
		{
			this.Text = text;
		}
		
		public override string ToString()
		{
			return "[" + this.Text + "]";
		}
	}
	
	class XamlCompletionItem : DefaultCompletionItem
	{
		string prefix, @namespace, name;
		
		public XamlCompletionItem(string prefix, string @namespace, string name)
			: base(prefix + ":" + name)
		{
			this.prefix = prefix;
			this.@namespace = @namespace;
			this.name = name;
		}
		
		public XamlCompletionItem(string @namespace, string name)
			: base(name)
		{
			this.prefix = "";
			this.@namespace = @namespace;
			this.name = name;
		}
		
		public string Prefix {
			get { return prefix; }
		}
		
		public string Namespace {
			get { return @namespace; }
		}
		
		public string Name {
			get { return name; }
		}
	}
	
	class XmlnsCompletionItem : DefaultCompletionItem
	{
		string @namespace, assembly;
		bool isUrl;
		
		public bool IsUrl {
			get { return isUrl; }
		}
		
		public XmlnsCompletionItem(string @namespace, string assembly)
			: base(@namespace + " (" + assembly + ")")
		{
			this.@namespace = @namespace;
			this.assembly = assembly;
		}
		
		public XmlnsCompletionItem(string @namespace, bool isUrl)
			: base(@namespace)
		{
			this.@namespace = @namespace;
			this.isUrl = isUrl;
			this.assembly = string.Empty;
		}
		
		public string Namespace {
			get { return @namespace; }
		}
		
		public string Assembly {
			get { return assembly; }
		}
		
		public override void Complete(CompletionContext context)
		{
			if (isUrl)
				base.Complete(context);
			else {
				ITextEditor editor = context.Editor;
				string newText = "clr-namespace:" + @namespace;
				if (!string.IsNullOrEmpty(assembly))
					newText += ";assembly=" + assembly;
				editor.Document.Replace(context.StartOffset, context.Length, newText);
				context.EndOffset = context.StartOffset + newText.Length;
			}
		}
	}
	
	class NewEventCompletionItem : DefaultCompletionItem
	{
		IEvent eventType;
		
		public IEvent EventType {
			get { return eventType; }
		}
		
		string targetName;
		
		public string TargetName {
			get { return targetName; }
		}
		
		public string HandlerName { get; private set; }
		
		public NewEventCompletionItem(IEvent eventType, string targetName)
			: base(StringParser.Parse("${res:AddIns.XamlBinding.NewEventHandlerItem}"))
		{
			this.eventType = eventType;
			this.targetName = targetName;
			this.HandlerName = ParseNamePattern(this.TargetName, this.EventType.Name);
		}
		
		public override void Complete(CompletionContext context)
		{
			context.Editor.Document.Replace(context.StartOffset, context.Length, this.HandlerName);
			
			context.EndOffset = context.StartOffset + this.HandlerName.Length;
		}
		
		public static string ParseNamePattern(string objectName, string eventName)
		{
			string name = XamlBindingOptions.EventHandlerNamePattern;
			
			foreach (Match match in Regex.Matches(name, "%[A-z0-9]*%")) {
				switch (match.Value.ToLowerInvariant()) {
					case "%object%":
						if (char.IsUpper(match.Value[1]))
							objectName = objectName.ToUpper()[0] + objectName.Substring(1, objectName.Length - 1);
						else
							objectName = objectName.ToLower()[0] + objectName.Substring(1, objectName.Length - 1);
						name = name.Replace(match.Index, match.Length, objectName);
						break;
					case "%event%":
						if (char.IsUpper(match.Value[1]))
							eventName = eventName.ToUpper()[0] + eventName.Substring(1, eventName.Length - 1);
						else
							eventName = eventName.ToLower()[0] + eventName.Substring(1, eventName.Length - 1);
						name = name.Replace(match.Index, match.Length, eventName);
						break;
					case "%%":
						name = name.Replace(match.Index, match.Length, "%");
						break;
					default:
						throw new ArgumentException("Pattern identifier invalid", match.Value);
				}
			}
			
			return name;
		}
	}
	
	class MarkupExtensionInsightItem : IInsightItem
	{
		IMethod ctor;
		
		public MarkupExtensionInsightItem(IMethod entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			this.ctor = entity;
		}
		
		public IMethod Ctor {
			get { return ctor; }
		}
		
		string headerText;
		bool descriptionCreated;
		string description;
		
		public object Header {
			get {
				if (headerText == null) {
					IAmbience ambience = AmbienceService.GetCurrentAmbience();
					ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
					headerText = ambience.Convert(ctor);
					headerText = headerText.Insert(headerText.LastIndexOf(')'), (ctor.Parameters.Count > 0 ? ", " : "") + "Named Parameters ...");
				}
				return headerText;
			}
		}
		
		public object Content {
			get {
				if (!descriptionCreated) {
					string entityDoc = ctor.Documentation;
					if (!string.IsNullOrEmpty(entityDoc)) {
						description = ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.CodeCompletionData.ConvertDocumentation(entityDoc);
					}
					descriptionCreated = true;
				}
				return description;
			}
		}
	}
	
	class MemberInsightItem : IInsightItem
	{
		string insightText;
		string headerText;
		string description;
		bool descriptionCreated;
		IMember member;
		
		public IMember Member {
			get { return member; }
		}
		
		public MemberInsightItem(IMember member, string insightText)
		{
			this.member = member;
			this.insightText = insightText;
		}
		
		public object Header {
			get {
				if (headerText == null) {
					headerText = this.member.Name + "=\"" + insightText + "\"";
				}
				
				return headerText;
			}
		}
		
		public object Content {
			get {
				if (!descriptionCreated) {
					string entityDoc = member.Documentation;
					if (!string.IsNullOrEmpty(entityDoc)) {
						description = ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.CodeCompletionData.ConvertDocumentation(entityDoc);
					}
					descriptionCreated = true;
				}
				return description;
			}
		}
	}
}
