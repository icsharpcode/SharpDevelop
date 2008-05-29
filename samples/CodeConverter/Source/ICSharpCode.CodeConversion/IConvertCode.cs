using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.CodeConversion
{
    public interface IConvertCode
    {
        bool Convert(string ProvidedSource,
                out string ConvertedSource,
                out string ErrorMessage);
    }
}
