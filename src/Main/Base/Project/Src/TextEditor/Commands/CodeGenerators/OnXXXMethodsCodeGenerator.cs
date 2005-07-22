// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.TextEditor;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class OnXXXMethodsCodeGenerator : CodeGenerator
	{
		public override string CategoryName {
			get {
				return "Event OnXXX methods";
			}
		}
		
		public override  string Hint {
			get {
				return "Choose events to generate OnXXX methods";
			}
		}
		
		public override int ImageIndex {
			get {
				
				return ClassBrowserIconService.EventIndex;
			}
		}
		
		public OnXXXMethodsCodeGenerator(IClass currentClass) : base(currentClass)
		{
			foreach (IEvent evt in currentClass.Events) {
				Content.Add(new EventWrapper(evt));
			}
		}
		
		protected override void StartGeneration(IList items, string fileExtension)
		{
			for (int i = 0; i < items.Count; ++i) {
				EventWrapper ew = (EventWrapper)items[i];
				string eventArgsName = String.Empty;
				if (ew.Event.ReturnType.FullyQualifiedName.EndsWith("Handler")) {
					eventArgsName = ew.Event.ReturnType.FullyQualifiedName.Substring(0, ew.Event.ReturnType.FullyQualifiedName.Length - "Handler".Length);
				} else {
					eventArgsName = ew.Event.ReturnType.FullyQualifiedName;
				}
				eventArgsName += "Args";
				
				editActionHandler.InsertString("protected " + (ew.Event.IsStatic ? "static" : "virtual") + " void On" + ew.Event.Name + "(" + eventArgsName + " e)");++numOps;
				if (StartCodeBlockInSameLine) {
					editActionHandler.InsertString(" {");++numOps;
				} else {
					Return();
					editActionHandler.InsertString("{");++numOps;
				}
				Return();
				Indent();
				editActionHandler.InsertString("if (" + ew.Event.Name + " != null)");++numOps;
				if (StartCodeBlockInSameLine) {
					editActionHandler.InsertString(" {");++numOps;
				} else {
					Return();
					editActionHandler.InsertString("{");++numOps;
				}
				
				Return();
				Indent();
				editActionHandler.InsertString(ew.Event.Name + "(this, e);");++numOps;
				Return();
				editActionHandler.InsertChar('}');++numOps;
				Return();
				editActionHandler.InsertChar('}');++numOps;
				Return();
				IndentLine();
			}
		}
		
		class EventWrapper
		{
			IEvent evt;
			public IEvent Event {
				get {
					return evt;
				}
			}
			public EventWrapper(IEvent evt)
			{
				this.evt = evt;
			}
			
			public override string ToString()
			{
				IAmbience ambience = AmbienceService.CurrentAmbience;
				ambience.ConversionFlags = ConversionFlags.None;
				return ambience.Convert(evt);
			}
		}
	}
}
