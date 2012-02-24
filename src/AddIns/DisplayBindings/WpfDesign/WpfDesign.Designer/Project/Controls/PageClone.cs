// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Description of PageClone.
	/// </summary>
	[ContentProperty("Content")]
	public class PageClone : FrameworkElement, IAddChild
	{
		static PageClone()
		{

			Control.IsTabStopProperty.OverrideMetadata(typeof(PageClone), new FrameworkPropertyMetadata(SharedInstances.BoxedFalse));
			KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(PageClone), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
			KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(PageClone), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
			KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(PageClone), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
			FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(PageClone), new FrameworkPropertyMetadata(SharedInstances.BoxedTrue));
		}
		
		public static readonly DependencyProperty ContentProperty = Page.ContentProperty.AddOwner(typeof(PageClone));
		
		ContentPresenter content;
		
		public PageClone()
		{
			content = new ContentPresenter();
			content.SetBinding(ContentPresenter.ContentProperty, new Binding("Content") { Source = this });
			AddVisualChild(content);
		}
		
		protected override int VisualChildrenCount {
			get { return 1; }
		}

		protected override Visual GetVisualChild(int index)
		{
			if (index != 0)
				throw new ArgumentOutOfRangeException();
			return content;
		}
		
		protected override Size ArrangeOverride(Size finalSize)
		{
			content.Arrange(new Rect(finalSize));
			return finalSize;
		}
		
		protected override Size MeasureOverride(Size availableSize)
		{
			content.Measure(availableSize);
			return content.DesiredSize;
		}
		
		[Category("Appearance")]
		public Brush Background {
			get { return (Brush)GetValue(Page.BackgroundProperty); }
			set { SetValue(Page.BackgroundProperty, value); }
		}
		
		public object Content {
			get {
				VerifyAccess();
				return GetValue(ContentProperty);
			}
			set {
				VerifyAccess();
				SetValue(ContentProperty, value);
			}
		}
		
		[Bindable(true), Category("Appearance"), Localizability(LocalizationCategory.Font, Modifiability = Modifiability.Unmodifiable)]
		public FontFamily FontFamily {
			get { return (FontFamily)GetValue(Page.FontFamilyProperty); }
			set { SetValue(Page.FontFamilyProperty, value); }
		}
		
		[Bindable(true), Category("Appearance"), TypeConverter(typeof(FontSizeConverter)), Localizability(LocalizationCategory.None)]
		public double FontSize {
			get { return (double)GetValue(Page.FontSizeProperty); }
			set { SetValue(Page.FontSizeProperty, value); }
		}

		[Bindable(true), Category("Appearance")]
		public Brush Foreground {
			get { return (Brush)GetValue(Page.ForegroundProperty); }
			set { SetValue(Page.ForegroundProperty, value); }
		}

		public bool KeepAlive {
			get { return JournalEntry.GetKeepAlive(this); }
			set { JournalEntry.SetKeepAlive(this, value); }
		}

		public NavigationService NavigationService {
			get { return NavigationService.GetNavigationService(this); }
		}
		
		public ControlTemplate Template {
			get { return (ControlTemplate)GetValue(Page.TemplateProperty); }
			set { SetValue(Page.TemplateProperty, value); }
		}

		public bool ShowsNavigationUI { get; set; }
		
		public string Title {
			get { return (string)base.GetValue(Page.TitleProperty); }
			set { base.SetValue(Page.TitleProperty, value); }
		}
		
		public string WindowTitle {
			get { return Title; }
			set { Title = value; }
		}
		
		public double WindowWidth { get; set; }
		
		public double WindowHeight { get; set; }
		
		void IAddChild.AddChild(object value)
		{
			base.VerifyAccess();
			if (this.Content == null || value == null)
				this.Content = value;
			else
				throw new InvalidOperationException();
		}
		
		void IAddChild.AddText(string text)
		{
			if (text == null)
				return;
			
			for (int i = 0; i < text.Length; i++) {
				if (!char.IsWhiteSpace(text[i]))
					throw new ArgumentException();
			}
		}
	}
	
	/// <summary>
	/// A <see cref="CustomInstanceFactory"/> for <see cref="Page"/>
	/// (and derived classes, unless they specify their own <see cref="CustomInstanceFactory"/>).
	/// </summary>
	[ExtensionFor(typeof(Page))]
	public class PageCloneExtension : CustomInstanceFactory
	{
		/// <summary>
		/// Used to create instances of <see cref="PageClone"/>.
		/// </summary>
		public override object CreateInstance(Type type, params object[] arguments)
		{
			Debug.Assert(arguments.Length == 0);
			return new PageClone();
		}
	}
}
