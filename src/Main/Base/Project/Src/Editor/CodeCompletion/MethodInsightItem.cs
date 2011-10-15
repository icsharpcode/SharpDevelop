// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ICSharpCode.SharpDevelop.Dom;
using System;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// An insight item that represents an entity.
	/// </summary>
	public class MethodInsightItem : IInsightItem
	{
		IEntity entity;
		
		public MethodInsightItem(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			this.entity = entity;
			this.highlightParameter = -1;
		}
		
		public IEntity Entity {
			get { return entity; }
		}
		
		string headerText;
		bool descriptionCreated;
		string description;
		int highlightParameter;
		object fancyHeader;
		
		public int HighlightParameter {
			get { return highlightParameter; }
			set {
				if (highlightParameter != value) {
					highlightParameter = value;
					fancyHeader = GenerateHeader();
				}
			}
		}
		
		object GenerateHeader()
		{
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
			
			if (headerText == null) {
				headerText = ambience.Convert(entity);
			}
			
			if (highlightParameter < 0)
				return headerText;
			
			if (entity is IMethod) {
				var method = entity as IMethod;
				string param = "";
				if (method.Parameters.Count > highlightParameter)
					param = ambience.Convert(method.Parameters[highlightParameter]);
				
				if (!string.IsNullOrEmpty(param)) {
					// append ','  or ')' to avoid missing highlighting if there are multiple parameter names starting with the same substring.
					string[] parts = headerText.Split(new[] { param + (method.Parameters.Count - 1 == highlightParameter ? ')' : ',') }, StringSplitOptions.None);
					if (parts.Length != 2)
						return headerText;
					return new TextBlock { // add text wrapping
						TextWrapping = TextWrapping.Wrap,
						Inlines = {
							parts[0],
							new Bold() { Inlines = { param } },
							(method.Parameters.Count - 1 == highlightParameter ? ')' : ',') + parts[1]
						}
					};
				}
			}
			
			return headerText;
		}
		
		public object Header {
			get {
				if (fancyHeader == null)
					fancyHeader = GenerateHeader();
				
				return fancyHeader;
			}
		}
		
		public object Content {
			get {
				if (!descriptionCreated) {
					string entityDoc = entity.Documentation;
					if (!string.IsNullOrEmpty(entityDoc)) {
						description = CodeCompletionItem.ConvertDocumentation(entityDoc);
					}
					descriptionCreated = true;
				}
				return description;
			}
		}
	}
}
