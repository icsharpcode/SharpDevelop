// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A class made up of multiple partial classes.
	/// </summary>
	public class CompoundClass : DefaultClass
	{
		/// <summary>
		/// The parts this class is based on.
		/// Requires manual locking!
		/// </summary>
		internal List<IClass> parts = new List<IClass>();
		
		/// <summary>
		/// Gets the parts this class is based on. This method is thread-safe and
		/// returns a copy of the list!
		/// </summary>
		public IList<IClass> GetParts()
		{
			lock (this) {
				return parts.ToArray();
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
		internal void UpdateInformationFromParts()
		{
			// Common for all parts:
			this.ClassType = parts[0].ClassType;
			this.CompilationUnit.FileName = parts[0].CompilationUnit.FileName;
			this.Region = parts[0].Region;
			
			ModifierEnum modifier = ModifierEnum.None;
			const ModifierEnum defaultClassVisibility = ModifierEnum.Internal;
			
			this.BaseTypes.Clear();
			this.Attributes.Clear();
			foreach (IClass part in parts) {
				if ((part.Modifiers & ModifierEnum.VisibilityMask) != defaultClassVisibility) {
					modifier |= part.Modifiers;
				} else {
					modifier |= part.Modifiers &~ ModifierEnum.VisibilityMask;
				}
				foreach (IReturnType rt in part.BaseTypes) {
					if (!rt.IsDefaultReturnType || rt.FullyQualifiedName != "System.Object") {
						this.BaseTypes.Add(rt);
					}
				}
				foreach (IAttribute attribute in part.Attributes) {
					this.Attributes.Add(attribute);
				}
			}
			if ((modifier & ModifierEnum.VisibilityMask) == ModifierEnum.None) {
				modifier |= defaultClassVisibility;
			}
			this.Modifiers = modifier;
		}
		
		/// <summary>
		/// Type parameters are same on all parts
		/// </summary>
		public override IList<ITypeParameter> TypeParameters {
			get {
				lock (this) {
					// Locking for the time of getting the reference to the sub-list is sufficient:
					// Classes used for parts never change, instead the whole part is replaced with
					// a new IClass instance.
					return parts[0].TypeParameters;
				}
			}
			set {
				throw new NotSupportedException();
			}
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
				lock (this) {
					List<IClass> l = new List<IClass>();
					foreach (IClass part in parts) {
						l.AddRange(part.InnerClasses);
					}
					return l;
				}
			}
		}
		
		public override List<IField> Fields {
			get {
				lock (this) {
					List<IField> l = new List<IField>();
					foreach (IClass part in parts) {
						l.AddRange(part.Fields);
					}
					return l;
				}
			}
		}
		
		public override List<IProperty> Properties {
			get {
				lock (this) {
					List<IProperty> l = new List<IProperty>();
					foreach (IClass part in parts) {
						l.AddRange(part.Properties);
					}
					return l;
				}
			}
		}
		
		public override List<IMethod> Methods {
			get {
				lock (this) {
					List<IMethod> l = new List<IMethod>();
					foreach (IClass part in parts) {
						l.AddRange(part.Methods);
					}
					return l;
				}
			}
		}
		
		public override List<IEvent> Events {
			get {
				lock (this) {
					List<IEvent> l = new List<IEvent>();
					foreach (IClass part in parts) {
						l.AddRange(part.Events);
					}
					return l;
				}
			}
		}
	}
}
