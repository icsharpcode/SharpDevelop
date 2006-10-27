// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1951 $</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Flags]
	public enum ConversionFlags {
		None                   = 0,
		ShowParameterNames     = 1,
		ShowAccessibility      = 16,
		UseFullyQualifiedNames = 2,
		ShowModifiers          = 4,
		ShowInheritanceList    = 8,
		IncludeHTMLMarkup      = 32,
		QualifiedNamesOnlyForReturnTypes = 128,
		IncludeBodies                    = 256,
		ShowReturnType                   = 512,
		
		StandardConversionFlags = ShowParameterNames |
			UseFullyQualifiedNames |
			ShowReturnType |
			ShowModifiers,
		
		All = ShowParameterNames |
			ShowAccessibility |
			UseFullyQualifiedNames |
			ShowModifiers |
			ShowReturnType |
			ShowInheritanceList,
	}
	
	public interface IAmbience
	{
		ConversionFlags ConversionFlags {
			get;
			set;
		}
		
		string Convert(ModifierEnum modifier);
		
		string Convert(IClass c);
		string ConvertEnd(IClass c);
		
		string Convert(IField field);
		string Convert(IProperty property);
		string Convert(IEvent e);
		
		string Convert(IMethod m);
		string ConvertEnd(IMethod m);
		
		string Convert(IParameter param);
		string Convert(IReturnType returnType);
		
		string WrapAttribute(string attribute);
		string WrapComment(string comment);
		
		string GetIntrinsicTypeName(string dotNetTypeName);
	}
	
	public abstract class AbstractAmbience : IAmbience
	{
		ConversionFlags conversionFlags = ConversionFlags.StandardConversionFlags;
		
		public ConversionFlags ConversionFlags {
			get {
				return conversionFlags;
			}
			set {
				conversionFlags = value;
			}
		}
		
		public bool ShowReturnType {
			get {
				return (conversionFlags & ConversionFlags.ShowReturnType) == ConversionFlags.ShowReturnType;
			}
		}
		
		public bool ShowAccessibility {
			get {
				return (conversionFlags & ConversionFlags.ShowAccessibility) == ConversionFlags.ShowAccessibility;
			}
		}
		
		public bool ShowParameterNames {
			get {
				return (conversionFlags & ConversionFlags.ShowParameterNames) == ConversionFlags.ShowParameterNames;
			}
		}
		
		public bool UseFullyQualifiedNames {
			get {
				return (conversionFlags & ConversionFlags.UseFullyQualifiedNames) == ConversionFlags.UseFullyQualifiedNames;
			}
		}
		
		public bool ShowModifiers {
			get {
				return (conversionFlags & ConversionFlags.ShowModifiers) == ConversionFlags.ShowModifiers;
			}
		}
		
		public bool ShowInheritanceList {
			get {
				return (conversionFlags & ConversionFlags.ShowInheritanceList) == ConversionFlags.ShowInheritanceList;
			}
		}
		
		public bool IncludeHTMLMarkup {
			get {
				return (conversionFlags & ConversionFlags.IncludeHTMLMarkup) == ConversionFlags.IncludeHTMLMarkup;
			}
		}
		
		public bool UseFullyQualifiedMemberNames {
			get {
				return UseFullyQualifiedNames && !((conversionFlags & ConversionFlags.QualifiedNamesOnlyForReturnTypes) == ConversionFlags.QualifiedNamesOnlyForReturnTypes);
			}
		}
		
		public bool IncludeBodies {
			get {
				return (conversionFlags & ConversionFlags.IncludeBodies) == ConversionFlags.IncludeBodies;
			}
		}
		
		public abstract string Convert(ModifierEnum modifier);
		public abstract string Convert(IClass c);
		public abstract string ConvertEnd(IClass c);
		public abstract string Convert(IField c);
		public abstract string Convert(IProperty property);
		public abstract string Convert(IEvent e);
		public abstract string Convert(IMethod m);
		public abstract string ConvertEnd(IMethod m);
		public abstract string Convert(IParameter param);
		public abstract string Convert(IReturnType returnType);
		
		public abstract string WrapAttribute(string attribute);
		public abstract string WrapComment(string comment);
		public abstract string GetIntrinsicTypeName(string dotNetTypeName);
	}
}
