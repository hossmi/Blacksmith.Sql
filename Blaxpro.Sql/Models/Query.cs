using System.Collections.Generic;

namespace Blaxpro.Sql.Models
{
    public class Query : IQuery
    {
        private readonly Dictionary<string, object> parameters;

        public Query(string statement)
        {
            this.Statement = statement;
            this.parameters = new Dictionary<string, object>();
        }

        public string Statement { get; set; }
        public IEnumerable<KeyValuePair<string, object>> Parameters => this.parameters;

        public object this[string parameter]
        {
            get => this.parameters[parameter];
            set => this.parameters[parameter] = value;
        }

        public void clearParameters()
        {
            this.parameters.Clear();
        }

        public static implicit operator Query(string query)
        {
            return new Query(query);
        }
    }
}