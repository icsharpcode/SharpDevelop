using System;
using ICSharpCode.Core;

namespace ICSharpCode.WixBinding.Options
{
    public class WixIdeOptions
    {
        public static readonly string OptionsProperty = "ICSharpCode.Wix.Options";
		
		static Properties properties;
		
		static WixIdeOptions()
		{
			properties = PropertyService.Get(OptionsProperty, new Properties());
		}
		
		public static bool UseSharpDevelopWix
		{
		    get { return properties.Get("UseSharpDevelopWix", true); }
		    set { properties.Set("UseSharpDevelopWix", value); }
		}
    }
}
