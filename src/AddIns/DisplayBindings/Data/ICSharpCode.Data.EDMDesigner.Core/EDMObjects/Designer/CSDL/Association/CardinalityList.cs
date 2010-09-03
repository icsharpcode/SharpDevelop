// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Association
{
    public class CardinalityList : List<CardinalityItem>
    {
        private static CardinalityList _instance;
        /// <summary>
        /// It isn't a thread safe singleton but it isn't a problem here
        /// </summary>
        public static CardinalityList Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CardinalityList() 
                    { 
                        new CardinalityItem { Value = ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common.Cardinality.One, Text = "1" }, 
                        new CardinalityItem { Value = ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common.Cardinality.ZeroToOne, Text = "0..1" }, 
                        new CardinalityItem { Value = ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common.Cardinality.Many, Text="*" }
                    };
                return _instance;
            }
        }
    }
}
