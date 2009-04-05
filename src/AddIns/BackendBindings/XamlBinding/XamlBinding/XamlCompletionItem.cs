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
	class XamlCompletionItem : DefaultCompletionItem
	{
		IEntity entity;
		
		public IEntity Entity {
			get { return entity; }
		}
		
		public XamlCompletionItem(IEntity entity, string prefix)
			: base(prefix + ":" + entity.Name)
		{
			this.entity = entity;
		}
		
		public XamlCompletionItem(IEntity entity)
			: base(entity.Name)
		{
			this.entity = entity;
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
