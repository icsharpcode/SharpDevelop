// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace ICSharpCode.CodeQualityAnalysis.Utility.Localizeable
{
	/// <summary>
	/// Description of LocalizableDescriptionAttribute.
	/// http://www.codeproject.com/KB/WPF/FriendlyEnums.aspx
	/// </summary>
	/// 
	
	[AttributeUsage(AttributeTargets.All,Inherited = false,AllowMultiple = true)]
	public sealed class LocalizableDescriptionAttribute : DescriptionAttribute
	{
		#region Public methods.
		// ------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="LocalizableDescriptionAttribute"/> class.
		/// </summary>
		/// <param name="description">The description.</param>
		/// <param name="resourcesType">Type of the resources.</param>
		public LocalizableDescriptionAttribute
			(string description,System.Type resourcesType) : base(description)
		{
//			_resourcesType = resourcesType;
		}
		
		
	public LocalizableDescriptionAttribute (string description) : base(description)
			
		{
//			_resourcesType = resourcesType;
		}
		#endregion

		#region Public properties.

		/// <summary>
		/// Get the string value from the resources.
		/// </summary>
		/// <value></value>
		/// <returns>The description stored in this attribute.</returns>
		/*
		public override string Description
        {
            get
            {
                if (!_isLocalized)
                {
                    ResourceManager resMan =
                         _resourcesType.InvokeMember(
                         @"ResourceManager",
                         BindingFlags.GetProperty | BindingFlags.Static |
                         BindingFlags.Public | BindingFlags.NonPublic,
                         null,
                         null,
                         new object[] { }) as ResourceManager;

                    CultureInfo culture =
                         _resourcesType.InvokeMember(
                         @"Culture",
                         BindingFlags.GetProperty | BindingFlags.Static |
                         BindingFlags.Public | BindingFlags.NonPublic,
                         null,
                         null,
                         new object[] { }) as CultureInfo;

                    _isLocalized = true;

                    if (resMan != null)
                    {
                        DescriptionValue =
                             resMan.GetString(DescriptionValue, culture);
                    }
                }

                return DescriptionValue;
            }
        }
		*/
		
		public override string Description
		{
			get
			{
				if (!_isLocalized)
				{
					_isLocalized = true;
					DescriptionValue = Description;

				}
				return DescriptionValue;
			}
		}
		#endregion

		#region Private variables.

//		private readonly Type _resourcesType;
		private bool _isLocalized;

		#endregion
	}
}
