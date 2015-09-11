using System.Collections.Generic;
using System.Xml.Serialization;

namespace SimpleRenamer.Framework
{
    public class RegexFile
    {
        public List<RegexExpression> RegexExpressions { get; set; }
    }

    public class RegexExpression
    {
        [XmlText]
        public string Expression { get; set; }
        [XmlAttribute]
        public bool IsEnabled { get; set; }

        public RegexExpression(string exp, bool en)
        {
            Expression = exp;
            IsEnabled = en;
        }

        public RegexExpression()
        {
        }
    }
}
