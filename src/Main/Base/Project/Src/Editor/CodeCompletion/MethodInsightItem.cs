// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
		}
		
		public IEntity Entity {
			get { return entity; }
		}
		
		string headerText;
		bool descriptionCreated;
		string description;
		
		public object Header {
			get {
				if (headerText == null) {
					IAmbience ambience = AmbienceService.GetCurrentAmbience();
					ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
					headerText = ambience.Convert(entity);
				}
				return headerText;
			}
		}
		
		public object Content {
			get {
				if (!descriptionCreated) {
					string entityDoc = entity.Documentation;
					if (!string.IsNullOrEmpty(entityDoc)) {
						description = ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.CodeCompletionData.ConvertDocumentation(entityDoc);
					}
					descriptionCreated = true;
				}
				return description;
			}
		}
	}
}
