using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace TreeMaps.Controls
{

  public class TreeMaps: ItemsControl
  {
    #region fields

    #endregion

    #region dependency properties

    public static DependencyProperty TreeMapModeProperty
      = DependencyProperty.Register("TreeMapMode", typeof(TreeMapAlgo), typeof(TreeMaps), new FrameworkPropertyMetadata(TreeMapAlgo.Squarified,FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    public static DependencyProperty ValuePropertyNameProperty
      = DependencyProperty.Register("ValuePropertyName", typeof(string), typeof(TreeMaps),new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    public static DependencyProperty MaxDepthProperty
      = DependencyProperty.Register("MaxDepth", typeof(int), typeof(TreeMaps),new FrameworkPropertyMetadata(1,FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty MinAreaProperty
      = DependencyProperty.Register("MinArea", typeof(int), typeof(TreeMaps),new FrameworkPropertyMetadata(64,FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion

    #region ctors

    static TreeMaps()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeMaps), new FrameworkPropertyMetadata(typeof(TreeMaps)));
    }

    #endregion

    #region properties

    public TreeMapAlgo TreeMapMode
    {
      get { return (TreeMapAlgo)this.GetValue(TreeMaps.TreeMapModeProperty); }
      set { this.SetValue(TreeMaps.TreeMapModeProperty, value); }
    }

    public int MaxDepth
    {
      get { return (int)this.GetValue(TreeMaps.MaxDepthProperty); }
      set { this.SetValue(TreeMaps.MaxDepthProperty, value); }
    }

    public int MinArea
    {
      get { return (int)this.GetValue(TreeMaps.MinAreaProperty); }
      set { this.SetValue(TreeMaps.MinAreaProperty, value); }
    }

    public string ValuePropertyName
    {
      get { return (string)this.GetValue(TreeMaps.ValuePropertyNameProperty); }
      set { this.SetValue(TreeMaps.ValuePropertyNameProperty, value); }
    }

    #endregion

    #region protected methods

    protected override DependencyObject GetContainerForItemOverride()
    {
      return new TreeMapItem(1, this.MaxDepth, this.MinArea, this.ValuePropertyName);
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return (item is TreeMapItem);
    }

    #endregion

  }

  public enum TreeMapAlgo
  {
    Standard,
    Squarified
  }
}
