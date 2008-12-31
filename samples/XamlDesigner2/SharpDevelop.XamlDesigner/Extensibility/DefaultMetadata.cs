using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using SharpDevelop.XamlDesigner.Extensibility.Attributes;
using SharpDevelop.XamlDesigner.PropertyGrid.Editors;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;
using System.Windows.Documents;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Input;
using System.Reflection;
using SharpDevelop.XamlDesigner.Dom;
using SharpDevelop.XamlDesigner.Placement;

namespace SharpDevelop.XamlDesigner.Extensibility
{
	static class DefaultMetadata
	{
		static Type[] numericTypes = new Type[] 
		{
			typeof(Byte),
			typeof(SByte),
			typeof(Int16),
			typeof(UInt16),
			typeof(Int32),
			typeof(UInt32),
			typeof(Int64),
			typeof(UInt64),
			typeof(Single),
			typeof(Double),
			typeof(Decimal)
		};

		internal static void Register()
		{
			AddItemInitializer(typeof(Panel), DefaultInitializers.Panel);
			AddItemInitializer(typeof(Border), DefaultInitializers.Border);
			AddItemInitializer(typeof(Shape), DefaultInitializers.Shape);
			AddItemInitializer(typeof(Label), DefaultInitializers.Label); 

			AddNewItemInitializer(typeof(ContentControl), DefaultInitializers.NewContentControl);

			AddEditor(typeof(bool), EditorTemplates.BoolEditor);
			AddEditor(typeof(bool?), EditorTemplates.NuallableBoolEditor);
			AddEditor(typeof(Thickness), EditorTemplates.ThicknessEditor);
			AddEditor(typeof(HorizontalAlignment), EditorTemplates.HorizontalAlignmentEditor);
			AddEditor(typeof(VerticalAlignment), EditorTemplates.VerticalAlignmentEditor);
			AddEditor(typeof(IList), EditorTemplates.CollectionEditor);
			AddEditor(typeof(Brush), EditorTemplates.BrushEditor);
			AddEditor(typeof(MulticastDelegate), EditorTemplates.EventEditor);

			AddEditor(FrameworkElement.DataContextProperty, EditorTemplates.ObjectEditor);

			foreach (var type in numericTypes) {
				AddEditor(type, EditorTemplates.NumberEditor);
			}

			MetadataStore.AddAttribute(UIElement.OpacityProperty, new ValueRangeAttribute() {
				ValueRange = new ValueRange() { Min = 0, Max = 1 }
			});

			AddStandardValues(typeof(Brush), typeof(Brushes));
			AddStandardValues(typeof(Color), typeof(Colors));
			AddStandardValues(typeof(FontStretch), typeof(FontStretches));
			AddStandardValues(typeof(FontWeight), typeof(FontWeights));
			AddStandardValues(typeof(FontStyle), typeof(FontStyles));
			AddStandardValues(typeof(Cursor), typeof(Cursors));
			AddStandardValues(typeof(PixelFormat), typeof(PixelFormats));
			AddStandardValues(typeof(TextDecorationCollection), typeof(TextDecorations));

			AddStandardValues(typeof(ICommand), typeof(ApplicationCommands));
			AddStandardValues(typeof(ICommand), typeof(EditingCommands));
			AddStandardValues(typeof(ICommand), typeof(NavigationCommands));
			AddStandardValues(typeof(ICommand), typeof(ComponentCommands));
			AddStandardValues(typeof(ICommand), typeof(MediaCommands));

			AddStandardValues(typeof(FontFamily), Fonts.SystemFontFamilies
				 .Select(f => new StandardValue() { Instance = f, Text = f.Source }));

			AddPopularProperty(Line.Y2Property);
			AddPopularProperty(NavigationWindow.ShowsNavigationUIProperty);
			AddPopularProperty(FlowDocumentScrollViewer.DocumentProperty);
			AddPopularProperty(GridViewRowPresenterBase.ColumnsProperty);
			AddPopularProperty(ListView.ViewProperty);
			AddPopularProperty(DocumentPageView.PageNumberProperty);
			AddPopularProperty(Popup.PlacementProperty);
			AddPopularProperty(Popup.PopupAnimationProperty);
			AddPopularProperty(ScrollBar.ViewportSizeProperty);
			AddPopularProperty(UniformGrid.RowsProperty);
			AddPopularProperty(TabControl.TabStripPlacementProperty);
			AddPopularProperty(Line.X1Property);
			AddPopularProperty(Line.Y1Property);
			AddPopularProperty(Line.X2Property);
			AddPopularProperty(Polygon.PointsProperty);
			AddPopularProperty(Polyline.PointsProperty);
			AddPopularProperty(Path.DataProperty);
			AddPopularProperty(HeaderedContentControl.HeaderProperty);
			AddPopularProperty(MediaElement.UnloadedBehaviorProperty);
			AddPopularProperty(Shape.FillProperty);
			AddPopularProperty(Page.TitleProperty);
			AddPopularProperty(ItemsControl.ItemsSourceProperty);
			AddPopularProperty(Image.SourceProperty);
			AddPopularProperty(TextBlock.TextProperty);
			AddPopularProperty(DockPanel.LastChildFillProperty);
			AddPopularProperty(Expander.IsExpandedProperty);
			AddPopularProperty(Shape.StrokeProperty);
			AddPopularProperty(RangeBase.ValueProperty);
			AddPopularProperty(ItemsControl.ItemContainerStyleProperty);
			AddPopularProperty(ToggleButton.IsCheckedProperty);
			AddPopularProperty(Window.TitleProperty);
			AddPopularProperty(Viewport3DVisual.CameraProperty);
			AddPopularProperty(Frame.SourceProperty);
			AddPopularProperty(Rectangle.RadiusXProperty);
			AddPopularProperty(Rectangle.RadiusYProperty);
			AddPopularProperty(FrameworkElement.HeightProperty);
			AddPopularProperty(FrameworkElement.WidthProperty);
			AddPopularProperty(UniformGrid.ColumnsProperty);
			AddPopularProperty(RangeBase.MinimumProperty);
			AddPopularProperty(RangeBase.MaximumProperty);
			AddPopularProperty(ScrollBar.OrientationProperty);
			AddPopularProperty(ContentControl.ContentProperty);
			AddPopularProperty(Popup.IsOpenProperty);
			AddPopularProperty(TextElement.FontSizeProperty);
			AddPopularProperty(FrameworkElement.NameProperty);
			AddPopularProperty(Popup.HorizontalOffsetProperty);
			AddPopularProperty(Popup.VerticalOffsetProperty);
			AddPopularProperty(Window.WindowStyleProperty);
			AddPopularProperty(Shape.StrokeThicknessProperty);
			AddPopularProperty(TextElement.ForegroundProperty);
			AddPopularProperty(FrameworkElement.VerticalAlignmentProperty);
			AddPopularProperty(Button.IsDefaultProperty);
			AddPopularProperty(UIElement.RenderTransformOriginProperty);
			AddPopularProperty(TextElement.FontFamilyProperty);
			AddPopularProperty(FrameworkElement.HorizontalAlignmentProperty);
			AddPopularProperty(ToolBar.BandProperty);
			AddPopularProperty(ToolBar.BandIndexProperty);
			AddPopularProperty(ItemsControl.ItemTemplateProperty);
			AddPopularProperty(TextBlock.TextWrappingProperty);
			AddPopularProperty(FrameworkElement.MarginProperty);
			AddPopularProperty(RangeBase.LargeChangeProperty);
			AddPopularProperty(RangeBase.SmallChangeProperty);
			AddPopularProperty(Panel.BackgroundProperty);
			AddPopularProperty(Shape.StrokeMiterLimitProperty);
			AddPopularProperty(TextElement.FontWeightProperty);
			AddPopularProperty(StackPanel.OrientationProperty);
			AddPopularProperty(ListBox.SelectionModeProperty);
			AddPopularProperty(FrameworkElement.StyleProperty);
			AddPopularProperty(TextBox.TextProperty);
			AddPopularProperty(Window.SizeToContentProperty);
			AddPopularProperty(Window.ResizeModeProperty);
			AddPopularProperty(TextBlock.TextTrimmingProperty);
			AddPopularProperty(Window.ShowInTaskbarProperty);
			AddPopularProperty(Window.IconProperty);
			AddPopularProperty(UIElement.RenderTransformProperty);
			AddPopularProperty(Button.IsCancelProperty);
			AddPopularProperty(Border.BorderBrushProperty);
			AddPopularProperty(Block.TextAlignmentProperty);
			AddPopularProperty(Border.CornerRadiusProperty);
			AddPopularProperty(Border.BorderThicknessProperty);
			AddPopularProperty(TreeViewItem.IsSelectedProperty);
			AddPopularProperty(Border.PaddingProperty);
			AddPopularProperty(Shape.StretchProperty);

			HideProperty(FrameworkElement.NameProperty);
			HideProperty(typeof(UIElement), "RenderSize");
			HideProperty(typeof(FrameworkElement), "Resources");
			HideProperty(typeof(Window), "Owner");

			AddContainerType(typeof(Grid), typeof(GridContainer));
			AddContainerType(typeof(Canvas), typeof(CanvasContainer));
			AddContainerType(typeof(StackPanel), typeof(StackPanelContainer));
			AddContainerType(typeof(DockPanel), typeof(DockPanelContainer));
			AddContainerType(typeof(WrapPanel), typeof(WrapPanelContainer));
			AddContainerType(typeof(UniformGrid), typeof(PreviewContainer));

			AddDefaultSize(typeof(UIElement), new Size(120, 100));
			AddDefaultSize(typeof(ContentControl), new Size(double.NaN, double.NaN));
			AddDefaultSize(typeof(Button), new Size(75, 23));

			var s1 = new Size(120, double.NaN);
			AddDefaultSize(typeof(Slider), s1);
			AddDefaultSize(typeof(TextBox), s1);
			AddDefaultSize(typeof(PasswordBox), s1);
			AddDefaultSize(typeof(ComboBox), s1);
			AddDefaultSize(typeof(ProgressBar), s1);

			var s2 = new Size(120, 20);
			AddDefaultSize(typeof(ToolBar), s2);
			AddDefaultSize(typeof(Menu), s2);			
		}

		static void AddEditor(Type type, DataTemplate editorTemplate)
		{
			MetadataStore.AddAttribute(type, new PropertyEditorAttribute() {
				EditorTemplate = editorTemplate
			});
		}

		static void AddEditor(DependencyProperty dp, DataTemplate editorTemplate)
		{
			MetadataStore.AddAttribute(dp, new PropertyEditorAttribute() {
				EditorTemplate = editorTemplate
			});
		}

		static void AddPopularProperty(DependencyProperty dp)
		{
			MetadataStore.AddAttribute(dp, new PopularAttribute());
		}

		static void HideProperty(DependencyProperty dp)
		{
			MetadataStore.AddAttribute(dp, new BrowsableAttribute(false));
		}

		static void HideProperty(Type type, string memberName)
		{
			MetadataStore.AddAttribute(type, memberName, new BrowsableAttribute(false));
		}

		static void AddStandardValues(Type type, Type valuesContainer)
		{
			AddStandardValues(type, valuesContainer
				.GetProperties(BindingFlags.Public | BindingFlags.Static)
				.Select(p => new StandardValue() {
					Instance = p.GetValue(null, null),
					Text = p.Name
				}));
		}

		static void AddStandardValues(Type type, IEnumerable<StandardValue> values)
		{
			MetadataStore.AddAttribute(type, new StandardValuesAttribute() {
				Type = type,
				StandardValues = values.ToArray()
			});
		}

		public static void AddContainerType(Type type, Type containerType)
		{
			MetadataStore.AddAttribute(type, new ContainerTypeAttribute() {
				ContainerType = containerType
			});
		}

		public static void AddDefaultSize(Type type, Size defaultSize)
		{
			MetadataStore.AddAttribute(type, new DefaultSizeAttribute() { DefaultSize = defaultSize });
		}

		public static void AddItemInitializer(Type type, Action<DesignItem> initializer)
		{
			MetadataStore.AddAttribute(type, new ItemInitializerAttribute() {
				ItemInitializer = initializer
			});
		}

		public static void AddNewItemInitializer(Type type, Action<DesignItem> initializer)
		{
			MetadataStore.AddAttribute(type, new NewItemInitializerAttribute() {
				NewItemInitializer = initializer
			});
		}
	}
}
