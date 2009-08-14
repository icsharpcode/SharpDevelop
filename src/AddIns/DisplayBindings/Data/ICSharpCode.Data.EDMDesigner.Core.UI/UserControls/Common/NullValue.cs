#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common
{
    internal class NullValue
    {
        private NullValue()
        {
        }

        internal static NullValue[] GetValues(string nullText)
        {
            return new[] { new NullValue { NullText = nullText } };
        }

        public string NullText { get; set; }

        public override string ToString()
        {
            return NullText;
        }
    }
}
