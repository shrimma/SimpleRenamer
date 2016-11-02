using System.Xml.Serialization;

namespace SimpleRenamer.Framework.DataModel
{
    public class RegexExpression
    {
        [XmlText]
        public string Expression { get; set; }
        [XmlAttribute]
        public bool IsEnabled { get; set; }
        [XmlAttribute]
        public bool IsForTvShow { get; set; }

        public RegexExpression(string exp, bool en, bool tv)
        {
            Expression = exp;
            IsEnabled = en;
            IsForTvShow = tv;
        }

        public RegexExpression()
        {
        }
    }
}
