// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls
{
    /// <summary>
    /// <remarks>
    /// Alex P's code
    /// </remarks>
    /// </summary>
    /// <seealso cref="http://www.codeplex.com/GreyableImage/SourceControl/changeset/view/16327#237393"/>
    public class GreyableImage : Image
    {
        // these are holding references to original and greyscale ImageSources
        private ImageSource _sourceC, _sourceG;
        // these are holding original and greyscale opacity masks
        private Brush _opacityMaskC, _opacityMaskG;

        static GreyableImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GreyableImage), new FrameworkPropertyMetadata(typeof(GreyableImage)));
        }

        /// <summary>
        /// Overwritten to handle changes of IsEnabled, Source and OpacityMask properties
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name.Equals("IsEnabled"))
            {
                if ((e.NewValue as bool?) == false)
                {
                    Source = _sourceG;
                    OpacityMask = _opacityMaskG;
                }
                else if ((e.NewValue as bool?) == true)
                {
                    Source = _sourceC;
                    OpacityMask = _opacityMaskC;
                }
            }
            else if (e.Property.Name.Equals("Source") &&
                     !object.ReferenceEquals(Source, _sourceC) &&
                     !object.ReferenceEquals(Source, _sourceG))  // only recache Source if it's the new one from outside
            {
                SetSources();
            }
            else if (e.Property.Name.Equals("OpacityMask") &&
                     !object.ReferenceEquals(OpacityMask, _opacityMaskC) &&
                     !object.ReferenceEquals(OpacityMask, _opacityMaskG)) // only recache opacityMask if it's the new one from outside
            {
                _opacityMaskC = OpacityMask;
            }

            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Cashes original ImageSource, creates and caches greyscale ImageSource and greyscale opacity mask
        /// </summary>
        private void SetSources()
        {
            // in case greyscale image cannot be created set greyscale source to original Source first
            _sourceG = _sourceC = Source;

            // create Opacity Mask for greyscale image as FormatConvertedBitmap does not keep transparency info
            _opacityMaskG = new ImageBrush(_sourceC);
            _opacityMaskG.Opacity = 0.6;

            try
            {
                // get the string Uri for the original image source first
                String stringUri = TypeDescriptor.GetConverter(Source).ConvertTo(Source, typeof(string)) as string;
                Uri uri = null;
                // try to resolve it as an absolute Uri (if it is relative and used it as is
                // it is likely to point in a wrong direction)
                if (!Uri.TryCreate(stringUri, UriKind.Absolute, out uri))
                {
                    // it seems that the Uri is relative, at this stage we can only assume that
                    // the image requested is in the same assembly as this oblect,
                    // so we modify the string Uri to make it absolute ...
                    stringUri = "pack://application:,,,/" + stringUri.TrimStart(new char[2] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar });

                    // ... and try to resolve again
                    uri = new Uri(stringUri);
                }

                // create and cache greyscale ImageSource
                _sourceG = new FormatConvertedBitmap(new BitmapImage(uri), PixelFormats.Gray8, null, 0);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Fail("The Image used cannot be greyed out.",
                                              "Use BitmapImage or URI as a Source in order to allow greyscaling. Make sure the absolute Uri is used as relative Uri may sometimes resolve incorrectly.\n\nException: " + e.Message);
            }
        }
    }
}
