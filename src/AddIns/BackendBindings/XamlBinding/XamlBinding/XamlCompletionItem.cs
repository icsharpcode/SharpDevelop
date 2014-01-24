// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text.RegularExpressions;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlCompletionItem.
	/// </summary>
	class XamlCompletionItem : DefaultCompletionItem
	{
		public XamlCompletionItem(string text)
			: base(text)
		{
			this.entity = null;
			this.Image = ClassBrowserIconService.Namespace;
		}
		
		public XamlCompletionItem(IEntity entity)
			: base(entity.Name)
		{
			this.entity = entity;
			this.Image = ClassBrowserIconService.GetIcon(entity);
		}
		
		public XamlCompletionItem(string text, IEntity entity)
			: base(text)
		{
			this.entity = entity;
			this.Image = ClassBrowserIconService.GetIcon(entity);
		}
		
		IEntity entity;
		public IEntity Entity {
			get {
				return entity;
			}
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
	
	class XamlLazyValueCompletionItem : XamlCompletionItem
	{
		bool addType;
		
		public XamlLazyValueCompletionItem(IEntity entity, string text, bool addType)
			: base(text, entity)
		{
			this.addType = addType;
		}
		
		public override void Complete(CompletionContext context)
		{
			if (addType) {
				string newText = Entity + "." + Text;
				context.Editor.Document.Replace(context.StartOffset, context.Length, newText);
				context.EndOffset = context.StartOffset + newText.Length;
			} else
				base.Complete(context);
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
		
		public object Header {
			get {
				if (headerText == null) {
					IAmbience ambience = AmbienceService.GetCurrentAmbience();
					ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
					headerText = ambience.ConvertSymbol(ctor);
					headerText = headerText.Insert(headerText.LastIndexOf(')'), (ctor.Parameters.Count > 0 ? ", " : "") + "Named Parameters ...");
				}
				return headerText;
			}
		}
		
		public object Content {
			get { return ctor.Documentation; }
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, e);
		}
	}
	
	class MemberInsightItem : IInsightItem
	{
		string insightText;
		string headerText;
		IMember member;
		
		public MemberInsightItem(IMember member, string insightText)
		{
			this.member = member;
			this.insightText = insightText;
		}
		
		public object Header {
			get {
				if (headerText == null) {
					headerText = member.Name + "=\"" + insightText + "\"";
				}
				
				return headerText;
			}
		}
		
		public object Content {
			get { return member.Documentation; }
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, e);
		}
	}
}
