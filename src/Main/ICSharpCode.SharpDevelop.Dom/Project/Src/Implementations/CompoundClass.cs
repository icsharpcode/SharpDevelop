// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A class made up of multiple partial classes.
	/// </summary>
	public class CompoundClass : DefaultClass
	{
		List<IClass> parts = new List<IClass>();
		
		/// <summary>
		/// Gets the parts this class is based on.
		/// </summary>
		public List<IClass> Parts {
			get {
				return parts;
			}
		}
		
		/// <summary>
		/// Creates a new CompoundClass with the specified class as first part.
		/// </summary>
		public CompoundClass(IClass firstPart) : base(firstPart.CompilationUnit, firstPart.FullyQualifiedName)
		{
			parts.Add(firstPart);
			UpdateInformationFromParts();
		}
		
		/// <summary>
		/// Re-calculate information from class parts (Modifier, Base classes, Type parameters etc.)
		/// </summary>
		public void UpdateInformationFromParts()
		{
			// Common for all parts:
			this.ClassType = parts[0].ClassType;
			this.CompilationUnit.FileName = parts[0].CompilationUnit.FileName;
			this.Region = parts[0].Region;
			
			ModifierEnum modifier = ModifierEnum.None;
			this.BaseTypes.Clear();
			this.TypeParameters.Clear();
			this.Attributes.Clear();
			foreach (IClass part in parts) {
				modifier |= part.Modifiers;
				foreach (IReturnType rt in part.BaseTypes) {
					if (!rt.IsDefaultReturnType || rt.FullyQualifiedName != "System.Object") {
						this.BaseTypes.Add(rt);
					}
				}
				foreach (ITypeParameter typeParam in part.TypeParameters) {
					this.TypeParameters.Add(typeParam);
				}
				foreach (IAttribute attribute in part.Attributes) {
					this.Attributes.Add(attribute);
				}
			}
			this.Modifiers = modifier;
		}
		
		/// <summary>
		/// CompoundClass has a normal return type even though IsPartial is set.
		/// </summary>
		protected override IReturnType CreateDefaultReturnType()
		{
			return new DefaultReturnType(this);
		}
		
		public override List<IClass> InnerClasses {
			get {
				List<IClass> l = new List<IClass>();
				foreach (IClass part in parts) {
					l.AddRange(part.InnerClasses);
				}
				return l;
			}
		}
		
		public override List<IField> Fields {
			get {
				List<IField> l = new List<IField>();
				foreach (IClass part in parts) {
					l.AddRange(part.Fields);
				}
				return l;
			}
		}
		
		public override List<IProperty> Properties {
			get {
				List<IProperty> l = new List<IProperty>();
				foreach (IClass part in parts) {
					l.AddRange(part.Properties);
				}
				return l;
			}
		}
		
		public override List<IMethod> Methods {
			get {
				List<IMethod> l = new List<IMethod>();
				foreach (IClass part in parts) {
					l.AddRange(part.Methods);
				}
				return l;
			}
		}
		
		public override List<IEvent> Events {
			get {
				List<IEvent> l = new List<IEvent>();
				foreach (IClass part in parts) {
					l.AddRange(part.Events);
				}
				return l;
			}
		}
	}
}
