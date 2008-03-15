// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class OnXXXMethodsCodeGenerator : CodeGeneratorBase
	{
		public override string CategoryName {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.EventOnXXX}";
			}
		}
		
		public override string Hint {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.EventOnXXX.Hint}";
			}
		}
		
		public override int ImageIndex {
			get {
				return ClassBrowserIconService.EventIndex;
			}
		}
		
		protected override void InitContent()
		{
			foreach (IEvent evt in currentClass.Events) {
				Content.Add(new EventWrapper(evt));
			}
		}
		
		public override void GenerateCode(List<AbstractNode> nodes, IList items)
		{
			foreach (EventWrapper ev in items) {
				nodes.Add(codeGen.CreateOnEventMethod(ev.Event));
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
				IAmbience ambience = AmbienceService.GetCurrentAmbience();
				ambience.ConversionFlags = ConversionFlags.None;
				return ambience.Convert(evt);
			}
		}
	}
}
