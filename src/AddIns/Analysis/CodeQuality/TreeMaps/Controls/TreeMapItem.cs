using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;

namespace TreeMaps.Controls
{
  [TemplatePart(Name = TreeMapItem.HeaderPartName, Type = typeof(FrameworkElement))]
  public class TreeMapItem : HeaderedItemsControl
  {
    #region consts

    private const string HeaderPartName = "PART_Header";

    #endregion

    #region fields

    private double _area;
    private TreeMaps _parentTreeMaps;
    
    #endregion

    #region dependency properties

    public static DependencyProperty TreeMapModeProperty
      = DependencyProperty.Register("TreeMapMode", typeof(TreeMapAlgo), typeof(TreeMapItem), new FrameworkPropertyMetadata(TreeMapAlgo.Squarified,FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    public static readonly DependencyProperty ValuePropertyNameProperty
      = DependencyProperty.Register("ValuePropertyName", typeof(string), typeof(TreeMapItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    public static readonly DependencyProperty LevelProperty
      = DependencyProperty.Register("Level", typeof(int), typeof(TreeMapItem),new FrameworkPropertyMetadata(0,FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty MaxDepthProperty
      = DependencyProperty.Register("MaxDepth", typeof(int), typeof(TreeMapItem),new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty MinAreaProperty
      = DependencyProperty.Register("MinArea", typeof(int), typeof(TreeMapItem), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ShouldRecurseProperty
      = DependencyProperty.Register("ShouldRecurse", typeof(bool), typeof(TreeMapItem), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion


    #region ctors

    static TreeMapItem()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeMapItem), new FrameworkPropertyMetadata(typeof(TreeMapItem)));
    }

    public TreeMapItem()
    {
      this.VerticalAlignment = VerticalAlignment.Stretch;
      this.HorizontalAlignment = HorizontalAlignment.Stretch;
      this.VerticalContentAlignment = VerticalAlignment.Stretch;
      this.HorizontalContentAlignment = HorizontalAlignment.Stretch;

      this.SnapsToDevicePixels = true;
    }

    public TreeMapItem(int level, int maxDepth, int minArea, string valuePropertyName)
      : this()
    {
      this.ValuePropertyName = valuePropertyName;
      this.Level = level;
      this.MaxDepth = maxDepth;
      this.MinArea = minArea;
    }
    #endregion

    #region properties

    public TreeMapAlgo TreeMapMode
    {
      get { return (TreeMapAlgo)this.GetValue(TreeMapItem.TreeMapModeProperty); }
      set { this.SetValue(TreeMapItem.TreeMapModeProperty, value); }
    }

    public string ValuePropertyName
    {
      get { return (string)this.GetValue(TreeMapItem.ValuePropertyNameProperty); }
      set { this.SetValue(TreeMapItem.ValuePropertyNameProperty, value); }
    }

    public int MaxDepth
    {
      get { return (int)this.GetValue(TreeMapItem.MaxDepthProperty); }
      internal set { this.SetValue(TreeMapItem.MaxDepthProperty, value); }
    }

    public int MinArea
    {
      get { return (int)this.GetValue(TreeMapItem.MinAreaProperty); }
      internal set { this.SetValue(TreeMapItem.MinAreaProperty, value); }
    }

    public bool ShouldRecurse
    {
      get { return (bool)this.GetValue(TreeMapItem.ShouldRecurseProperty); }
      internal set { this.SetValue(TreeMapItem.ShouldRecurseProperty, value); }
    }

    public int Level
    {
      get { return (int)this.GetValue(TreeMapItem.LevelProperty); }
      set { this.SetValue(TreeMapItem.LevelProperty, value); }
    }

    internal double Area
    {
      get { return _area; }
    }

    internal ItemsControl ParentItemsControl
    {
      get { return ItemsControl.ItemsControlFromItemContainer(this); }
    }

    internal TreeMaps ParentTreeMap
    {
      get
      {
        for (ItemsControl control = this.ParentItemsControl; control != null; control = ItemsControl.ItemsControlFromItemContainer(control))
        {
          TreeMaps view = control as TreeMaps;
          if (view != null)
            return view;
        }
        return null;
      }
    }

    internal TreeMapItem ParentTreeMapItem
    {
      get { return (this.ParentItemsControl as TreeMapItem); }
    }

    #endregion

    #region protected methods

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      Size size = base.ArrangeOverride(arrangeBounds);
      if (this.IsValidSize(size))
        _area = size.Width * size.Height;
      else
        _area = 0;
      this.UpdateShouldRecurse();

      return size;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
      return new TreeMapItem(this.Level+1,this.MaxDepth,this.MinArea,this.ValuePropertyName);
      
    }

    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
      base.OnVisualParentChanged(oldParent);
      _parentTreeMaps = this.ParentTreeMap;
      if (_parentTreeMaps != null)
      {
        Binding bindingMode = new Binding(TreeMaps.TreeMapModeProperty.Name);
        bindingMode.Source = _parentTreeMaps;
        BindingOperations.SetBinding(this, TreeMapItem.TreeMapModeProperty, bindingMode);

        Binding bindingValue = new Binding(TreeMaps.ValuePropertyNameProperty.Name);
        bindingValue.Source = _parentTreeMaps;
        BindingOperations.SetBinding(this, TreeMapItem.ValuePropertyNameProperty, bindingValue);

        Binding bindingMinArea = new Binding(TreeMaps.MinAreaProperty.Name);
        bindingMinArea.Source = _parentTreeMaps;
        BindingOperations.SetBinding(this, TreeMapItem.MinAreaProperty, bindingMinArea);

        Binding bindingMaxDepth = new Binding(TreeMaps.MaxDepthProperty.Name);
        bindingMaxDepth.Source = _parentTreeMaps;
        BindingOperations.SetBinding(this, TreeMapItem.MaxDepthProperty, bindingMaxDepth);
      }
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      if (e.Property == TreeMapItem.ValuePropertyNameProperty || e.Property == TreeMapItem.DataContextProperty)
      {
        if (this.ValuePropertyName != null && this.DataContext != null)
        {
          Binding binding = new Binding(this.ValuePropertyName);
          binding.Source = this.DataContext;
          BindingOperations.SetBinding(this, TreeMapsPanel.WeightProperty, binding);
        }
      }
      else if (e.Property == TreeMapItem.MaxDepthProperty)
        this.UpdateShouldRecurse();
      else if (e.Property == TreeMapItem.MinAreaProperty)
        this.UpdateShouldRecurse();
      else if (e.Property == TreeMapItem.LevelProperty)
        this.UpdateShouldRecurse();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return (item is TreeMapItem);
    }

    #endregion

    #region private methods

    private bool IsValidSize(Size size)
    {
      return (!size.IsEmpty && size.Width > 0 && size.Width != double.NaN && size.Height > 0 && size.Height != double.NaN);
    }

    private void UpdateShouldRecurse()
    {
      if (!this.HasHeader)
      {
        this.ShouldRecurse = false;
        return;
      }

      this.ShouldRecurse = ((this.MaxDepth == 0) || (this.Level < this.MaxDepth)) && ((this.MinArea == 0) || (this.Area >this.MinArea));
    }

    #endregion

  }
}
