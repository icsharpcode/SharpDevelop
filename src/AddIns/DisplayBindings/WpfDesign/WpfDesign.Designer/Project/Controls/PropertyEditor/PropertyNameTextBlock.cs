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
		readonly DockPanel toolTipDockPanel;
		bool toolTipInitialized;
		internal DependencyPropertyDotButton ContextMenuProvider;
		
		public PropertyNameTextBlock(IPropertyEditorDataProperty property)
			: base(new Run(property.Name))
		{
			this.property = property;
			this.TextAlignment = TextAlignment.Right;
			this.TextTrimming = TextTrimming.CharacterEllipsis;
			
			this.ToolTip = toolTipDockPanel = new DockPanel();
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
			if (toolTipInitialized)
				return;
			toolTipInitialized = true;
			TextBlock textBlock = new TextBlock();
			textBlock.TextAlignment = TextAlignment.Left;
			textBlock.Inlines.Add(new Bold(new Run(property.Name)));
			if (property.ReturnType != null) {
				textBlock.Inlines.Add(" (" + property.ReturnType.Name + ")");
			}
			DockPanel.SetDock(textBlock, Dock.Top);
			toolTipDockPanel.Children.Add(textBlock);
			object description = property.GetDescription();
			if (description != null) {
				ContentControl cc = new ContentControl();
				cc.Content = description;
				toolTipDockPanel.Children.Add(cc);
			}
		}
	}
}
