// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public abstract class AbstractFieldCodeGenerator : CodeGeneratorBase
	{
		protected override void InitContent()
		{
			foreach (IField field in currentClass.Fields) {
				Content.Add(new FieldWrapper(field));
			}
		}
		
		public class FieldWrapper
		{
			IField field;
			
			public IField Field {
				get {
					return field;
				}
			}
			
			public FieldWrapper(IField field)
			{
				this.field = field;
			}
			
			public override string ToString()
			{
				IAmbience ambience = AmbienceService.GetCurrentAmbience();
				ambience.ConversionFlags = ConversionFlags.ShowReturnType | ConversionFlags.ShowModifiers;
				return ambience.Convert(field);
			}
		}
	}
}
