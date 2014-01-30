// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Adorners
{
	/// <summary>
	/// Base class for extensions that present adorners on the screen.
	/// </summary>
	/// <remarks>
	/// Cider adorner introduction:
	/// http://blogs.msdn.com/jnak/archive/2006/04/24/580393.aspx
	/// <quote>
	/// Adorners are WPF elements that float on top of the design surface and track the
	/// size and location of the elements they are adorning. All of the UI the designer
	/// presents to the user, including snap lines, grab handles and grid rulers,
	/// is composed of these adorners.
	/// </quote>
	/// About design-time adorners and their placement:
	/// read http://myfun.spaces.live.com/blog/cns!AC1291870308F748!240.entry
	/// and http://myfun.spaces.live.com/blog/cns!AC1291870308F748!242.entry
	/// </remarks>
	public abstract class AdornerProvider : DefaultExtension
	{
		#region class AdornerCollection
		/// <summary>
		/// Describes a collection of adorner visuals.
		/// </summary>
		sealed class AdornerPanelCollection : Collection<AdornerPanel>
		{
			readonly AdornerProvider _provider;
			
			internal AdornerPanelCollection(AdornerProvider provider)
			{
				this._provider = provider;
			}
			
			/// <summary/>
			protected override void InsertItem(int index, AdornerPanel item)
			{
				base.InsertItem(index, item);
				_provider.OnAdornerAdd(item);
			}
			
			/// <summary/>
			protected override void RemoveItem(int index)
			{
				_provider.OnAdornerRemove(base[index]);
				base.RemoveItem(index);
			}
			
			/// <summary/>
			protected override void SetItem(int index, AdornerPanel item)
			{
				_provider.OnAdornerRemove(base[index]);
				base.SetItem(index, item);
				_provider.OnAdornerAdd(item);
			}
			
			/// <summary/>
			protected override void ClearItems()
			{
				foreach (AdornerPanel v in this) {
					_provider.OnAdornerRemove(v);
				}
				base.ClearItems();
			}
		}
		#endregion
		
		readonly AdornerPanelCollection _adorners;
		bool isVisible;
		
		/// <summary>
		/// Creates a new AdornerProvider instance.
		/// </summary>
		protected AdornerProvider()
		{
			_adorners = new AdornerPanelCollection(this);
		}
		
		/// <summary>
		/// Is called after the ExtendedItem was set.
		/// This methods displays the registered adorners
		/// </summary>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			isVisible = true;
			foreach (AdornerPanel v in _adorners) {
				OnAdornerAdd(v);
			}
		}
		
		/// <summary>
		/// Is called when the extension is removed.
		/// This method hides the registered adorners.
		/// </summary>
		protected override void OnRemove()
		{
			base.OnRemove();
			foreach (AdornerPanel v in _adorners) {
				OnAdornerRemove(v);
			}
			isVisible = false;
		}
		
		/// <summary>
		/// Gets the list of adorners displayed by this AdornerProvider.
		/// </summary>
		public Collection<AdornerPanel> Adorners {
			get { return _adorners; }
		}
		
		/// <summary>
		/// Adds an UIElement as adorner with the specified placement.
		/// </summary>
		protected void AddAdorner(AdornerPlacement placement, AdornerOrder order, UIElement adorner)
		{
			AdornerPanel p = new AdornerPanel();
			p.Order = order;
			AdornerPanel.SetPlacement(adorner, placement);
			p.Children.Add(adorner);
			this.Adorners.Add(p);
		}
		
		/// <summary>
		/// Adds several UIElements as adorners with the specified placement.
		/// </summary>
		protected void AddAdorners(AdornerPlacement placement, params UIElement[] adorners)
		{
			AdornerPanel p = new AdornerPanel();
			foreach (UIElement adorner in adorners) {
				AdornerPanel.SetPlacement(adorner, placement);
				p.Children.Add(adorner);
			}
			this.Adorners.Add(p);
		}
		
		internal void OnAdornerAdd(AdornerPanel item)
		{
			if (!isVisible) return;
			
			item.SetAdornedElement(this.ExtendedItem.View, this.ExtendedItem);
			
			IDesignPanel avs = Services.GetService<IDesignPanel>();
			avs.Adorners.Add(item);
		}
		
		internal void OnAdornerRemove(AdornerPanel item)
		{
			if (!isVisible) return;
			
			IDesignPanel avs = Services.GetService<IDesignPanel>();
			avs.Adorners.Remove(item);
		}
	}
}
