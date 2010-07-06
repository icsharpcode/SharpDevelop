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
using System.ComponentModel;
using System.Xml;
using System.Windows.Forms.Integration;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;
using System.Reflection;
using System.Net.Cache;


namespace AvalonDock
{
    
    public abstract class ManagedContent : ContentControl, INotifyPropertyChanged
    {
        static ManagedContent()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ManagedContent), new FrameworkPropertyMetadata(typeof(ManagedContent)));

            WidthProperty.OverrideMetadata(typeof(ManagedContent), new FrameworkPropertyMetadata(double.NaN, null, new CoerceValueCallback(
                (s, v) =>
                {
                    if (!DesignerProperties.GetIsInDesignMode(s as DependencyObject))
                        return double.NaN;

                    return v;
                })));
            HeightProperty.OverrideMetadata(typeof(ManagedContent), new FrameworkPropertyMetadata(double.NaN, null, new CoerceValueCallback(
                (s, v) =>
                {
                    if (!DesignerProperties.GetIsInDesignMode(s as DependencyObject))
                        return double.NaN;

                    return v;
                })));

            FocusableProperty.OverrideMetadata(typeof(ManagedContent), new FrameworkPropertyMetadata(true));
        }

        public ManagedContent()
        {
            this.Loaded += new RoutedEventHandler(ManagedContent_Loaded);
            this.Unloaded += new RoutedEventHandler(ManagedContent_Unloaded);
        }

        //WindowsFormsHost GetWinFormsHost()
        //{
        //    WindowsFormsHost contentHost = null;

        //    if (this.Content is UserControl)
        //    {
        //        UserControl usTemp = this.Content as UserControl;

        //        if (usTemp.Content is WindowsFormsHost)
        //            contentHost = usTemp.Content as WindowsFormsHost;
        //    }
        //    else if (this.Content is WindowsFormsHost)
        //    {
        //        contentHost = this.Content as WindowsFormsHost;
        //    }

        //    return contentHost;
        //}

        //void ManagedContent_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
            //WindowsFormsHost contentHost = GetWinFormsHost();

            //if (contentHost != null)
            //{
            //    object childCtrl = contentHost.GetType().GetProperty("Child").GetValue(contentHost, null);

            //    if (childCtrl != null)
            //    {
            //        this.Dispatcher.Invoke(new Action<object>((o) => o.CallMethod("Refresh", null)), DispatcherPriority.Render, childCtrl);
            //    }
            //}
        //}

        protected virtual void OnContentLoaded()
        {
            RaisePropertyChanged("ContainerPane");
        }

        protected virtual void OnContentUnloaded()
        {
            RaisePropertyChanged("ContainerPane");
        }

		void ManagedContent_Loaded(object sender, RoutedEventArgs e)
		{
            OnContentLoaded();

            //WindowsFormsHost contentHost = GetWinFormsHost();

            //if (contentHost != null)
            //{
            //    contentHost.SizeChanged += new SizeChangedEventHandler(ManagedContent_SizeChanged);
            //}
		}

		void ManagedContent_Unloaded(object sender, RoutedEventArgs e)
		{
            OnContentUnloaded();

            //WindowsFormsHost contentHost = GetWinFormsHost();

            //if (contentHost != null)
            //{
            //    contentHost.SizeChanged -= new SizeChangedEventHandler(ManagedContent_SizeChanged);
            //}
		}

        #region Title

        /// <summary>
        /// Gets or set the title of the content
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ManagedContent));

        #endregion
        
        #region Icon

        /// <summary>
        /// Access to <see cref="IconProperty"/> dependency property
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Select an icon object for the content
        /// </summary>
        public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ManagedContent),
            new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerce_Icon)));

        private static object OnCoerce_Icon(DependencyObject o, object value)
        {
            //if (value is string)
            //{
            //    Uri iconUri;
            //    //// try to resolve given value as an absolute URI
            //    if (Uri.TryCreate(value as String, UriKind.Relative, out iconUri))
            //    {
            //        ImageSource img = new BitmapImage(iconUri);
            //        if (img != null)
            //            return img;//new Image() { Source = img };

            //        //GreyableImage seems to be not compatible with .net 4
            //        //if (null != img)
            //        //{
            //        //    GreyableImage icon = (o as ManagedContent).Icon as GreyableImage;
            //        //    if (null == icon)
            //        //        icon = new GreyableImage();

            //        //    icon.Source = img;
            //        //    //icon.Stretch = Stretch.None;
            //        //    //icon.SnapsToDevicePixels = true;

            //        //    return icon;
            //        //}
            //    }
            //}
            return value;
        }

        #endregion

        #region DefaultElement

        /// <summary>
        /// Access to <see cref="DefaultFocusedElementProperty"/>
        /// </summary>
        public IInputElement DefaultElement
        {

            get { return (IInputElement)GetValue(DefaultFocusedElementProperty); }

            set { SetValue(DefaultFocusedElementProperty, value); }

        }

        /// <summary>
        /// Gets or sets an element which is focused by default when content is activated
        /// </summary>
        public static readonly DependencyProperty DefaultFocusedElementProperty = DependencyProperty.Register("DefaultElement", typeof(IInputElement), typeof(ManagedContent));

        #endregion

        FrameworkElement _dragEnabledArea;

        /// <summary>
        /// Gets the draggable area of the document
        /// </summary>
        protected FrameworkElement DragEnabledArea
        {
            get { return _dragEnabledArea; }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _dragEnabledArea = GetTemplateChild("PART_DragArea") as FrameworkElement;

            if (_dragEnabledArea != null)
            {
                _dragEnabledArea.MouseDown += new MouseButtonEventHandler(OnDragMouseDown);
                _dragEnabledArea.MouseMove += new MouseEventHandler(OnDragMouseMove);
                _dragEnabledArea.MouseUp += new MouseButtonEventHandler(OnDragMouseUp);
                _dragEnabledArea.MouseLeave += new MouseEventHandler(OnDragMouseLeave);
            }

            if (_dragEnabledArea != null)
                _dragEnabledArea.InputBindings.Add(new InputBinding(ManagedContentCommands.Close, new MouseGesture(MouseAction.MiddleClick)));

            if (_dragEnabledArea != null && _dragEnabledArea.ContextMenu == null)
            {
                _dragEnabledArea.MouseRightButtonDown += (s, e) =>
                    {
                        if (!e.Handled)
                        {
                            Activate();
                            if (_dragEnabledArea.ContextMenu == null)
                            {
                                Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(delegate
                                {
                                    ContainerPane.OpenOptionsMenu(null);
                                }));
                            }
                            e.Handled = true;
                        }
                    };
            }
        }

        #region Mouse management

        protected virtual void OnDragStart(Point ptMouse, Point ptrelativeMouse)
        {
        
        }

        Point ptStartDrag;
        bool isMouseDown = false;

        protected Point StartDragPoint
        {
            get { return ptStartDrag; }
        }

        protected bool IsMouseDown
        {
            get { return isMouseDown; }
        }

        protected void ResetIsMouseDownFlag()
        {
            isMouseDown = false;
        }

        protected virtual void OnDragMouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("OnDragMouseDown" + e.ClickCount);
            if (!e.Handled && Manager != null)// && State != DockableContentState.AutoHide)
            {
                isMouseDown = true;
                ptStartDrag = e.GetPosition((IInputElement)System.Windows.Media.VisualTreeHelper.GetParent(this));
            }
        }

        protected virtual void OnDragMouseMove(object sender, MouseEventArgs e)
        {
        }

        protected virtual void OnDragMouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;

            Debug.WriteLine("OnDragMouseUp" + e.ClickCount);
        }

        Point ptRelativePosition;

        protected virtual void OnDragMouseLeave(object sender, MouseEventArgs e)
        {
            if (!e.Handled && isMouseDown && e.LeftButton == MouseButtonState.Pressed && Manager != null)
            {
                if (!IsMouseCaptured)
                {
                    Point ptMouseMove = e.GetPosition((IInputElement)System.Windows.Media.VisualTreeHelper.GetParent(this));
                    ManagedContent contentToSwap = null;
                    if (ContainerPane != null)
                    {
                        foreach (ManagedContent content in ContainerPane.Items)
                        {
                            if (content == this)
                                continue;

                            HitTestResult res = VisualTreeHelper.HitTest(content, e.GetPosition(content));
                            if (res != null)
                            {
                                contentToSwap = content;
                                break;
                            }
                        }
                    }

                    if (contentToSwap != null &&
                        contentToSwap != this)
                    {
                        Pane containerPane = ContainerPane;
                        int myIndex = containerPane.Items.IndexOf(this);

                        ContainerPane.Items.RemoveAt(myIndex);

                        int otherIndex = containerPane.Items.IndexOf(contentToSwap);
                        containerPane.Items.RemoveAt(otherIndex);

                        containerPane.Items.Insert(otherIndex, this);

                        containerPane.Items.Insert(myIndex, contentToSwap);

                        containerPane.SelectedItem = this;
                        
                        e.Handled = false;
                        //avoid ismouseDown = false call
                        return;
                    }
                    else if (Math.Abs(ptMouseMove.X - StartDragPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(ptMouseMove.Y - StartDragPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        ptRelativePosition = e.GetPosition(DragEnabledArea);

                        ResetIsMouseDownFlag();
                        OnDragStart(StartDragPoint, ptRelativePosition);
                        e.Handled = true;
                    }
                }
            }

            ResetIsMouseDownFlag();
        }


        #endregion

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (!e.Handled)
            {
                Activate();
                //FocusManager.SetFocusedElement(Content as DependencyObject, DefaultElement);
                //IInputElement focusedElement = e.Source as IInputElement;
                //if (focusedElement != null) Keyboard.Focus(focusedElement);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ContainerPane != null)
                    ContainerPane.SelectedItem = this;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Gets container pane currently hosting the content
        /// </summary>
        /// <remarks>Please note that this value could change as user move the content around the <see cref="DockingManager"/>.</remarks>
        public Pane ContainerPane
        {
            get 
            {
                Pane containerPane = Parent as Pane;
                if (containerPane != null)
                    return containerPane;

                return this.FindVisualAncestor<Pane>(false);
            }
        }

        /// <summary>
        /// Remove this content from its parent container pane
        /// </summary>
        /// <returns></returns>
        internal virtual ManagedContent DetachFromContainerPane()
        {
            if (ContainerPane != null)
            {
                int indexOfContent = ContainerPane.Items.IndexOf(this);
                return ContainerPane.RemoveContent(indexOfContent) as ManagedContent;
            }

            return null;
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            RaisePropertyChanged("ContainerPane");
            base.OnVisualParentChanged(oldParent);
        }

        #region Manager

        private DockingManager _manager = null;
        
        /// <summary>
        /// Get current hosting docking manager (<see cref="DockingManager"/>)
        /// </summary>
        public DockingManager Manager
        {
            get { return _manager; }
            internal set
            {
                if (_manager != value)
                {
                    DockingManager oldValue = _manager;
                    _manager = value;
                    OnManagerChanged(oldValue, value);
                    RaisePropertyChanged("Manager");
                }
            }
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Manager property.
        /// </summary>
        protected virtual void OnManagerChanged(DockingManager oldValue, DockingManager newValue)
        {
        }

        #endregion

        protected DockingManager GetParentManager(Pane containerPane)
        {
            if (containerPane == null)
                containerPane = ContainerPane;

            if (containerPane != null)
                return ContainerPane.GetManager();

            return null;
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            Debug.WriteLine(string.Format("[{0}].OnGotKeyboardFocus() Source={1} NewFocus={2} OldFocus={3}", this.Name, e.Source.GetType().ToString(), e.NewFocus.GetType().ToString(), e.OldFocus == null ? "<null>" : e.OldFocus.GetType().ToString()));

            if (Manager != null &&
                this.IsKeyboardFocusWithin)
            {
                Manager.ActiveContent = this;
            }
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            Debug.WriteLine(string.Format("[{0}].OnLostKeyboardFocus() Source={1} NewFocus={2} OldFocus={3}", this.Name, e.Source.GetType().ToString(), e.NewFocus == null ? "<null>" : e.NewFocus.GetType().ToString(), e.OldFocus == null ? "<null>" : e.OldFocus.GetType().ToString()));
            base.OnLostKeyboardFocus(e);
        }

        #region IsActiveContent

        /// <summary>
        /// IsActiveContent Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey IsActiveContentPropertyKey
            = DependencyProperty.RegisterReadOnly("IsActiveContent", typeof(bool), typeof(ManagedContent),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnIsActiveContentChanged)));

        public static readonly DependencyProperty IsActiveContentProperty
            = IsActiveContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the IsActiveContent property.  This dependency property 
        /// indicates the active (selected) content between all contents of the docking manager
        /// </summary>
        public bool IsActiveContent
        {
            get { return (bool)GetValue(IsActiveContentProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the IsActiveContent property.  
        /// This dependency property indicates the current content is the active content between all docking manager contents
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        internal void SetIsActiveContent(bool value)
        {
            SetValue(IsActiveContentPropertyKey, value);
        }


        /// <summary>
        /// Handles changes to the IsActiveContent property.
        /// </summary>
        private static void OnIsActiveContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ManagedContent)d).OnIsActiveContentChanged(e);
        }

        DateTime _lastActivation = DateTime.MinValue;

        internal DateTime LastActivation
        {
            get { return _lastActivation; }
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsActiveContent property.
        /// </summary>
        protected virtual void OnIsActiveContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsActiveContent)
                _lastActivation = DateTime.Now;

            FocusContent();

            Pane parentPane = ContainerPane as Pane;
            if (parentPane != null)
            {
                parentPane.RefreshContainsActiveContentProperty();
                if (IsActiveContent)
                    parentPane.SelectedItem = this;
            }

            //for backward compatibility
            RaisePropertyChanged("IsActiveContent");

            if (IsActiveContentChanged != null)
                IsActiveContentChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Provides derived classes an opportunity to manage custom focus strategy.
        /// </summary>
        /// <remarks>
        /// Derived classes should not call base class if don't want AvalonDock to set focus on <see cref="DefaultElement"/> object
        /// </remarks>
        protected virtual void FocusContent()
        {
            if (IsActiveContent && !IsKeyboardFocused)
            {
                #region Focus on winforms content
                if (this.Content is WindowsFormsHost)
                {
                    //Use reflection in order to remove WinForms assembly reference
                    //WindowsFormsHost contentHost = this.Content as WindowsFormsHost;

                    //object childCtrl = contentHost.GetType().GetProperty("Child").GetValue(contentHost, null);

                    //if (childCtrl != null)
                    //{
                    //    if (!childCtrl.GetPropertyValue<bool>("Focused"))
                    //    {
                    //        childCtrl.CallMethod("Focus", null);
                    //    }
                    //}

                    //Dispatcher.BeginInvoke(DispatcherPriority.Background, new ThreadStart(delegate
                    //        {
                    //            if (IsActiveContent && !IsKeyboardFocused)
                    //            {
                    //                if (this.Content is WindowsFormsHost)
                    //                {
                    //                    //Use reflection in order to remove WinForms assembly reference
                    //                    WindowsFormsHost contentHost = this.Content as WindowsFormsHost;

                    //                    object childCtrl = contentHost.GetType().GetProperty("Child").GetValue(contentHost, null);

                    //                    if (childCtrl != null)
                    //                    {
                    //                        if (!childCtrl.GetPropertyValue<bool>("Focused"))
                    //                        {
                    //                            childCtrl.CallMethod("Focus", null);
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }));
                }
                #endregion

                Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(delegate
                        {
                            if (IsActiveContent && !IsKeyboardFocused)
                            {
                                if (DefaultElement != null)
                                {
                                    Debug.WriteLine("Try to set kb focus to " + DefaultElement);

                                    IInputElement kbFocused = Keyboard.Focus(DefaultElement);

                                    if (kbFocused != null)
                                        Debug.WriteLine("Focused element " + kbFocused);
                                    else
                                        Debug.WriteLine("No focused element");

                                }
                                else if (Content is UIElement && Content is DependencyObject)
                                {
                                    Debug.WriteLine("Try to set kb focus to " + this.Content.ToString());
                                    (Content as UIElement).Focus();
                                    IInputElement kbFocused = Keyboard.Focus(this.Content as IInputElement);
                                    if (kbFocused != null)
                                        Debug.WriteLine("Focused element " + kbFocused);
                                    else
                                        Debug.WriteLine("No focused element");
                                }
                            }
                        }));

                      
            }
        }

        /// <summary>
        /// Event fired when the <see cref="IsActiveContent"/> property changes
        /// </summary>
        public event EventHandler IsActiveContentChanged;
        #endregion

        #region IsActiveDocument

        /// <summary>
        /// IsActiveDocument Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey IsActiveDocumentPropertyKey
            = DependencyProperty.RegisterReadOnly("IsActiveDocument", typeof(bool), typeof(ManagedContent),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnIsActiveDocumentChanged)));

        public static readonly DependencyProperty IsActiveDocumentProperty
            = IsActiveDocumentPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the IsActiveDocument property.  This dependency property 
        /// indicates is content is the active document.
        /// </summary>
        public bool IsActiveDocument
        {
            get { return (bool)GetValue(IsActiveDocumentProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the IsActiveDocument property.  
        /// This dependency property indicates is content is the active document.
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        internal void SetIsActiveDocument(bool value)
        {
            SetValue(IsActiveDocumentPropertyKey, value);
        }

        /// <summary>
        /// Handles changes to the IsActiveDocument property.
        /// </summary>
        private static void OnIsActiveDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ManagedContent)d).OnIsActiveDocumentChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsActiveDocument property.
        /// </summary>
        protected virtual void OnIsActiveDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (ContainerPane != null)
                    ContainerPane.SelectedItem = this;
            }

            DocumentPane parentDocumentPane = ContainerPane as DocumentPane;
            if (parentDocumentPane != null)
            {
                parentDocumentPane.RefreshContainsActiveDocumentProperty();
            }

            //Debug.WriteLine("{0}-{1}-{2}", IsFocused, IsKeyboardFocused, IsKeyboardFocusWithin);

            //for backward compatibility
            RaisePropertyChanged("IsActiveDocumentChanged");

            if (IsActiveDocumentChanged != null)
                IsActiveDocumentChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event fired when the <see cref="IsActiveContent"/> property changes
        /// </summary>
        public event EventHandler IsActiveDocumentChanged;

        #endregion

        #region IsLocked

        /// <summary>
        /// IsLocked Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.Register("IsLocked", typeof(bool), typeof(ManagedContent),
                new FrameworkPropertyMetadata((bool)false));

        /// <summary>
        /// Gets or sets the IsLocked property.  This dependency property 
        /// indicates if this content is locked (for <see cref="DocumentContent"/> objects this often means that it's readonly).
        /// </summary>
        public bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }

        #endregion

        #region FloatingWindowSize

        /// <summary>
        /// FloatingWindowSize Dependency Property
        /// </summary>
        public static readonly DependencyProperty FloatingWindowSizeProperty =
            DependencyProperty.Register("FloatingWindowSize", typeof(Size), typeof(ManagedContent),
                new FrameworkPropertyMetadata(new Size(250,400),
                    new PropertyChangedCallback(OnFloatingWindowSizeChanged)));

        /// <summary>
        /// Gets or sets the FloatingWindowSize property.  This dependency property 
        /// indicates the size of the floating window hosting the content when it's floating.
        /// </summary>
        public Size FloatingWindowSize
        {
            get { return (Size)GetValue(FloatingWindowSizeProperty); }
            set { SetValue(FloatingWindowSizeProperty, value); }
        }

        /// <summary>
        /// Handles changes to the FloatingWindowSize property.
        /// </summary>
        private static void OnFloatingWindowSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ManagedContent)d).OnFloatingWindowSizeChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the FloatingWindowSize property.
        /// </summary>
        protected virtual void OnFloatingWindowSizeChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        
        #region IsCloseable
        /// <summary>
        /// Get or set a value indicating if this content can be closed or hidden
        /// </summary>
        public bool IsCloseable
        {
            get { return (bool)GetValue(IsCloseableProperty); }
            set { SetValue(IsCloseableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCloseable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCloseableProperty =
            DependencyProperty.Register("IsCloseable", typeof(bool), typeof(ManagedContent), new UIPropertyMetadata(true));

        internal virtual bool CanClose()
        {
            if (!IsCloseable)
                return false;

            return true;
        } 
        #endregion

        #region Commands

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.CommandBindings.Add(
                new CommandBinding(ManagedContentCommands.Hide, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(ManagedContentCommands.Close, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(ManagedContentCommands.Show, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(ManagedContentCommands.Activate, this.OnExecuteCommand, this.OnCanExecuteCommand));


        }

        void OnExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ManagedContentCommands.Show)
            {
                Show();
                e.Handled = true;
            }
            else if (e.Command == ManagedContentCommands.Hide)
            {
                e.Handled = Hide();
            }
            else if (e.Command == ManagedContentCommands.Close)
            {
                e.Handled = Close();
            }
            else if (e.Command == ManagedContentCommands.Activate)
            {
                Activate();
                e.Handled = true;
            }

            //else if (e.Command == ShowOptionsCommand)
            //{
            //    OpenOptionsContextMenu();
            //    e.Handled = true;
            //}

        }


        void OnCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanExecuteCommand(e.Command);

            //Debug.WriteLine("ManagedContent.OnCanExecuteCommand({0}) = {1} (ContinueRouting={2})", (e.Command as RoutedUICommand).Name, e.CanExecute, e.ContinueRouting);
        }


        /// <summary>
        /// Retrive a value indicating if the command passed can be executed based to the content state
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns>True if the command can be execute, false otherwise.</returns>
        protected virtual bool CanExecuteCommand(ICommand command)
        {
            if (Manager == null)
                return false;

            return true;
        }

        /// <summary>
        /// Shows the content
        /// </summary>
        /// <remarks>How content is shows depends from the type of the content.</remarks>
        public abstract void Show();

        /// <summary>
        /// Shows the content inside a <see cref="DockingManager"/> object
        /// </summary>
        /// <remarks>How content is shows depends from the type of the content.</remarks>
        public abstract void Show(DockingManager manager);

        /// <summary>
        /// Event fired when the content is about to be closed
        /// </summary>
        public event EventHandler<CancelEventArgs> Closing;

        /// <summary>
        /// Event fired when the content has been closed
        /// </summary>
        /// <remarks>Note that when a document is closed property like <see cref="ManagedContent.ContainerPane"/> or <see cref="ManagedContent.Manager"/> returns null.</remarks>
        public event EventHandler Closed;

        /// <summary>
        /// Ovveride this method to handle <see cref="DocumentContent.OnClosing"/> event.
        /// </summary>
        protected virtual void OnClosing(CancelEventArgs e)
        {
            if (Closing != null && !e.Cancel)
            {
                Closing(this, e);
            }
        }

        /// <summary>
        /// Ovveride this method to handle <see cref="DocumentContent.OnClose"/> event.
        /// </summary>
        protected virtual void OnClosed()
        {
            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Close the content
        /// </summary>
        /// <returns>Returns true if the content was succesfully closed, false otherwise.</returns>
        public abstract bool Close();

        /// <summary>
        /// Hide the content
        /// </summary>
        public abstract bool Hide();

        /// <summary>
        /// Set the content as the active content
        /// </summary>
        /// <remarks>After this method returns property <see cref="IsActiveContent"/> returns true.</remarks>
        public virtual void Activate()
        {
            if (ContainerPane != null && Manager != null)// && Manager.ActiveContent != this)
            {
                ContainerPane.SelectedItem = this;

                FocusContent();

                if (Manager != null)
                    Manager.ActiveContent = this;
            }
        }
        #endregion

        #region Save/Restore Content Layout
        /// <summary>
        /// Save content specific layout settings
        /// </summary>
        /// <param name="storeWriter">Backend store writer</param>
        /// <remarks>Custom derived class can overloads this method to handle custom layout persistence.</remarks>
        public virtual void SaveLayout(XmlWriter storeWriter)
        {
        }

        /// <summary>
        /// Restore content specific layout settings
        /// </summary>
        /// <param name="storeReader">Saved xml element containg content layout settings</param>
        /// <remarks>Custom derived class must overload this method to restore custom layout settings previously saved trought <see cref="SaveLayout"/>.</remarks>
        public virtual void RestoreLayout(XmlElement contentElement)
        {
        }
        #endregion



    }
}
