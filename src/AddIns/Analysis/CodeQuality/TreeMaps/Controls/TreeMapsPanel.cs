using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace TreeMaps.Controls
{
  public class TreeMapsPanel : Panel
  {
    #region fields

    private Rect _emptyArea;
    private double _weightSum = 0;
    private List<WeightUIElement> _items = new List<WeightUIElement>();

    #endregion

    #region dependency properties

    public static readonly DependencyProperty 
      WeightProperty = DependencyProperty.RegisterAttached("Weight", typeof(double),typeof(TreeMapsPanel),new FrameworkPropertyMetadata(1.0,FrameworkPropertyMetadataOptions.AffectsParentArrange|FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    #endregion

    #region enum

    protected enum RowOrientation
    {
      Horizontal,
      Vertical
    }

    #endregion

    #region properties

    public static double GetWeight(DependencyObject uiElement)
    {
      if (uiElement == null)
        return 0;
      else
        return (double)uiElement.GetValue(TreeMapsPanel.WeightProperty);
    }

    public static void SetWeight(DependencyObject uiElement, double value)
    {
      if (uiElement != null)
        uiElement.SetValue(TreeMapsPanel.WeightProperty, value);
    }

    protected Rect EmptyArea
    {
      get { return _emptyArea; }
      set { _emptyArea = value; }
    }

    protected List<WeightUIElement> ManagedItems
    {
      get { return _items; }
    }

    #endregion

    #region protected methods

    protected override Size ArrangeOverride(Size arrangeSize)
    {
      foreach (WeightUIElement child in this.ManagedItems)
        child.UIElement.Arrange(new Rect(child.ComputedLocation, child.ComputedSize));
      return arrangeSize;
    }

    protected override Size MeasureOverride(Size constraint)
    {
      this.EmptyArea = new Rect(0, 0, constraint.Width, constraint.Height);
      this.PrepareItems();

      double area = this.EmptyArea.Width * this.EmptyArea.Height;
      foreach (WeightUIElement item in this.ManagedItems)
        item.RealArea = area * item.Weight / _weightSum;

      this.ComputeBounds();

      foreach (WeightUIElement child in this.ManagedItems)
      {
        if (this.IsValidSize(child.ComputedSize))
          child.UIElement.Measure(child.ComputedSize);
        else
          child.UIElement.Measure(new Size(0, 0));
      }

      return constraint;
    }

    protected virtual void ComputeBounds()
    {
      this.ComputeTreeMaps(this.ManagedItems);
    }

    protected double GetShortestSide()
    {
      return Math.Min(this.EmptyArea.Width, this.EmptyArea.Height);
    }

    protected RowOrientation GetOrientation()
    {
      return (this.EmptyArea.Width > this.EmptyArea.Height ? RowOrientation.Horizontal : RowOrientation.Vertical);
    }

    protected virtual Rect GetRectangle(RowOrientation orientation, WeightUIElement item, double x, double y, double width, double height)
    {
      if (orientation == RowOrientation.Horizontal)
        return new Rect(x, y, item.RealArea / height, height);
      else
        return new Rect(x, y, width, item.RealArea / width);
    }

    protected virtual void ComputeNextPosition(RowOrientation orientation, ref double xPos, ref double yPos, double width, double height)
    {
      if (orientation == RowOrientation.Horizontal)
        xPos += width;
      else
        yPos += height;
    }

    protected void ComputeTreeMaps(List<WeightUIElement> items)
    {
      RowOrientation orientation = this.GetOrientation();

      double areaSum = 0;

      foreach (WeightUIElement item in items)
        areaSum += item.RealArea;

      Rect currentRow;
      if (orientation == RowOrientation.Horizontal)
      {
        currentRow = new Rect(_emptyArea.X, _emptyArea.Y, areaSum / _emptyArea.Height, _emptyArea.Height);
        _emptyArea = new Rect(_emptyArea.X + currentRow.Width, _emptyArea.Y, Math.Max(0, _emptyArea.Width - currentRow.Width), _emptyArea.Height);
      }
      else
      {
        currentRow = new Rect(_emptyArea.X, _emptyArea.Y, _emptyArea.Width, areaSum / _emptyArea.Width);
        _emptyArea = new Rect(_emptyArea.X, _emptyArea.Y + currentRow.Height, _emptyArea.Width, Math.Max(0, _emptyArea.Height - currentRow.Height));
      }

      double prevX = currentRow.X;
      double prevY = currentRow.Y;

      foreach (WeightUIElement item in items)
      {
        Rect rect = this.GetRectangle(orientation, item, prevX, prevY, currentRow.Width, currentRow.Height);

        item.AspectRatio = rect.Width / rect.Height;
        item.ComputedSize = rect.Size;
        item.ComputedLocation = rect.Location;

        this.ComputeNextPosition(orientation, ref prevX, ref prevY, rect.Width, rect.Height);
      }
    }

    #endregion

    #region private methods

    private bool IsValidSize(Size size)
    {
      return (!size.IsEmpty && size.Width > 0 && size.Width != double.NaN && size.Height > 0 && size.Height != double.NaN);
    }

    private bool IsValidItem(WeightUIElement item)
    {
      return (item != null && item.Weight != double.NaN && Math.Round(item.Weight, 0) != 0);
    }

    private void PrepareItems()
    {

      _weightSum = 0;
      this.ManagedItems.Clear();


      foreach (UIElement child in this.Children)
      {
        WeightUIElement element = new WeightUIElement(child, TreeMapsPanel.GetWeight(child));
        if (this.IsValidItem(element))
          {
            _weightSum += element.Weight;
            this.ManagedItems.Add(element);
          }
          else
          {
            element.ComputedSize = Size.Empty;
            element.ComputedLocation = new Point(0, 0);
            element.UIElement.Measure(element.ComputedSize);
            element.UIElement.Visibility = Visibility.Collapsed;
          }
      }

      this.ManagedItems.Sort(WeightUIElement.CompareByValueDecreasing);
    }

    #endregion

    #region inner classes

    protected class WeightUIElement
    {
      #region fields

      private double _weight;
      private double _area;
      private UIElement _element;
      private Size _desiredSize;
      private Point _desiredLocation;
      private double _ratio;

      #endregion

      #region ctors

      public WeightUIElement(UIElement element, double weight)
      {
        _element = element;
        _weight = weight;
      }

      #endregion

      #region properties

      internal Size ComputedSize
      {
        get { return _desiredSize; }
        set { _desiredSize = value; }
      }

      internal Point ComputedLocation
      {
        get { return _desiredLocation; }
        set { _desiredLocation = value; }
      }
      public double AspectRatio
      {
        get { return _ratio; }
        set { _ratio = value; }
      }
      public double Weight
      {
        get { return _weight; }
      }
      public double RealArea
      {
        get { return _area; }
        set { _area = value; }
      }

      public UIElement UIElement
      {
        get { return _element; }
      }

      #endregion

      #region static members

      public static int CompareByValueDecreasing(WeightUIElement x, WeightUIElement y)
      {
        if (x == null)
        {
          if (y == null)
            return -1;
          else
            return 0;
        }
        else
        {
          if (y == null)
            return 1;
          else
            return x.Weight.CompareTo(y.Weight) * -1;
        }
      }

      #endregion
    }

    #endregion

  }
}
