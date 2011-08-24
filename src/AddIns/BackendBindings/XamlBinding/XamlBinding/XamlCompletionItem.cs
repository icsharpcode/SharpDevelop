// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XamlBinding
{
	class XamlCodeCompletionItem : CodeCompletionItem
	{
		public XamlCodeCompletionItem(IEntity entity, string prefix)
			: base(entity)
		{
			if (string.IsNullOrEmpty(prefix))
				this.Text = entity.Name;
			else
				this.Text = prefix + ":" + entity.Name;
			this.Content = this.Text;
		}
		
		public XamlCodeCompletionItem(IEntity entity)
			: base(entity)
		{
			this.Text = entity.Name;
			this.Content = this.Text;
		}
		
		public XamlCodeCompletionItem(string text, IEntity entity)
			: base(entity)
		{
			this.Text = text;
			this.Content = this.Text;
		}
		
		public XamlCodeCompletionItem(IEntity entity, string prefix, string className)
			: base(entity)
		{
			if (string.IsNullOrEmpty(prefix))
				this.Text = className + "." + entity.Name;
			else
				this.Text = prefix + ":" + className + "." + entity.Name;
			this.Content = this.Text;
		}
		
		public override string ToString()
		{
			return "[" + this.Text + "]";
		}
	}
	
	class XamlLazyValueCompletionItem : XamlCodeCompletionItem
	{
		bool addType;
		
		public XamlLazyValueCompletionItem(IEntity entity, string text, bool addType)
			: base(entity)
		{
			this.addType = addType;
			this.Text = text;
			this.Content = this.Text;
		}
		
		public override void Complete(CompletionContext context)
		{
			if (addType) {
				MarkAsUsed();
				string newText = Entity.DeclaringType.Name + "." + Text;
				context.Editor.Document.Replace(context.StartOffset, context.Length, newText);
				context.EndOffset = context.StartOffset + newText.Length;
			} else
				base.Complete(context);
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
			this.Image = ClassBrowserIconService.Namespace;
		}
	}
	
	class SpecialCompletionItem : DefaultCompletionItem
	{
		public SpecialCompletionItem(string name)
			: base(name)
		{
			this.Image = ClassBrowserIconService.Namespace;
		}
	}
	
	class SpecialValueCompletionItem : DefaultCompletionItem
	{
		public SpecialValueCompletionItem(string name)
			: base(name)
		{
			this.Image = ClassBrowserIconService.Const;
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
			this.Image = ClassBrowserIconService.Namespace;
		}
		
		public XmlnsCompletionItem(string @namespace, bool isUrl)
			: base(@namespace)
		{
			this.@namespace = @namespace;
			this.isUrl = isUrl;
			this.assembly = string.Empty;
			this.Image = ClassBrowserIconService.Namespace;
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
			this.Image = ClassBrowserIconService.Event;
		}
		
		public override void Complete(CompletionContext context)
		{
			context.Editor.Document.Replace(context.StartOffset, context.Length, this.HandlerName);
			
			context.EndOffset = context.StartOffset + this.HandlerName.Length;
		}
		
		static readonly Regex namePatternRegex = new Regex("%[A-z0-9]*%", RegexOptions.Compiled);
		
		public static string ParseNamePattern(string objectName, string eventName)
		{
			string name = XamlBindingOptions.EventHandlerNamePattern;
			
			while (namePatternRegex.IsMatch(name)) {
				Match match = namePatternRegex.Match(name);
				switch (match.Value.ToUpperInvariant()) {
					case "%OBJECT%":
						if (char.IsUpper(match.Value[1]))
							objectName = objectName.ToUpperInvariant()[0] + objectName.Substring(1, objectName.Length - 1);
						else
							objectName = objectName.ToLowerInvariant()[0] + objectName.Substring(1, objectName.Length - 1);
						name = name.Replace(match.Index, match.Length, objectName);
						break;
					case "%EVENT%":
						if (char.IsUpper(match.Value[1]))
							eventName = eventName.ToUpperInvariant()[0] + eventName.Substring(1, eventName.Length - 1);
						else
							eventName = eventName.ToLowerInvariant()[0] + eventName.Substring(1, eventName.Length - 1);
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
						description = CodeCompletionItem.ConvertDocumentation(entityDoc);
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
						description = CodeCompletionItem.ConvertDocumentation(entityDoc);
					}
					descriptionCreated = true;
				}
				return description;
			}
		}
	}
}
