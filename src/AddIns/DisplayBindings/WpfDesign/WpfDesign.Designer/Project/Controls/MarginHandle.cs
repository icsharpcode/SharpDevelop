// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using ICSharpCode.WpfDesign.Adorners;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
    /// <summary>
    /// Adorner that displays the margin of a control in a Grid.
    /// </summary>
    class MarginHandle : Control
    {
       
        static MarginHandle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarginHandle), new FrameworkPropertyMetadata(typeof(MarginHandle)));
        }

        /// <summary>
        /// Dependency property for <see cref="HandleLength"/>.
        /// </summary>
        public static readonly DependencyProperty HandleLengthProperty
            = DependencyProperty.Register("HandleLength", typeof(double), typeof(MarginHandle), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnHandleLengthChanged)));

        /// <summary>
        /// Gets/Sets the length of Margin Handle.
        /// </summary>
        public double HandleLength{
            get { return (double)GetValue(HandleLengthProperty); }
            set { SetValue(HandleLengthProperty, value); }
        }
        
        readonly Grid grid;
        readonly DesignItem adornedControlItem;
        readonly AdornerPanel adornerPanel;
        readonly HandleOrientation orientation;
        readonly FrameworkElement adornedControl;

        /// <summary> This grid contains the handle line and the endarrow.</summary>  
        Grid lineArrow;
        MarginStub marginStub;
                
        /// <summary>
        /// Gets/Sets the angle by which handle rotates.
        /// </summary>
        public double Angle { get; set; }
        
        /// <summary>
        /// Gets/Sets the angle by which the Margin display has to be rotated
        /// </summary>
        public double TextTransform{
            get{
                if((double)orientation==90 || (double)orientation == 180)
                    return 180;
                if ((double)orientation == 270)
                    return 0;
                return (double)orientation;
            }
        	set{ }
        }
                
        /// <summary>
        /// Decides the visiblity of handle/stub when <see cref="HandleLength"/> changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnHandleLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MarginHandle mar=(MarginHandle)d;
            mar.DecideVisiblity((double)e.NewValue);
        }
        
        /// <summary>
        /// Decides whether to permanently display the handle or not.
        /// </summary>
        public bool ShouldBeVisible { get; set; }

        public MarginHandle(DesignItem adornedControlItem, AdornerPanel adornerPanel, HandleOrientation orientation)
        {
        	 Debug.Assert(adornedControlItem!=null);
            this.adornedControlItem = adornedControlItem;
            this.adornerPanel = adornerPanel;
            this.orientation = orientation;
            Angle = (double)orientation;
            grid=(Grid)adornedControlItem.Parent.Component;
            adornedControl=(FrameworkElement)adornedControlItem.Component;
            marginStub = new MarginStub(this);
            BindAndPlaceHandle();
        }

        /// <summary>
        /// Binds the <see cref="HandleLength"/> to the margin and place the handles.
        /// </summary>
        void BindAndPlaceHandle()
        {
            if (!adornerPanel.Children.Contains(this))
                adornerPanel.Children.Add(this);
            if (!adornerPanel.Children.Contains(marginStub))
                adornerPanel.Children.Add(marginStub);
            RelativePlacement placement=new RelativePlacement();
            Binding binding = new Binding();
            binding.Source = adornedControl;
            switch (orientation)
            {
                case HandleOrientation.Left:
                    binding.Path = new PropertyPath("Margin.Left"); 
                    placement = new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Center);
                    break;
                case HandleOrientation.Top:
                    binding.Path = new PropertyPath("Margin.Top");
                    placement = new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Top);
                    break;
                case HandleOrientation.Right:
                    binding.Path = new PropertyPath("Margin.Right");
                    placement = new RelativePlacement(HorizontalAlignment.Right, VerticalAlignment.Center);
                    break;
                case HandleOrientation.Bottom:
                    binding.Path = new PropertyPath("Margin.Bottom");
                    placement = new RelativePlacement(HorizontalAlignment.Center, VerticalAlignment.Bottom);
                    break;
            }

            binding.Mode = BindingMode.TwoWay;
            SetBinding(HandleLengthProperty, binding);

            AdornerPanel.SetPlacement(this, placement);
            AdornerPanel.SetPlacement(marginStub, placement);
           
            DecideVisiblity(this.HandleLength);
        }

        /// <summary>
        /// Decides the visibllity of Handle or stub,whichever is set and hides the line-endarrow if the control is near the Grid or goes out of it.
        /// </summary>
        /// <param name="handleLength"></param>
        public void DecideVisiblity(double handleLength)
        {
        	 if(ShouldBeVisible){
            	marginStub.Visibility = handleLength == 0.0 ? Visibility.Visible : Visibility.Hidden;
            	this.Visibility = handleLength != 0.0 ? Visibility.Visible : Visibility.Hidden;
            	if (this.lineArrow != null){            
                	lineArrow.Visibility = handleLength < 23 ? Visibility.Hidden : Visibility.Visible;
            	}
        	}
        }
     
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            lineArrow = new Grid();
            lineArrow = (Grid)Template.FindName("lineArrow", this) as Grid;
            Debug.Assert(lineArrow != null);
        }

    }

    /// <summary>
    /// Display a stub indicating that the margin is not set.
    /// </summary>
    class MarginStub : Control
    {
        MarginHandle marginHandle;
        static MarginStub()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarginStub), new FrameworkPropertyMetadata(typeof(MarginStub)));
        }

        public MarginStub(MarginHandle marginHandle)
        {
            this.marginHandle = marginHandle;
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            marginHandle.DecideVisiblity(marginHandle.HandleLength);
        }
        
    }

    /// <summary>
    /// Specifies the Handle orientation
    /// </summary>
    public enum HandleOrientation
    {
        /*  Rotation of the handle is done with respect to right orientation and in clockwise direction*/

        /// <summary>
        /// Indicates that the margin handle is left-oriented and rotated 180 degrees with respect to <see cref="Right"/>.
        /// </summary>
        Left = 180,
        /// <summary>
        /// Indicates that the margin handle is top-oriented and rotated 270 degrees with respect to <see cref="Right"/>.
        /// </summary>
        Top = 270,
        /// <summary>
        /// Indicates that the margin handle is right.
        /// </summary>
        Right = 0,
        /// <summary>
        /// Indicates that the margin handle is left-oriented and rotated 180 degrees with respect to <see cref="Right"/>.
        /// </summary>
        Bottom = 90
    }
}

