// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Core
{
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
		
		public bool UseLinkArrayList {
			get {
				return (conversionFlags & ConversionFlags.UseLinkArrayList) == ConversionFlags.UseLinkArrayList;
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
		
		protected ArrayList linkArrayList;
		
		public ArrayList LinkArrayList {
			get {
				return linkArrayList;
			}
			set {
				linkArrayList = value;
			}
		}
		
		public abstract string WrapAttribute(string attribute);
		public abstract string WrapComment(string comment);
		public abstract string GetIntrinsicTypeName(string dotNetTypeName);
		
	}
}
