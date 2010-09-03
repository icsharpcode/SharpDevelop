// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Represents an image.
	/// </summary>
	public interface IImage
	{
		/// <summary>
		/// Gets the image as WPF ImageSource.
		/// </summary>
		ImageSource ImageSource { get; }
		
		/// <summary>
		/// Gets the image as System.Drawing.Bitmap.
		/// </summary>
		Bitmap Bitmap { get; }
		
		/// <summary>
		/// Gets the image as System.Drawing.Icon.
		/// </summary>
		Icon Icon { get; }
	}
	
	/// <summary>
	/// Represents an image that gets loaded from a ResourceService.
	/// </summary>
	public class ResourceServiceImage : IImage
	{
		readonly string resourceName;
		
		/// <summary>
		/// Creates a new ResourceServiceImage.
		/// </summary>
		/// <param name="resourceName">The name of the image resource.</param>
		public ResourceServiceImage(string resourceName)
		{
			if (resourceName == null)
				throw new ArgumentNullException("resourceName");
			this.resourceName = resourceName;
		}
		
		/// <inheritdoc/>
		public ImageSource ImageSource {
			get {
				return PresentationResourceService.GetBitmapSource(resourceName);
			}
		}
		
		/// <inheritdoc/>
		public Bitmap Bitmap {
			get {
				return WinFormsResourceService.GetBitmap(resourceName);
			}
		}
		
		/// <inheritdoc/>
		public Icon Icon {
			get {
				Icon icon = WinFormsResourceService.GetIcon(resourceName);
				if (icon == null)
					throw new ResourceNotFoundException(resourceName);
				return icon;
			}
		}
	}
}
