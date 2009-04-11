// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	class XamlCompletionItem : CodeCompletionItem
	{
		public XamlCompletionItem(IEntity entity, string prefix)
			: base(entity)
		{
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
		
		public string HandlerName { get; set; }
		
		public NewEventCompletionItem(IEvent eventType, string targetName)
			: base("<new event handler>") // TODO : replace by resource string
		{
			this.eventType = eventType;
			this.targetName = targetName;
		}
		
		public override void Complete(CompletionContext context)
		{
			context.Editor.Document.Replace(context.StartOffset, context.Length, this.HandlerName);
		}
	}
}
