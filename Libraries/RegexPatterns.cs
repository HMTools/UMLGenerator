using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace UMLGenerator.Libraries
{
    public static class RegexPatterns
    {
        public static string GetAccessModifier(string statement)
        {
            var match =  Regex.Match(statement, @"(^| +)(?<AcessModifier>public|(protected internal)|protected|internal|private|(private protected)) +");
            return match.Success ? match.Groups["AcessModifier"].Value : "";
        }
    }
}
