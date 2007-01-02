// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// The type editor used when no other type editor could be found.
	/// </summary>
	[TypeEditor(typeof(object))]
	public sealed class FallbackEditor : TextBlock
	{
		/// <summary>
		/// Creates a new FallbackEditor instance for the specified property.
		/// </summary>
		public FallbackEditor(IPropertyEditorDataProperty property)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			
			this.TextTrimming = TextTrimming.CharacterEllipsis;
			if (property.IsSet) {
				this.FontWeight = FontWeights.Bold;
			}
			object val = property.Value;
			if (val == null) {
				this.Text = "null";
				this.FontStyle = FontStyles.Italic;
			} else {
				try {
					this.Text = val.ToString();
				} catch (Exception ex) {
					this.FontWeight = FontWeights.Regular;
					Inlines.Add(new Italic(new Run(ex.GetType().Name)));
					Inlines.Add(" ");
					Inlines.Add(ex.Message);
				}
			}
		}
	}
}
