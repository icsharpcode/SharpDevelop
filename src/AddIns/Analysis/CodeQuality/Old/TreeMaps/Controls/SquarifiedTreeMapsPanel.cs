using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace TreeMaps.Controls
{
  public class SquarifiedTreeMapsPanel : TreeMapsPanel
  {
    #region protected methods

    protected override Rect GetRectangle(RowOrientation orientation, WeightUIElement item, double x, double y, double width, double height)
    {
      if (orientation == RowOrientation.Horizontal)
        return new Rect(x, y, width, item.RealArea / width);
      else
        return new Rect(x, y, item.RealArea / height, height);
    }

    protected override void ComputeNextPosition(RowOrientation orientation, ref double xPos, ref double yPos, double width, double height)
    {
      if (orientation == RowOrientation.Horizontal)
        yPos += height;
      else
        xPos += width;
    }

    protected override void ComputeBounds()
    {
      this.Squarify(this.ManagedItems, new List<WeightUIElement>(), this.GetShortestSide());
    }

    #endregion

    #region private methods

    private void Squarify(List<WeightUIElement> items, List<WeightUIElement> row, double sideLength)
    {
      if (items.Count == 0)
      {
        this.AddRowToLayout(row);
        return;
      }

      WeightUIElement item = items[0];
      List<WeightUIElement> row2 = new List<WeightUIElement>(row);
      row2.Add(item);
      List<WeightUIElement> items2 = new List<WeightUIElement>(items);
      items2.RemoveAt(0);

      double worst1 = this.Worst(row, sideLength);
      double worst2 = this.Worst(row2, sideLength);

      if (row.Count == 0 || worst1 > worst2)
        this.Squarify(items2, row2, sideLength);
      else
      {
        this.AddRowToLayout(row);
        this.Squarify(items, new List<WeightUIElement>(), this.GetShortestSide());
      }
    }

    private void AddRowToLayout(List<WeightUIElement> row)
    {
      base.ComputeTreeMaps(row);
    }

    private double Worst(List<WeightUIElement> row, double sideLength)
    {
      if (row.Count == 0) return 0;

      double maxArea = 0;
      double minArea = double.MaxValue;
      double totalArea = 0;
      foreach (WeightUIElement item in row)
      {
        maxArea = Math.Max(maxArea, item.RealArea);
        minArea = Math.Min(minArea, item.RealArea);
        totalArea += item.RealArea;
      }
      if (minArea == double.MaxValue) minArea = 0;

      double val1 = (sideLength * sideLength * maxArea) / (totalArea * totalArea);
      double val2 = (totalArea * totalArea) / (sideLength * sideLength * minArea);
      return Math.Max(val1, val2);
    }

    #endregion
  }
}
