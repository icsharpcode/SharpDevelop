/*
 * Created by SharpDevelop.
 * User: trubra
 * Date: 2014-12-22
 * Time: 11:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Description of MultiPointResizeThumb.
	/// </summary>
	sealed class MultiPointResizeThumb: ResizeThumb
    {
        private int _index;
        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                PointTrackerPlacementSupport p = AdornerPlacement as PointTrackerPlacementSupport;
                if (p != null)
                    p.Index = value;
            }
        }

        public void Invalidate()
        {
            PointTrackerPlacementSupport p = AdornerPlacement as PointTrackerPlacementSupport;

        }

        private AdornerPlacement _adornerPlacement;

        public AdornerPlacement AdornerPlacement
        {
            get { return _adornerPlacement; }
            set { _adornerPlacement = value; }
        }

    }
}
