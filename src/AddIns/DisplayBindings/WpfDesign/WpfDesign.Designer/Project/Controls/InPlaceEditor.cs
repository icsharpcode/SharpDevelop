// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup.Primitives;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Supports editing Text in the Designer	
	/// </summary>
	public class InPlaceEditor : TextBox
	{
		static InPlaceEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (InPlaceEditor), new FrameworkPropertyMetadata(typeof (InPlaceEditor)));
		}
		
		/// <summary>
		/// This property is binded to the Text Property of the editor. 
		/// </summary>
		public static readonly DependencyProperty BindProperty =
            DependencyProperty.Register("Bind", typeof (string), typeof (InPlaceEditor), new FrameworkPropertyMetadata());
		
		public string Bind{
            get { return (string) GetValue(BindProperty); }
            set { SetValue(BindProperty, value); }
        }
		
		readonly DesignItem designItem;
		ChangeGroup changeGroup;
		TextBlock textBlock;
		TextBox editor;
		
		/// <summary>
		/// This is the name of the property that is being edited for example Window.Title, Button.Content .
		/// </summary>
		string property;
		
		public InPlaceEditor(DesignItem designItem)
		{
			this.designItem=designItem;
		}
		
		public override void OnApplyTemplate()
    	{
            base.OnApplyTemplate();
            editor = new TextBox();
            editor = Template.FindName("editor", this) as TextBox; // Gets the TextBox-editor from the Template 
            Debug.Assert(editor != null);
   		}
		
		/// <summary>
		/// Binds the Text Property of the element extended with <see cref="Bind"/>.
		/// </summary>
		/// <param name="textBlock"></param>
		public void SetBinding(TextBlock textBlock)
		{
			Debug.Assert(textBlock!=null);
			this.textBlock = textBlock;
   			Binding binding = new Binding("Text");
           binding.Source = this.textBlock;
           binding.Mode = BindingMode.TwoWay;
           SetBinding(BindProperty, binding);
           property=PropertyUpdated(textBlock);
		}
		
		/// <summary>
		/// Returns the property that is being edited in the element for example editing Window Title returns "Title", 
		/// Button text as "Content".
		/// </summary>
		private string PropertyUpdated(TextBlock text)
		{
			MarkupObject obj = MarkupWriter.GetMarkupObjectFor(designItem.Component);
           foreach (MarkupProperty property in obj.Properties) {
               if (property.DependencyProperty != null && property.StringValue == textBlock.Text)
                   return property.Name;
           }
           return null;
		}
		
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
       	{
           base.OnGotKeyboardFocus(e);
           changeGroup = designItem.OpenGroup("Change Text");
           editor.Focus();
       	}
		
		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
       	{
           if (changeGroup != null)
               changeGroup.Abort();
           if (textBlock != null)
                textBlock.Visibility = Visibility.Visible;
           base.OnLostKeyboardFocus(e);
      	}
		
		/// <summary>
		/// Change is committed if the user releases the Escape Key.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyUp(KeyEventArgs e)
       	{
           base.OnKeyUp(e);
           if (e.Key == Key.Escape) {
	           	// Commit the changes to the DOM
	           	if(property!=null)
	           		designItem.Properties[property].SetValue(Bind);
	           	designItem.Properties[Control.FontFamilyProperty].SetValue(editor.FontFamily);
	           	designItem.Properties[Control.FontSizeProperty].SetValue(editor.FontSize);
	           	designItem.Properties[Control.FontStretchProperty].SetValue(editor.FontStretch);
	           	designItem.Properties[Control.FontStyleProperty].SetValue(editor.FontStyle);
	           	designItem.Properties[Control.FontWeightProperty].SetValue(editor.FontWeight);
           	
               if (changeGroup != null)
                   changeGroup.Commit();
               changeGroup = null;
               this.Visibility = Visibility.Hidden;
               textBlock.Visibility = Visibility.Visible;
           }
       	}		
	}
}
