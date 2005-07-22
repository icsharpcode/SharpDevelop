// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public abstract class AbstractFieldCodeGenerator : CodeGenerator
	{
		public AbstractFieldCodeGenerator(IClass currentClass) : base(currentClass)
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
				IAmbience ambience = AmbienceService.CurrentAmbience;
				ambience.ConversionFlags = ConversionFlags.ShowReturnType | ConversionFlags.ShowModifiers;
				return ambience.Convert(field);
			}
		}
	}
}
