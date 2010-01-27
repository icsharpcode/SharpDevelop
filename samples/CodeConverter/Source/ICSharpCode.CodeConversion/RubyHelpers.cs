using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;

namespace ICSharpCode.CodeConversion
{
    public class RubyHelpers
    {
        public static bool Convert(SupportedLanguage inputLanguage, string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            NRefactoryToRubyConverter converter = new
                    NRefactoryToRubyConverter(inputLanguage);

            string convertedCode = converter.Convert(ProvidedSource);

            ConvertedSource = convertedCode;
            ErrorMessage = "";

            return true;
        }
    }
}
