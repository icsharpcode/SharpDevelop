// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ICSharpCode.WpfDesign.PropertyEditor;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	// Text block used in the first column of the PropertyGridView.
	// Creates ToolTip and ContextMenu objects on-demand.
	sealed class PropertyNameTextBlock : TextBlock
	{
		readonly IPropertyEditorDataProperty property;
		readonly TextBlock toolTipTextBlock;
		bool toolTipTextBlockInitialized;
		internal DependencyPropertyDotButton ContextMenuProvider;
		
		public PropertyNameTextBlock(IPropertyEditorDataProperty property)
			: base(new Run(property.Name))
		{
			this.property = property;
			this.TextAlignment = TextAlignment.Right;
			this.TextTrimming = TextTrimming.CharacterEllipsis;
			
			this.ToolTip = toolTipTextBlock = new TextBlock();
		}
		
		protected override void OnToolTipOpening(ToolTipEventArgs e)
		{
			CreateToolTip();
			base.OnToolTipOpening(e);
		}
		
		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			if (ContextMenuProvider != null) {
				this.ContextMenu = ContextMenuProvider.CreateContextMenu();
			}
			base.OnContextMenuOpening(e);
		}
		
		void CreateToolTip()
		{
			if (toolTipTextBlockInitialized)
				return;
			toolTipTextBlockInitialized = true;
			toolTipTextBlock.TextAlignment = TextAlignment.Left;
			toolTipTextBlock.Inlines.Add(new Bold(new Run(property.Name)));
			if (property.ReturnType != null) {
				toolTipTextBlock.Inlines.Add(" (" + property.ReturnType.Name + ")");
			}
			if (!string.IsNullOrEmpty(property.Description)) {
				toolTipTextBlock.Inlines.Add(new LineBreak());
				toolTipTextBlock.Inlines.Add(property.Description);
			}
		}
	}
}
