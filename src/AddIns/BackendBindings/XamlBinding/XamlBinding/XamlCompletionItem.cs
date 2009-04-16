// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XamlBinding
{
	class XamlCompletionItem : CodeCompletionItem
	{
		public XamlCompletionItem(IEntity entity, string prefix)
			: base(entity)
		{
			if (string.IsNullOrEmpty(prefix))
				this.Text = entity.Name;
			else
				this.Text = prefix + ":" + entity.Name;
		}
		
		public XamlCompletionItem(IEntity entity)
			: base(entity)
		{
			this.Text = entity.Name;
		}
		
		public XamlCompletionItem(string text, IEntity entity)
			: base(entity)
		{
			this.Text = text;
		}
		
		public override string ToString()
		{
			return "[" + this.Text + "]";
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
			: base("<new event handler>") // TODO : replace by resource string
		{
			this.eventType = eventType;
			this.targetName = targetName;
			// TODO : Add formatting options
			this.HandlerName = this.TargetName + "_" + this.EventType.Name;
		}
		
		public override void Complete(CompletionContext context)
		{
			context.Editor.Document.Replace(context.StartOffset, context.Length, this.HandlerName);
			
			context.EndOffset = context.StartOffset + this.HandlerName.Length;
		}
	}
}
