using System;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;


namespace AvalonDock
{
  /// <summary>
  /// Image control that get's greyed out when disabled.
  /// This control is intended to be used for toolbar, menu or button icons where ability of an icon to 
  /// grey itself out when disabled is essential.
  /// <remarks>
  /// 1) Greyscale image is created using FormatConvertedBitmap class. Unfortunately when converting the
  ///    image to greyscale this class does n0t preserve transparency information. To overcome that, there is 
  ///    an opacity mask created from original image that is applied to greyscale image in order to preserve
  ///    transparency information. Because of that if an OpacityMask is applied to original image that mask 
  ///    has to be combined with that special opacity mask of greyscale image in order to make a proper 
  ///    greyscale image look. If you know how to combine two opacity masks please let me know.
  /// 2) DrawingImage source is not supported at the moment.
  /// 3) Have not tried to use any BitmapSource derived sources accept for BitmapImage so it may not be 
  ///    able to convert some of them to greyscale.
  /// 4) When specifying source Uri from XAML try to use Absolute Uri otherwise the greyscale image
  ///    may not be created in some scenarious. There is some code to improve the situation but I cannot 
  ///    guarantee it will work in all possible scenarious.
  /// 5) In case the greyscaled version cannot be created for whatever reason the original image with 
  ///    60% opacity (i.e. dull colours) will be used instead (that will work even with the DrawingImage 
  ///    source).
  /// </remarks>
  /// </summary>
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
