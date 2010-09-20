// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.WpfDesign.Adorners;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Describes the result of a <see cref="IDesignPanel.HitTest(Point, bool, bool)"/> call.
	/// </summary>
	public struct DesignPanelHitTestResult : IEquatable<DesignPanelHitTestResult>
	{
		/// <summary>
		/// Represents the result that nothing was hit.
		/// </summary>
		public static readonly DesignPanelHitTestResult NoHit = new DesignPanelHitTestResult();
		
		readonly Visual _visualHit;
		AdornerPanel _adornerHit;
		DesignItem _modelHit;
		
		/// <summary>
		/// The actual visual that was hit.
		/// </summary>
		public Visual VisualHit {
			get { return _visualHit; }
		}
		
		/// <summary>
		/// The adorner panel containing the adorner that was hit.
		/// </summary>
		public AdornerPanel AdornerHit {
			get { return _adornerHit; }
			set { _adornerHit = value; }
		}
		
		/// <summary>
		/// The model item that was hit.
		/// </summary>
		public DesignItem ModelHit {
			get { return _modelHit; }
			set { _modelHit = value; }
		}
		
		/// <summary>
		/// Create a new DesignPanelHitTestResult instance.
		/// </summary>
		public DesignPanelHitTestResult(Visual visualHit)
		{
			this._visualHit = visualHit;
			this._adornerHit = null;
			this._modelHit = null;
		}
		
		#region Equals and GetHashCode implementation
		// The code in this region is useful if you want to use this structure in collections.
		// If you don't need it, you can just remove the region and the ": IEquatable<DesignPanelHitTestResult>" declaration.
		
		/// <summary>
		/// Tests if this hit test result equals the other result.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is DesignPanelHitTestResult)
				return Equals((DesignPanelHitTestResult)obj); // use Equals method below
			else
				return false;
		}
		
		/// <summary>
		/// Tests if this hit test result equals the other result.
		/// </summary>
		public bool Equals(DesignPanelHitTestResult other)
		{
			// add comparisions for all members here
			return _visualHit == other._visualHit && _adornerHit == other._adornerHit && _modelHit == other._modelHit;
		}
		
		/// <summary>
		/// Gets the hash code.
		/// </summary>
		public override int GetHashCode()
		{
			// combine the hash codes of all members here (e.g. with XOR operator ^)
			return (_visualHit != null ? _visualHit.GetHashCode() : 0)
				^ (_adornerHit != null ? _adornerHit.GetHashCode() : 0)
				^ (_modelHit != null ? _modelHit.GetHashCode() : 0);
		}
		
		/// <summary/>
		public static bool operator ==(DesignPanelHitTestResult lhs, DesignPanelHitTestResult rhs)
		{
			return lhs.Equals(rhs);
		}
		
		/// <summary/>
		public static bool operator !=(DesignPanelHitTestResult lhs, DesignPanelHitTestResult rhs)
		{
			return !(lhs.Equals(rhs)); // use operator == and negate result
		}
		#endregion
	}
}
