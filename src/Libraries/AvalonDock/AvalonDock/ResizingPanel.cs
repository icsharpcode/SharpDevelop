//Copyright (c) 2007-2010, Adolfo Marinucci
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, 
//are permitted provided that the following conditions are met:
//
//* Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer.
//* Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution.
//* Neither the name of Adolfo Marinucci nor the names of its contributors may 
//  be used to endorse or promote products derived from this software without 
//  specific prior written permission.
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
//AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
//IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
//INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
//PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
//HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
//EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Diagnostics;
using System.Linq;


namespace AvalonDock
{
    public class ResizingPanel : Panel, IDockableControl
    {
        /// <summary>
        /// Gets or sets the orientation of the panel
        /// </summary>
        /// <remarks>If horizontal oriented children are positioned from left to right and width of each child is computed according to <see cref="ResizingWidth"/> attached property value. When vertical oriented children are arranged from top to bottom, according to <see cref="ResizingHeight"/> of each child.</remarks>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Give access to Orientation attached property
        /// </summary>
        /// <remarks>If horizontal oriented children are positioned from left to right and width of each child is computed according to <see cref="ResizingWidth"/> attached property value. When vertical oriented children are arranged from top to bottom, according to <see cref="ResizingHeight"/> of each child.</remarks>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ResizingPanel), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure, OnOrientationChanged));

        static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ResizingPanel)d).splitterListIsDirty = true;
        }

        public static GridLength GetResizeWidth(DependencyObject obj)
        {
            return (GridLength)obj.GetValue(ResizeWidthProperty);
        }

        public static void SetResizeWidth(DependencyObject obj, GridLength value)
        {
            obj.SetValue(ResizeWidthProperty, value);
        }


        public static readonly DependencyProperty ResizeWidthProperty =
            DependencyProperty.RegisterAttached("ResizeWidth",
            typeof(GridLength),
            typeof(ResizingPanel),
            new FrameworkPropertyMetadata(new GridLength(1.0, GridUnitType.Star),
                OnSplitSizeChanged,
                OnCoerceSplitSize),
                new ValidateValueCallback(IsSplitSizeValid));

        public static GridLength GetResizeHeight(DependencyObject obj)
        {
            return (GridLength)obj.GetValue(ResizeHeightProperty);
        }

        public static void SetResizeHeight(DependencyObject obj, GridLength value)
        {
            obj.SetValue(ResizeHeightProperty, value);
        }


        public static readonly DependencyProperty ResizeHeightProperty =
            DependencyProperty.RegisterAttached("ResizeHeight",
            typeof(GridLength),
            typeof(ResizingPanel),
            new FrameworkPropertyMetadata(new GridLength(1.0, GridUnitType.Star),
                OnSplitSizeChanged,
                OnCoerceSplitSize),
                new ValidateValueCallback(IsSplitSizeValid));

        static void OnSplitSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ResizingPanel parentPanel = LogicalTreeHelper.GetParent(sender) as ResizingPanel;
            if (parentPanel != null)
                parentPanel.InvalidateMeasure();
        }

        static object OnCoerceSplitSize(DependencyObject sender, object value)
        {
            GridLength gd = (GridLength)value;

            if (gd.Value < 0.0)
                gd = new GridLength(0.0, gd.GridUnitType);

            return gd;
        }

        static bool IsSplitSizeValid(object value)
        {
            GridLength v = (GridLength)value;
            return v.IsStar || v.IsAbsolute;//at the moment auto is not supported
        }



        public static Size GetEffectiveSize(DependencyObject obj)
        {
            return (Size)obj.GetValue(EffectiveSizeProperty);
        }

        public static void SetEffectiveSize(DependencyObject obj, Size value)
        {
            obj.SetValue(EffectiveSizeProperty, value);
        }

        public static readonly DependencyProperty EffectiveSizeProperty =
            DependencyProperty.RegisterAttached("EffectiveSize", typeof(Size), typeof(ResizingPanel), new FrameworkPropertyMetadata(new Size()));

 
        List<Resizer> _splitterList = new List<Resizer>();

        /// <summary>
        /// Correct sizes of children if all of them are set to absolutes
        /// </summary>
        void CorrectSizes()
        {
            IEnumerable<FrameworkElement> children = Children.OfType<FrameworkElement>().Where<FrameworkElement>(c => !(c is Resizer));

            if (children.All<FrameworkElement>(c => c.IsAbsolute()))
            {
                double totSum = children.Sum<FrameworkElement>(f => f.GetAbsoluteValue());
                foreach (var c in children)
                {
                    if (Orientation == Orientation.Horizontal)
                        SetResizeWidth(c, new GridLength(c.GetAbsoluteValue() / totSum, GridUnitType.Star));
                    else
                        SetResizeHeight(c, new GridLength(c.GetAbsoluteValue() / totSum, GridUnitType.Star));
                }
            }
        }

        /// <summary>
        /// Helper funcs which correct elements size of a resizing panel
        /// </summary>
        internal void AdjustPanelSizes()
        {
            IEnumerable<FrameworkElement> children = Children.OfType<FrameworkElement>().Where<FrameworkElement>(c => !(c is Resizer));

            if (!this.IsLogicalChildContained<DocumentPane>())
            { 
                //if no document pane is contained in this panel
                //adjust elements so that any child will get a proportional star size
                if (Orientation == Orientation.Horizontal)
                {
                    double totSum = children.Sum<FrameworkElement>(f => f.IsAbsolute() ? f.GetAbsoluteValue() : GetEffectiveSize(f).Width);
                    foreach (var c in children)
                        SetResizeWidth(c, new GridLength((c.IsAbsolute() ? c.GetAbsoluteValue() : GetEffectiveSize(c).Width) / totSum, GridUnitType.Star));
                }
                else
                {
                    double totSum = children.Sum<FrameworkElement>(f => f.IsAbsolute() ? f.GetAbsoluteValue() : GetEffectiveSize(f).Height);
                    foreach (var c in children)
                        SetResizeHeight(c, new GridLength((c.IsAbsolute() ? c.GetAbsoluteValue() : GetEffectiveSize(c).Height) / totSum, GridUnitType.Star));
                }
            }

        }


        /// <summary>
        /// Compute the desidered size of the panel
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            //Debug.WriteLine("ResizingPanel.MeasureOverride()");

            SetupSplitters();

            CorrectSizes();

            //Compute the list of visible children
            List<FrameworkElement> visibleChildren = new List<FrameworkElement>();
            for (int i = 0; i < VisualChildrenCount; i++)
            {
                FrameworkElement child = GetVisualChild(i) as FrameworkElement;

                IDockableControl dockableControl = child as IDockableControl;
                if (dockableControl  != null &&
                    !dockableControl.IsDocked)
                {
                    child.Measure(Size.Empty);

                    if (i == VisualChildrenCount - 1 &&
                        i > 0)
                    {
                        child = GetVisualChild(i - 1) as FrameworkElement;
                        Debug.Assert(child is Resizer);

                        child.Measure(Size.Empty);

                        if (visibleChildren.Count > 0)
                        {
                            Debug.Assert(visibleChildren[visibleChildren.Count - 1] is Resizer);
                            visibleChildren[visibleChildren.Count - 1].Measure(Size.Empty);
                            visibleChildren.RemoveAt(visibleChildren.Count - 1);
                        }
                    }
                    else if (i < VisualChildrenCount - 1)
                    {
                        i++;
                        child = GetVisualChild(i) as FrameworkElement;
                        Debug.Assert(child is Resizer);
                        child.Measure(Size.Empty);
                    }

                    continue;
                }

                visibleChildren.Add(child);
            }


            //with no children no space needed
            if (visibleChildren.Count == 0)
                return new Size();

            NormalizeStarLength(visibleChildren);

            Debug.Assert(!(visibleChildren.Last<FrameworkElement>() is Resizer));

            if (availableSize.Width == double.PositiveInfinity &&
                Orientation == Orientation.Horizontal)
            {
                Size newAvailSize = new Size();
                foreach (FrameworkElement child in visibleChildren)
                {
                    child.Measure(availableSize);
                    newAvailSize.Width += child.DesiredSize.Width;
                    newAvailSize.Height = Math.Max(child.DesiredSize.Height, newAvailSize.Height);
                }
                availableSize = newAvailSize;
            }
            //Thx to TMx
            else if (availableSize.Height == double.PositiveInfinity &&
                Orientation == Orientation.Vertical)
            {
                Size newAvailSize = new Size();
                foreach (FrameworkElement child in visibleChildren)
                {
                    child.Measure(newAvailSize);
                    newAvailSize.Width = Math.Max(child.DesiredSize.Width, newAvailSize.Width);
                    newAvailSize.Height += child.DesiredSize.Height;
                }
                availableSize = newAvailSize;
            }

            var splitters = from FrameworkElement child in visibleChildren
                            where child is Resizer
                            select child;
            var childStars = from FrameworkElement child in visibleChildren
                             where (!(child is Resizer)) && child.IsStar()
                             select child;

            var childAbsolutes = from FrameworkElement child in visibleChildren
                                 where (!(child is Resizer)) && child.IsAbsolute()
                                 select child;

            var childAutoSizes = from FrameworkElement child in visibleChildren
                                 where (!(child is Resizer)) && child.IsAuto()
                                 select child;

            //calculate the size of the splitters
            Size splitterSize = new Size();
            foreach (Resizer splitter in splitters)
            {
                splitterSize.Width += splitter.MinWidth;
                splitterSize.Height += splitter.MinHeight;
            }

            Size minimumSize = new Size(splitterSize.Width, splitterSize.Height);
            foreach (FrameworkElement child in childStars)
            {
                minimumSize.Width += child.MinWidth;
                minimumSize.Height += child.MinHeight;
            }
            foreach (FrameworkElement child in childAbsolutes)
            {
                minimumSize.Width += child.MinWidth;
                minimumSize.Height += child.MinHeight;
            }
            foreach (FrameworkElement child in childAutoSizes)
            {
                minimumSize.Width += child.MinWidth;
                minimumSize.Height += child.MinHeight;
            }


            Size minimumPrefferedSize = new Size(minimumSize.Width, minimumSize.Height);
            foreach (FrameworkElement child in childAbsolutes)
            {
                minimumPrefferedSize.Width += child.GetAbsoluteValue() - child.MinWidth;
                minimumPrefferedSize.Height += child.GetAbsoluteValue() - child.MinHeight;
            }
            foreach (FrameworkElement child in childAutoSizes)
            {
                minimumPrefferedSize.Width += child.DesiredSize.Width - child.MinWidth;
                minimumPrefferedSize.Height += child.DesiredSize.Height - child.MinHeight;
            }

            if (Orientation == Orientation.Horizontal)
            {
                #region Horizontal Orientation

                //if finalSize is not sufficient...
                double offset = 0.0;
                double maxHeight = 0.0;
                if (minimumSize.Width >= availableSize.Width)
                {
                    foreach (FrameworkElement child in visibleChildren)
                    {
                        child.Measure(new Size(child.MinWidth, availableSize.Height));
                        maxHeight = Math.Max(child.DesiredSize.Height, maxHeight);
                        offset += child.MinWidth;
                    }
                }
                else if (minimumPrefferedSize.Width >= availableSize.Width)
                {
                    double delta = (minimumPrefferedSize.Width - availableSize.Width) / childAbsolutes.Count<FrameworkElement>();

                    foreach (FrameworkElement child in visibleChildren)
                    {
                        if (child is Resizer)
                        {
                            child.Measure(new Size(child.MinWidth, availableSize.Height));
                            maxHeight = Math.Max(child.DesiredSize.Height, maxHeight);
                            offset += child.MinWidth;
                        }
                        else if (child.IsAbsolute())
                        {
                            child.Measure(new Size(Math.Max(child.GetAbsoluteValue() - delta, child.MinWidth), availableSize.Height));
                            maxHeight = Math.Max(child.DesiredSize.Height, maxHeight);
                            offset += child.GetAbsoluteValue() - delta;
                        }
                        else
                        {
                            child.Measure(new Size(child.MinWidth, availableSize.Height));
                            maxHeight = Math.Max(child.DesiredSize.Height, maxHeight);
                            offset += child.MinWidth;
                        }
                    }
                }
                else
                {
                    double starsSum = childStars.Sum<FrameworkElement>(v => v.GetStarValue());
                    double starsFinalWidth =
                        availableSize.Width -
                        splitters.Sum<FrameworkElement>(s => s.MinWidth) -
                        childAbsolutes.Sum<FrameworkElement>(a => a.GetAbsoluteValue()) -
                        childAutoSizes.Sum<FrameworkElement>(a => a.DesiredSize.Width);

                    foreach (FrameworkElement child in visibleChildren)
                    {
                        if (child is Resizer)
                        {
                            child.Measure(new Size(child.MinWidth, availableSize.Height));
                            maxHeight = Math.Max(child.DesiredSize.Height, maxHeight);
                            offset += child.MinWidth;
                        }
                        else if (child.IsAbsolute())
                        {
                            child.Measure(new Size(child.GetAbsoluteValue(), availableSize.Height));
                            maxHeight = Math.Max(child.DesiredSize.Height, maxHeight);
                            offset += child.GetAbsoluteValue();
                        }
                        else if (child.IsStar())
                        {
                            double w = child.GetStarValue() / starsSum * starsFinalWidth;
                            child.Measure(new Size(w, availableSize.Height));
                            maxHeight = Math.Max(child.DesiredSize.Height, maxHeight);
                            offset += w;
                        }
                        else
                        {
                            child.Measure(new Size(child.DesiredSize.Width, availableSize.Height));
                            maxHeight = Math.Max(child.DesiredSize.Height, maxHeight);
                            offset += child.DesiredSize.Width;
                        }
                    }
                }

                return new Size(offset, maxHeight);

                #endregion
            }
            else
            {
                #region Vertical Orientation

                //if finalSize is not sufficient...
                double offset = 0.0;
                double maxWidth = 0.0;
                if (minimumSize.Height >= availableSize.Height)
                {
                    foreach (FrameworkElement child in visibleChildren)
                    {
                        child.Measure(new Size(availableSize.Width, child.MinHeight));
                        maxWidth = Math.Max(child.DesiredSize.Width, maxWidth);
                        offset += child.MinHeight;
                    }
                }
                else if (minimumPrefferedSize.Height >= availableSize.Height)
                {
                    double delta = (minimumPrefferedSize.Height - availableSize.Height) / childAbsolutes.Count<FrameworkElement>();

                    foreach (FrameworkElement child in visibleChildren)
                    {
                        if (child is Resizer)
                        {
                            child.Measure(new Size(availableSize.Width, child.MinHeight));
                            maxWidth = Math.Max(child.DesiredSize.Width, maxWidth);
                            offset += child.MinHeight;
                        }
                        else if (child.IsAbsolute())
                        {
                            child.Measure(new Size(availableSize.Width, Math.Max(child.GetAbsoluteValue() - delta, child.MinHeight)));
                            maxWidth = Math.Max(child.DesiredSize.Width, maxWidth);
                            offset += child.GetAbsoluteValue() - delta;
                        }
                        else
                        {
                            child.Measure(new Size(availableSize.Width, child.MinHeight));
                            maxWidth = Math.Max(child.DesiredSize.Width, maxWidth);
                            offset += child.MinWidth;
                        }
                    }
                }
                else
                {
                    double starsSum = childStars.Sum<FrameworkElement>(v => v.GetStarValue());
                    double starsFinalHeight =
                        availableSize.Height -
                        splitters.Sum<FrameworkElement>(s => s.MinHeight) -
                        childAbsolutes.Sum<FrameworkElement>(a => a.GetAbsoluteValue()) -
                        childAutoSizes.Sum<FrameworkElement>(a => a.DesiredSize.Height);

                    foreach (FrameworkElement child in visibleChildren)
                    {
                        if (child is Resizer)
                        {
                            child.Measure(new Size(availableSize.Width, child.MinHeight));
                            maxWidth = Math.Max(child.DesiredSize.Width, maxWidth);
                            offset += child.MinWidth;
                        }
                        else if (child.IsAbsolute())
                        {
                            child.Measure(new Size(availableSize.Width, child.GetAbsoluteValue()));
                            maxWidth = Math.Max(child.DesiredSize.Width, maxWidth);
                            offset += child.GetAbsoluteValue();
                        }
                        else if (child.IsStar())
                        {
                            double w = child.GetStarValue() / starsSum * starsFinalHeight;
                            child.Measure(new Size(availableSize.Width, w));
                            maxWidth = Math.Max(child.DesiredSize.Width, maxWidth);
                            offset += w;
                        }
                        else
                        {
                            child.Measure(new Size(availableSize.Width, child.DesiredSize.Height));
                            maxWidth = Math.Max(child.DesiredSize.Width, maxWidth);
                            offset += child.DesiredSize.Width;
                        }
                    }
                }

                return new Size(maxWidth, offset);

                #endregion

            }
        }

        private void NormalizeStarLength(IEnumerable<FrameworkElement> visibleChildren)
        {
            var childrenWithStarLength = visibleChildren.Where(c => c is IDockableControl && c.IsStar());
            int childrenWithStartLengthCount = childrenWithStarLength.Count();

            if (childrenWithStartLengthCount == 0)
                return;

            if (childrenWithStartLengthCount == 1)
            {
                ResizingPanel.SetResizeWidth(childrenWithStarLength.First(), new GridLength(1.0, GridUnitType.Star));
                return;
            }

            double sumStars = childrenWithStarLength.Sum(c => c.GetStarValue());

            if (sumStars == 0)
            {
                //problem!?! try to fix...
                childrenWithStarLength.ForEach(c =>
                    {
                        ResizingPanel.SetResizeWidth(c, new GridLength(1.0 / childrenWithStartLengthCount, GridUnitType.Star));
                    });
            }
            else
            {
                childrenWithStarLength.ForEach(c =>
                {
                    ResizingPanel.SetResizeWidth(c, new GridLength(1.0 * c.GetStarValue() / sumStars, GridUnitType.Star));
                });                
            }
            
        }

        Size[] _childrenFinalSizes = null;

        /// <summary>
        /// Arranges children giving them a proportional space according to their <see cref="SplitSize"/> attached property value
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            //Debug.WriteLine("ResizingPanel.ArrangeOverride()");

            //Compute the list of visible children
            List<FrameworkElement> visibleChildren = new List<FrameworkElement>();
            for (int i = 0; i < VisualChildrenCount; i++)
            {
                FrameworkElement child = GetVisualChild(i) as FrameworkElement;

                IDockableControl dockableControl = child as IDockableControl;
                if (dockableControl != null &&
                    !dockableControl.IsDocked)
                {
                    child.Arrange(new Rect());

                    if (i == VisualChildrenCount - 1 &&
                        i > 0)
                    {
                        child = GetVisualChild(i - 1) as FrameworkElement;
                        Debug.Assert(child is Resizer);

                        child.Arrange(new Rect());

                        if (visibleChildren.Count > 0)
                        {
                            Debug.Assert(visibleChildren[visibleChildren.Count - 1] is Resizer);
                            visibleChildren[visibleChildren.Count - 1].Arrange(new Rect());
                            visibleChildren.RemoveAt(visibleChildren.Count - 1);
                        }

                    }
                    else if (i < VisualChildrenCount - 1)
                    {
                        i++;
                        child = GetVisualChild(i) as FrameworkElement;
                        child.Arrange(new Rect());
                        Debug.Assert(child is Resizer);
                    }

                    continue;
                }

                visibleChildren.Add(child);
            }

            //with no children fill the space
            if (visibleChildren.Count == 0)
            {
                _childrenFinalSizes = new Size[] { };
                return new Size();
            }

            Debug.Assert(!(visibleChildren.Last<FrameworkElement>() is Resizer));
                

            _childrenFinalSizes = new Size[visibleChildren.Count];

            var splitters = from FrameworkElement child in visibleChildren
                            where child is Resizer
                            select child;
            var childStars = from FrameworkElement child in visibleChildren
                             where (!(child is Resizer)) && child.IsStar()
                             select child;

            var childAbsolutes = from FrameworkElement child in visibleChildren
                                 where (!(child is Resizer)) && child.IsAbsolute()
                                 select child;

            var childAutoSizes = from FrameworkElement child in visibleChildren
                                 where (!(child is Resizer)) && child.IsAuto()
                                 select child;

            //calculate the size of the splitters
            Size splitterSize = new Size();
            foreach (Resizer splitter in splitters)
            {
                splitterSize.Width += splitter.MinWidth;
                splitterSize.Height += splitter.MinHeight;
            }

            Size minimumSize = new Size(splitterSize.Width, splitterSize.Height);
            foreach (FrameworkElement child in childStars)
            {
                minimumSize.Width += child.MinWidth;
                minimumSize.Height += child.MinHeight;
            }
            foreach (FrameworkElement child in childAbsolutes)
            {
                minimumSize.Width += child.MinWidth;
                minimumSize.Height += child.MinHeight;
            }
            foreach (FrameworkElement child in childAutoSizes)
            {
                minimumSize.Width += child.MinWidth;
                minimumSize.Height += child.MinHeight;
            }


            Size minimumPrefferedSize = new Size(minimumSize.Width, minimumSize.Height);
            foreach (FrameworkElement child in childAbsolutes)
            {
                minimumPrefferedSize.Width += child.GetAbsoluteValue() - child.MinWidth;
                minimumPrefferedSize.Height += child.GetAbsoluteValue() - child.MinHeight;
            }
            foreach (FrameworkElement child in childAutoSizes)
            {
                minimumPrefferedSize.Width += child.DesiredSize.Width - child.MinWidth;
                minimumPrefferedSize.Height += child.DesiredSize.Height - child.MinHeight;
            }

            int iChild = 0;

            if (Orientation == Orientation.Horizontal)
            {
                #region Horizontal Orientation

                //if finalSize is not sufficient...
                if (minimumSize.Width >= finalSize.Width)
                {
                    foreach (FrameworkElement child in visibleChildren)
                    {
                        _childrenFinalSizes[iChild++] = new Size(child.MinWidth, finalSize.Height);
                    }
                }
                else if (minimumPrefferedSize.Width >= finalSize.Width)
                {
                    double delta = (minimumPrefferedSize.Width - finalSize.Width) / childAbsolutes.Count<FrameworkElement>();

                    foreach (FrameworkElement child in visibleChildren)
                    {
                        if (child is Resizer)
                            _childrenFinalSizes[iChild++] = new Size(child.MinWidth, finalSize.Height);
                        else if (child.IsAbsolute())
                            _childrenFinalSizes[iChild++] = new Size(Math.Max(child.GetAbsoluteValue() - delta, 0.0), finalSize.Height);
                        else
                            _childrenFinalSizes[iChild++] = new Size(child.MinWidth, finalSize.Height);
                    }
                }
                else
                {
                    double starsSum = childStars.Sum<FrameworkElement>(v => v.GetStarValue());
                    double starsFinalWidth =
                        finalSize.Width -
                        splitters.Sum<FrameworkElement>(s => s.MinWidth) -
                        childAbsolutes.Sum<FrameworkElement>(a => a.GetAbsoluteValue()) -
                        childAutoSizes.Sum<FrameworkElement>(a => a.DesiredSize.Width);

                    foreach (FrameworkElement child in visibleChildren)
                    {
                        if (child is Resizer)
                            _childrenFinalSizes[iChild++] = new Size(child.MinWidth, finalSize.Height);
                        else if (child.IsAbsolute())
                            _childrenFinalSizes[iChild++] = new Size(child.GetAbsoluteValue(), finalSize.Height);
                        else if (child.IsStar())
                            _childrenFinalSizes[iChild++] = new Size(child.GetStarValue() / starsSum * starsFinalWidth, finalSize.Height);
                        else
                            _childrenFinalSizes[iChild++] = new Size(child.DesiredSize.Width, finalSize.Height);
                    }

                }

                double offset = 0.0;

                for (int i = 0; i < visibleChildren.Count; i++)
                {
                    FrameworkElement child = visibleChildren[i] as FrameworkElement;
                    child.Arrange(new Rect(offset, 0.0, _childrenFinalSizes[i].Width, finalSize.Height));
                    offset += _childrenFinalSizes[i].Width;

                    SetEffectiveSize(child, new Size(_childrenFinalSizes[i].Width, finalSize.Height));
                }

                return new Size(offset, finalSize.Height);
                #endregion
            }
            else
            {
                #region Vertical Orientation

                //if finalSize is not sufficient...
                if (minimumSize.Height >= finalSize.Height)
                {
                    foreach (FrameworkElement child in visibleChildren)
                    {
                        _childrenFinalSizes[iChild++] = new Size(finalSize.Width, child.MinHeight);
                    }
                }
                else if (minimumPrefferedSize.Height >= finalSize.Height)
                {
                    double delta = (minimumPrefferedSize.Height - finalSize.Height) / childAbsolutes.Count<FrameworkElement>();

                    foreach (FrameworkElement child in visibleChildren)
                    {
                        if (child is Resizer)
                            _childrenFinalSizes[iChild++] = new Size(finalSize.Width, child.MinHeight);
                        else if (child.IsAbsolute())
                            _childrenFinalSizes[iChild++] = new Size(finalSize.Width, Math.Max(child.GetAbsoluteValue() - delta, 0.0));
                        else
                            _childrenFinalSizes[iChild++] = new Size(finalSize.Width, child.MinHeight);
                    }
                }
                else
                {
                    double starsSum = childStars.Sum<FrameworkElement>(v => v.GetStarValue());
                    double starsFinalHeight =
                        finalSize.Height -
                        splitters.Sum<FrameworkElement>(s => s.MinHeight) -
                        childAbsolutes.Sum<FrameworkElement>(a => a.GetAbsoluteValue()) -
                        childAutoSizes.Sum<FrameworkElement>(a => a.DesiredSize.Height);

                    foreach (FrameworkElement child in visibleChildren)
                    {
                        if (child is Resizer)
                            _childrenFinalSizes[iChild++] = new Size(finalSize.Width, child.MinHeight);
                        else if (child.IsAbsolute())
                            _childrenFinalSizes[iChild++] = new Size(finalSize.Width, child.GetAbsoluteValue());
                        else if (child.IsStar())
                            _childrenFinalSizes[iChild++] = new Size(finalSize.Width, child.GetStarValue() / starsSum * starsFinalHeight);
                        else
                            _childrenFinalSizes[iChild++] = new Size(finalSize.Width, child.DesiredSize.Height);
                    }

                }

                double offset = 0.0;

                for (int i = 0; i < visibleChildren.Count; i++)
                {
                    FrameworkElement child = visibleChildren[i] as FrameworkElement;
                    child.Arrange(new Rect(0.0, offset, finalSize.Width, _childrenFinalSizes[i].Height));
                    offset += _childrenFinalSizes[i].Height;
                    SetEffectiveSize(child, new Size(finalSize.Width, _childrenFinalSizes[i].Height));
                }

                return new Size(finalSize.Width, offset);
                #endregion
            }


        }

        bool setupSplitters = false;
        bool splitterListIsDirty = false;
 
        void SetupSplitters()
        {
            if (!splitterListIsDirty)
                return;

            if (setupSplitters)
                return;

            setupSplitters = true;

            while (_splitterList.Count > 0)
            {
                Resizer splitter = _splitterList[0];
                splitter.DragStarted -= new DragStartedEventHandler(splitter_DragStarted);
                splitter.DragDelta -= new DragDeltaEventHandler(splitter_DragDelta);
                splitter.DragCompleted -= new DragCompletedEventHandler(splitter_DragCompleted);
                _splitterList.Remove(splitter);
                Children.Remove(splitter);
            }

            int i = 0;//child index
            int j = 0;//splitter index

            while (i < Children.Count - 1)
            {
                if (j == _splitterList.Count)
                {
                    Resizer splitter = new Resizer();
                    splitter.Cursor =  this.Orientation == Orientation.Horizontal ? Cursors.SizeWE : Cursors.SizeNS;
                    _splitterList.Add(splitter);
                    splitter.DragStarted += new DragStartedEventHandler(splitter_DragStarted);
                    splitter.DragDelta += new DragDeltaEventHandler(splitter_DragDelta);
                    splitter.DragCompleted += new DragCompletedEventHandler(splitter_DragCompleted);
                    Children.Insert(i + 1, splitter);
                }

                i += 2;
                j++;
            }

            for (j = 0; j < _splitterList.Count; j++)
            {
                _splitterList[j].Width = (Orientation == Orientation.Horizontal) ? 6 : double.NaN;
                _splitterList[j].Height = (Orientation == Orientation.Vertical) ? 6 : double.NaN;
            }

#if DEBUG
            Debug.Assert(_splitterList.Count == Children.Count / 2);
            i = 0;
            while (Children.Count > 0)
            {
                Debug.Assert(Children[i] != null);
                Debug.Assert(!(Children[i] is Resizer));
                i++;
                if (i >= Children.Count)
                    break;

                Debug.Assert((Children[i] is Resizer));
                i++;

            }
#endif
            splitterListIsDirty = false;
            setupSplitters = false;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            splitterListIsDirty = true;
        }


        /// <summary>
        /// This method is called by a splitter when it is dragged
        /// </summary>
        /// <param name="splitter">Dragged splitter</param>
        /// <param name="delta"></param>
        void splitter_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Resizer splitter = sender as Resizer;

            //Point draggedPoint = this.PointToScreenDPI(
            //    new Point(e.HorizontalChange, e.VerticalChange));
            Window wnd = Window.GetWindow(this);
            var trToWnd = TransformToAncestor(wnd);
            Vector transformedDelta = trToWnd.Transform(new Point(e.HorizontalChange, e.VerticalChange)) -
                trToWnd.Transform(new Point());
            

            if (Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                Canvas.SetLeft(_resizerGhost, _initialStartPoint.X + transformedDelta.X);
            }
            else
            {
                Canvas.SetTop(_resizerGhost, _initialStartPoint.Y + transformedDelta.Y);
            }


            //ResizingPanelSplitter splitter = e.Source as ResizingPanelSplitter;
            //int i = 0;

            ////Compute the list of visible children
            //List<FrameworkElement> visibleChildren = new List<FrameworkElement>();
            //for (i = 0; i < VisualChildrenCount; i++)
            //{
            //    FrameworkElement child = GetVisualChild(i) as FrameworkElement;

            //    IDockableControl dockableControl = child as IDockableControl;
            //    if (dockableControl != null &&
            //        !dockableControl.IsDocked)
            //    {
            //        if (i == VisualChildrenCount - 1 &&
            //            i > 0)
            //        {
            //            //remove the last splitter added
            //            if (visibleChildren.Count > 0 &&
            //                visibleChildren.Last<FrameworkElement>() is ResizingPanelSplitter)
            //                visibleChildren.RemoveAt(visibleChildren.Count - 1);
            //        }
            //        else if (i < VisualChildrenCount - 1)
            //        {
            //            //discard the next splitter
            //            i++;
            //        }

            //        continue;
            //    }

            //    visibleChildren.Add(child);
            //}

            //if (visibleChildren.Count == 0)
            //    return;

            //if (visibleChildren.Last<FrameworkElement>() is ResizingPanelSplitter)
            //    visibleChildren.RemoveAt(visibleChildren.Count - 1);

            //Size[] currentSizes = new Size[visibleChildren.Count];
            //double delta = Orientation == Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange;

            //if (_childrenFinalSizes == null)
            //    return;

            //_childrenFinalSizes.CopyTo(currentSizes, 0);

            //int iSplitter = visibleChildren.IndexOf(splitter);

            //Debug.Assert(iSplitter > -1);

            //List<FrameworkElement> prevChildren = new List<FrameworkElement>();
            //for (i = iSplitter - 1; i >= 0; i--)
            //{
            //    FrameworkElement child = visibleChildren[i] as FrameworkElement;
            //    if (child is ResizingPanelSplitter)
            //        continue;
            //    if (child.IsAbsolute() || child.IsAuto())
            //    {
            //        if (prevChildren.Count == 0)
            //        {
            //            prevChildren.Add(child);
            //        }
            //        break;
            //    }
            //    if (child.IsStar())
            //    {
            //        prevChildren.Add(child);
            //    }
            //}

            //List<FrameworkElement> nextChildren = new List<FrameworkElement>();

            //for (i = iSplitter + 1; i < visibleChildren.Count; i++)
            //{
            //    FrameworkElement child = visibleChildren[i] as FrameworkElement;
            //    if (child is ResizingPanelSplitter)
            //        continue;
            //    if (child.IsAbsolute() || child.IsAuto())
            //    {
            //        if (nextChildren.Count == 0)
            //            nextChildren.Add(child);
            //        break;
            //    }
            //    if (child.IsStar())
            //    {
            //        nextChildren.Add(child);
            //    }
            //}


            //double prevMinSize = prevChildren.Sum<FrameworkElement>(c => Orientation == Orientation.Horizontal ? c.MinWidth : c.MinHeight);
            //double nextMinSize = nextChildren.Sum<FrameworkElement>(c => Orientation == Orientation.Horizontal ? c.MinWidth : c.MinHeight);
            //double prevMaxSize = prevChildren.Sum<FrameworkElement>(c => Orientation == Orientation.Horizontal ? c.MaxWidth : c.MaxHeight);
            //double nextMaxSize = nextChildren.Sum<FrameworkElement>(c => Orientation == Orientation.Horizontal ? c.MaxWidth : c.MaxHeight);

            //double prevSize = prevChildren.Sum<FrameworkElement>(c => Orientation == Orientation.Horizontal ? currentSizes[visibleChildren.IndexOf(c)].Width : currentSizes[visibleChildren.IndexOf(c)].Height);
            //double nextSize = nextChildren.Sum<FrameworkElement>(c => Orientation == Orientation.Horizontal ? currentSizes[visibleChildren.IndexOf(c)].Width : currentSizes[visibleChildren.IndexOf(c)].Height);

            //if (prevSize + delta < prevMinSize)
            //    delta = prevMinSize - prevSize;
            //if (nextSize - delta < nextMinSize)
            //    delta = -(nextMinSize - nextSize);

            //double remDelta = delta * 2;

            //while (!HelperFunc.AreClose(delta, 0.0))
            //{
            //    int prevChildrenCountWithNoMinLen =
            //        prevChildren.Count<FrameworkElement>(c => delta > 0 ? true : (Orientation == Orientation.Horizontal ? currentSizes[visibleChildren.IndexOf(c)].Width > c.MinWidth : currentSizes[visibleChildren.IndexOf(c)].Height > c.MinHeight));
            //    int nextChildrenCountWithNoMinLen =
            //        nextChildren.Count<FrameworkElement>(c => delta < 0 ? true : (Orientation == Orientation.Horizontal ? currentSizes[visibleChildren.IndexOf(c)].Width > c.MinWidth : currentSizes[visibleChildren.IndexOf(c)].Height > c.MinHeight));
                
            //    delta = remDelta / 2.0;

            //    for (i = 0; i < currentSizes.Length; i++)
            //    {
            //        FrameworkElement child = visibleChildren[i] as FrameworkElement;
            //        if (child is ResizingPanelSplitter)
            //            continue;

            //        if (Orientation == Orientation.Horizontal)
            //        {
            //            if (prevChildren.Contains(child) && prevChildrenCountWithNoMinLen > 0)
            //            {
            //                double s = delta / prevChildrenCountWithNoMinLen;
            //                if (currentSizes[i].Width + s < child.MinWidth)
            //                    s = child.MinWidth - currentSizes[i].Width;
                            
            //                currentSizes[i].Width += s;
            //                remDelta -= s;
            //            }
            //            if (nextChildren.Contains(child) && nextChildrenCountWithNoMinLen > 0)
            //            {
            //                double s = delta / nextChildrenCountWithNoMinLen;
            //                if (currentSizes[i].Width - s < child.MinWidth)
            //                    s = currentSizes[i].Width - child.MinWidth;

            //                currentSizes[i].Width -= s;
            //                remDelta -= s;
            //            }
            //        }
            //        else
            //        {
            //            if (prevChildren.Contains(child) && prevChildrenCountWithNoMinLen > 0)
            //            {
            //                double s = delta / prevChildrenCountWithNoMinLen;
            //                if (currentSizes[i].Height + s < child.MinHeight)
            //                    s = child.MinHeight - currentSizes[i].Height;

            //                currentSizes[i].Height += s;
            //                remDelta -= s;
            //            }
            //            if (nextChildren.Contains(child) && nextChildrenCountWithNoMinLen > 0)
            //            {
            //                double s = delta / nextChildrenCountWithNoMinLen;
            //                if (currentSizes[i].Height - s < child.MinHeight)
            //                    s = currentSizes[i].Height - child.MinHeight;

            //                currentSizes[i].Height -= s;
            //                remDelta -= s;
            //            }
            //        }
            //    }
            //}

            //Debug.Assert(HelperFunc.AreClose(delta, 0.0));

            //double totalStartsSum = 0.0;
            //double totalSizeForStarts = 0.0;

            //for (i = 0; i < visibleChildren.Count; i++)
            //{ 
            //    FrameworkElement child = visibleChildren[i] as FrameworkElement;
            //    if (child is ResizingPanelSplitter)
            //        continue;
            //    if (child.IsStar())
            //    {
            //        totalStartsSum += child.GetStarValue();
            //        totalSizeForStarts += Orientation == Orientation.Horizontal ? currentSizes[i].Width : currentSizes[i].Height;
            //    }
            //}
                

            //double starsScaleFactor = totalStartsSum / totalSizeForStarts;

            //for (i = 0; i < currentSizes.Length; i++)
            //{
            //    FrameworkElement child = visibleChildren[i] as FrameworkElement;

            //    if (child is ResizingPanelSplitter)
            //        continue;

            //    if (child.IsStar())
            //    {
            //        if (Orientation == Orientation.Horizontal)
            //        {
            //            SetResizeWidth(child,
            //                new GridLength(HelperFunc.MultiplyCheckNaN(currentSizes[i].Width, starsScaleFactor), GridUnitType.Star));
            //        }
            //        else
            //        {
            //            SetResizeHeight(child,
            //                new GridLength(HelperFunc.MultiplyCheckNaN(currentSizes[i].Height, starsScaleFactor), GridUnitType.Star));
            //        }
            //    }
            //    else if (child.IsAbsolute())
            //    {
            //        if (Orientation == Orientation.Horizontal)
            //        {
            //            SetResizeWidth(child,
            //                new GridLength(currentSizes[i].Width, GridUnitType.Pixel));
            //        }
            //        else
            //        {
            //            SetResizeHeight(child,
            //                new GridLength(currentSizes[i].Height, GridUnitType.Pixel)); 
            //        }
            //    }
            //}

            //InvalidateMeasure();
        }

        void splitter_DragStarted(object sender, DragStartedEventArgs e)
        {
            var resizer = sender as Resizer;
            ShowResizerOverlayWindow(resizer);
        }

        void splitter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            HideResizerOverlayWindow();

            Resizer splitter = e.Source as Resizer;
            int i = 0;

            //Compute the list of visible children
            List<FrameworkElement> visibleChildren = new List<FrameworkElement>();
            for (i = 0; i < VisualChildrenCount; i++)
            {
                FrameworkElement child = GetVisualChild(i) as FrameworkElement;

                IDockableControl dockableControl = child as IDockableControl;
                if (dockableControl != null &&
                    !dockableControl.IsDocked)
                {
                    if (i == VisualChildrenCount - 1 &&
                        i > 0)
                    {
                        //remove the last splitter added
                        if (visibleChildren.Count > 0 &&
                            visibleChildren.Last<FrameworkElement>() is Resizer)
                            visibleChildren.RemoveAt(visibleChildren.Count - 1);
                    }
                    else if (i < VisualChildrenCount - 1)
                    {
                        //discard the next splitter
                        i++;
                    }

                    continue;
                }

                visibleChildren.Add(child);
            }

            if (visibleChildren.Count == 0)
                return;

            if (visibleChildren.Last<FrameworkElement>() is Resizer)
                visibleChildren.RemoveAt(visibleChildren.Count - 1);

            Size[] currentSizes = new Size[visibleChildren.Count];
            Window wnd = Window.GetWindow(this);
            var trToWnd = TransformToAncestor(wnd).Inverse;
            Vector transformedDelta = trToWnd.Transform(new Point(e.HorizontalChange, e.VerticalChange)) -
                trToWnd.Transform(new Point());
            double delta = Orientation == Orientation.Horizontal ? transformedDelta.X : transformedDelta.Y;

            if (_childrenFinalSizes == null)
                return;

            _childrenFinalSizes.CopyTo(currentSizes, 0);

            int iSplitter = visibleChildren.IndexOf(splitter);

            Debug.Assert(iSplitter > -1);

            List<FrameworkElement> prevChildren = new List<FrameworkElement>();
            for (i = iSplitter - 1; i >= 0; i--)
            {
                FrameworkElement child = visibleChildren[i] as FrameworkElement;
                if (child is Resizer)
                    continue;
                if (child.IsAbsolute() || child.IsAuto())
                {
                    if (prevChildren.Count == 0)
                    {
                        prevChildren.Add(child);
                    }
                    break;
                }
                if (child.IsStar())
                {
                    prevChildren.Add(child);
                }
            }

            List<FrameworkElement> nextChildren = new List<FrameworkElement>();

            for (i = iSplitter + 1; i < visibleChildren.Count; i++)
            {
                FrameworkElement child = visibleChildren[i] as FrameworkElement;
                if (child is Resizer)
                    continue;
                if (child.IsAbsolute() || child.IsAuto())
                {
                    if (nextChildren.Count == 0)
                        nextChildren.Add(child);
                    break;
                }
                if (child.IsStar())
                {
                    nextChildren.Add(child);
                }
            }


            double prevMinSize = prevChildren.Sum<FrameworkElement>(c => Orientation == Orientation.Horizontal ? c.MinWidth : c.MinHeight);
            double nextMinSize = nextChildren.Sum<FrameworkElement>(c => Orientation == Orientation.Horizontal ? c.MinWidth : c.MinHeight);
            double prevMaxSize = prevChildren.Sum<FrameworkElement>(c => Orientation == Orientation.Horizontal ? c.MaxWidth : c.MaxHeight);
            double nextMaxSize = nextChildren.Sum<FrameworkElement>(c => Orientation == Orientation.Horizontal ? c.MaxWidth : c.MaxHeight);

            double prevSize = prevChildren.Sum<FrameworkElement>(c => Orientation == Orientation.Horizontal ? currentSizes[visibleChildren.IndexOf(c)].Width : currentSizes[visibleChildren.IndexOf(c)].Height);
            double nextSize = nextChildren.Sum<FrameworkElement>(c => Orientation == Orientation.Horizontal ? currentSizes[visibleChildren.IndexOf(c)].Width : currentSizes[visibleChildren.IndexOf(c)].Height);

            if (prevSize + delta < prevMinSize)
                delta = prevMinSize - prevSize;
            if (nextSize - delta < nextMinSize)
                delta = -(nextMinSize - nextSize);

            double remDelta = delta * 2;

            while (!HelperFunc.AreClose(delta, 0.0))
            {
                int prevChildrenCountWithNoMinLen =
                    prevChildren.Count<FrameworkElement>(c => delta > 0 ? true : (Orientation == Orientation.Horizontal ? currentSizes[visibleChildren.IndexOf(c)].Width > c.MinWidth : currentSizes[visibleChildren.IndexOf(c)].Height > c.MinHeight));
                int nextChildrenCountWithNoMinLen =
                    nextChildren.Count<FrameworkElement>(c => delta < 0 ? true : (Orientation == Orientation.Horizontal ? currentSizes[visibleChildren.IndexOf(c)].Width > c.MinWidth : currentSizes[visibleChildren.IndexOf(c)].Height > c.MinHeight));

                delta = remDelta / 2.0;

                for (i = 0; i < currentSizes.Length; i++)
                {
                    FrameworkElement child = visibleChildren[i] as FrameworkElement;
                    if (child is Resizer)
                        continue;

                    if (Orientation == Orientation.Horizontal)
                    {
                        if (prevChildren.Contains(child) && prevChildrenCountWithNoMinLen > 0)
                        {
                            double s = delta / prevChildrenCountWithNoMinLen;
                            if (currentSizes[i].Width + s < child.MinWidth)
                                s = child.MinWidth - currentSizes[i].Width;

                            currentSizes[i].Width += s;
                            remDelta -= s;
                        }
                        if (nextChildren.Contains(child) && nextChildrenCountWithNoMinLen > 0)
                        {
                            double s = delta / nextChildrenCountWithNoMinLen;
                            if (currentSizes[i].Width - s < child.MinWidth)
                                s = currentSizes[i].Width - child.MinWidth;

                            currentSizes[i].Width -= s;
                            remDelta -= s;
                        }
                    }
                    else
                    {
                        if (prevChildren.Contains(child) && prevChildrenCountWithNoMinLen > 0)
                        {
                            double s = delta / prevChildrenCountWithNoMinLen;
                            if (currentSizes[i].Height + s < child.MinHeight)
                                s = child.MinHeight - currentSizes[i].Height;

                            currentSizes[i].Height += s;
                            remDelta -= s;
                        }
                        if (nextChildren.Contains(child) && nextChildrenCountWithNoMinLen > 0)
                        {
                            double s = delta / nextChildrenCountWithNoMinLen;
                            if (currentSizes[i].Height - s < child.MinHeight)
                                s = currentSizes[i].Height - child.MinHeight;

                            currentSizes[i].Height -= s;
                            remDelta -= s;
                        }
                    }
                }
            }

            Debug.Assert(HelperFunc.AreClose(delta, 0.0));

            double totalStartsSum = 0.0;
            double totalSizeForStarts = 0.0;

            for (i = 0; i < visibleChildren.Count; i++)
            {
                FrameworkElement child = visibleChildren[i] as FrameworkElement;
                if (child is Resizer)
                    continue;
                if (child.IsStar())
                {
                    totalStartsSum += child.GetStarValue();
                    totalSizeForStarts += Orientation == Orientation.Horizontal ? currentSizes[i].Width : currentSizes[i].Height;
                }
            }

            double starsScaleFactor = totalStartsSum / totalSizeForStarts;

            for (i = 0; i < currentSizes.Length; i++)
            {
                FrameworkElement child = visibleChildren[i] as FrameworkElement;

                if (child is Resizer)
                    continue;

                if (child.IsStar())
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        SetResizeWidth(child,
                            new GridLength(HelperFunc.MultiplyCheckNaN(currentSizes[i].Width, starsScaleFactor), GridUnitType.Star));
                    }
                    else
                    {
                        SetResizeHeight(child,
                            new GridLength(HelperFunc.MultiplyCheckNaN(currentSizes[i].Height, starsScaleFactor), GridUnitType.Star));
                    }
                }
                else if (child.IsAbsolute())
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        SetResizeWidth(child,
                            new GridLength(currentSizes[i].Width, GridUnitType.Pixel));
                    }
                    else
                    {
                        SetResizeHeight(child,
                            new GridLength(currentSizes[i].Height, GridUnitType.Pixel));
                    }
                }
            }

            InvalidateMeasure();
        }

        Border _resizerGhost = null;
        Window _resizerWindowHost = null;
        Vector _initialStartPoint;

        void ShowResizerOverlayWindow(Resizer splitter)
        { 
            Point ptTopLeftScreen = this.PointToScreenDPI(new Point());

            _resizerGhost = new Border()
            {
                Background = Brushes.Black,
                Opacity = 0.7
            };

            Size actualSize = this.TransformedActualSize();

            if (Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                _resizerGhost.Width = 5.0;
                _resizerGhost.Height = actualSize.Height;
            }
            else
            {
                _resizerGhost.Height = 5.0;
                _resizerGhost.Width = actualSize.Width;
            }

            _initialStartPoint = splitter.PointToScreenDPI(new Point()) - this.PointToScreenDPI(new Point());

            if (Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                Canvas.SetLeft(_resizerGhost, _initialStartPoint.X);
            }
            else
            {
                Canvas.SetTop(_resizerGhost, _initialStartPoint.Y);
            }

            Canvas panelHostResizer = new Canvas() 
            { 
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch
            };

            panelHostResizer.Children.Add(_resizerGhost);
            
            
            _resizerWindowHost = new Window()
            {
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = System.Windows.WindowStyle.None,
                ShowInTaskbar = false,
                AllowsTransparency = true,
                Background = null,
                Width = actualSize.Width,
                Height = actualSize.Height,
                Left = ptTopLeftScreen.X,
                Top = ptTopLeftScreen.Y,
                ShowActivated = false,
                Owner = Window.GetWindow(this),
                Content = panelHostResizer
                //,
                //LayoutTransform = (MatrixTransform)this.TansformToAncestor()
            };

            _resizerWindowHost.Show();
        }

        void HideResizerOverlayWindow()
        {
            if (_resizerWindowHost != null)
            {
                _resizerWindowHost.Close();
                _resizerWindowHost = null;
            }
        }

        #region IDockableControl Membri di

        public bool IsDocked
        {
            get
            {
                foreach (UIElement child in this.Children)
                {
                    if (child is IDockableControl)
                        if (((IDockableControl)child).IsDocked)
                            return true;
                }

                return false;
            }
        }

        #endregion


        /// <summary>
        /// Remove a child from children collection
        /// </summary>
        /// <param name="childToRemove"></param>
        internal void RemoveChild(FrameworkElement childToRemove)
        {
            int indexOfChildToRemove = Children.IndexOf(childToRemove);

            Debug.Assert(indexOfChildToRemove != -1);

            Children.RemoveAt(indexOfChildToRemove);

            if (Children.Count > 0)
            {
                SetupSplitters();

                if (Children.Count == 1)
                {
                    UIElement singleChild = this.Children[0];

                    if (Parent is ResizingPanel)
                    {
                        ResizingPanel parentPanel = Parent as ResizingPanel;
                        if (parentPanel != null)
                        {
                            int indexOfThisPanel = parentPanel.Children.IndexOf(this);
                            parentPanel.Children.RemoveAt(indexOfThisPanel);
                            this.Children.Remove(singleChild);
                            parentPanel.Children.Insert(indexOfThisPanel, singleChild);

                            if (parentPanel.Orientation == Orientation.Horizontal)
                            {
                                SetResizeWidth(singleChild, GetResizeWidth(this));
                            }
                            else
                            {
                                SetResizeHeight(singleChild, GetResizeHeight(this));
                            }
                        }
                    }
                    else if (Parent is DockingManager)
                    {
                        DockingManager manager = Parent as DockingManager;
                        if (manager != null)
                        {
                            this.Children.Remove(singleChild);
                            manager.Content = singleChild;
                        }
                    }

                }
            }
            else
            {
                ResizingPanel parentPanel = Parent as ResizingPanel;
                if (parentPanel != null)
                {
                    parentPanel.RemoveChild(this);
                }
            }
        }

        /// <summary>
        /// Insert a new child element into the children collection.
        /// </summary>
        /// <param name="childToInsert">New child element to insert.</param>
        /// <param name="relativeChild">Child after or before which <see cref="childToInsert"/> element must be insert.</param>
        /// <param name="next">True if new child must be insert after the <see cref="relativeChild"/> element. False otherwise.</param>
        internal void InsertChildRelativeTo(FrameworkElement childToInsert, FrameworkElement relativeChild, bool next)
        {
            int childRelativeIndex = Children.IndexOf(relativeChild);

            Debug.Assert(childRelativeIndex != -1);

            Children.Insert(
                next ? childRelativeIndex + 1 : childRelativeIndex, childToInsert);

            SetupSplitters();

            InvalidateMeasure();
        }

        


    }


    internal static class ResizingPanelExFuncs
    {
        public static double GetAbsoluteValue(this FrameworkElement child)
        {
            ResizingPanel parentPanel = LogicalTreeHelper.GetParent(child) as ResizingPanel;
            GridLength len = parentPanel.Orientation == Orientation.Horizontal ? ResizingPanel.GetResizeWidth(child) : ResizingPanel.GetResizeHeight(child);
            if (!len.IsAbsolute)
                throw new InvalidOperationException();
            return len.Value;
        }

        public static double GetStarValue(this FrameworkElement child)
        {
            ResizingPanel parentPanel = LogicalTreeHelper.GetParent(child) as ResizingPanel;
            GridLength len = parentPanel.Orientation == Orientation.Horizontal ? ResizingPanel.GetResizeWidth(child) : ResizingPanel.GetResizeHeight(child);
            if (!len.IsStar)
                throw new InvalidOperationException();
            return len.Value;
        }

        public static bool IsStar(this FrameworkElement child)
        {
            ResizingPanel parentPanel = LogicalTreeHelper.GetParent(child) as ResizingPanel;
            return parentPanel.Orientation == Orientation.Horizontal ?
                ResizingPanel.GetResizeWidth(child).IsStar :
                ResizingPanel.GetResizeHeight(child).IsStar;
        }
        public static bool IsAbsolute(this FrameworkElement child)
        {
            ResizingPanel parentPanel = LogicalTreeHelper.GetParent(child) as ResizingPanel;
            return parentPanel.Orientation == Orientation.Horizontal ?
                ResizingPanel.GetResizeWidth(child).IsAbsolute :
                ResizingPanel.GetResizeHeight(child).IsAbsolute;
        }

        public static bool IsAuto(this FrameworkElement child)
        {
            ResizingPanel parentPanel = LogicalTreeHelper.GetParent(child) as ResizingPanel;
            return parentPanel.Orientation == Orientation.Horizontal ?
                ResizingPanel.GetResizeWidth(child).IsAuto :
                ResizingPanel.GetResizeHeight(child).IsAuto;
        }

    }

}
