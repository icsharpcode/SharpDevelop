// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Documents;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Xml;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Provides helper methods to create nicely formatted FlowDocuments from NRefactory XmlDoc.
	/// </summary>
	public static class XmlDocFormatter
	{
		public static FlowDocument CreateTooltip(IType type, bool useFullyQualifiedMemberNames = true)
		{
			var ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.ShowDeclaringType;
			if (useFullyQualifiedMemberNames)
				ambience.ConversionFlags |= ConversionFlags.UseFullyQualifiedEntityNames;
			string header;
			if (type is ITypeDefinition)
				header = ambience.ConvertSymbol((ITypeDefinition)type);
			else
				header = ambience.ConvertType(type);
			
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			DocumentationUIBuilder b = new DocumentationUIBuilder(ambience);
			b.AddCodeBlock(header, keepLargeMargin: true);
			
			ITypeDefinition entity = type.GetDefinition();
			if (entity != null) {
				var documentation = XmlDocumentationElement.Get(entity);
				if (documentation != null) {
					foreach (var child in documentation.Children) {
						b.AddDocumentationElement(child);
					}
				}
			}
			return b.CreateFlowDocument();
		}
		
		public static FlowDocument CreateTooltip(IEntity entity, bool useFullyQualifiedMemberNames = true)
		{
			var ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.ShowDeclaringType;
			if (useFullyQualifiedMemberNames)
				ambience.ConversionFlags |= ConversionFlags.UseFullyQualifiedEntityNames;
			string header = ambience.ConvertSymbol(entity);
			var documentation = XmlDocumentationElement.Get(entity);
			
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			DocumentationUIBuilder b = new DocumentationUIBuilder(ambience);
			b.AddCodeBlock(header, keepLargeMargin: true);
			if (documentation != null) {
				foreach (var child in documentation.Children) {
					b.AddDocumentationElement(child);
				}
			}
			return b.CreateFlowDocument();
		}
	}
}
