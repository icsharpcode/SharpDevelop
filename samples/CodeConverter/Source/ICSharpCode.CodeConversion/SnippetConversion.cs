using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace ICSharpCode.CodeConversion
{
    public class ConvertCSharpSnippetToVbNet : IConvertCode
    {
        public bool Convert(string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            CodeSnippetConverter converter = new CodeSnippetConverter() { ReferencedContents = ReferencedContentsSingleton.ReferencedContents };
            ConvertedSource = converter.CSharpToVB(ProvidedSource, out ErrorMessage);
            return (ErrorMessage.Length == 0);
        }
    }

    public class ConvertVbNetSnippetToCSharp : IConvertCode
    {
        public bool Convert(string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            CodeSnippetConverter converter = new CodeSnippetConverter() { ReferencedContents = ReferencedContentsSingleton.ReferencedContents };
            ConvertedSource = converter.VBToCSharp(ProvidedSource, out ErrorMessage);
            return (ErrorMessage.Length == 0);
        }
    }

    public class ConvertCSharpToBoo : IConvertCode
    {
        public bool Convert(string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            bool bSuccessfulConversion = BooHelpers.ConvertToBoo("convert.cs",
                ProvidedSource,
                out ConvertedSource,
                out ErrorMessage);

            return bSuccessfulConversion;
        }
    }

    public class ConvertVbNetToBoo : IConvertCode
    {
        public bool Convert(string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            bool bSuccessfulConversion = BooHelpers.ConvertToBoo("convert.vb",
                ProvidedSource,
                out ConvertedSource,
                out ErrorMessage);

            return bSuccessfulConversion;
        }
    }

    public class ConvertCSharpToPython : IConvertCode
    {
        public bool Convert(string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            return PythonHelpers.Convert(SupportedLanguage.CSharp, ProvidedSource, out ConvertedSource, out ErrorMessage);
        }
    }

    public class ConvertCSharpToRuby : IConvertCode
    {
        public bool Convert(string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            return RubyHelpers.Convert(SupportedLanguage.CSharp, ProvidedSource, out ConvertedSource, out ErrorMessage);
        }
    }

    public class ConvertVbNetToPython : IConvertCode
    {
        public bool Convert(string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            return PythonHelpers.Convert(SupportedLanguage.VBNet, ProvidedSource, out ConvertedSource, out ErrorMessage);
        }
    }

    public class ConvertVbNetToRuby : IConvertCode
    {
        public bool Convert(string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            return RubyHelpers.Convert(SupportedLanguage.VBNet, ProvidedSource, out ConvertedSource, out ErrorMessage);
        }
    }
}