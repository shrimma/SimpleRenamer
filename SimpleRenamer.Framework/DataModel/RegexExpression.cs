using System.Xml.Serialization;

namespace SimpleRenamer.Framework.DataModel
{
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
