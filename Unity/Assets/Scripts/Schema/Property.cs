using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Schema
{
    public class Property
    {
        #region Caching

        private static Dictionary<string, Property> _properties;

        public static Property GetProperty(JToken property)
        {
            if (_properties == null)
            {
                _properties = new Dictionary<string, Property>();
            }

            string propertyId = (string) property["_id"];
            Property dProperty = _properties[propertyId];

            if (dProperty == null)
            {
                Property newProperty = new Property(property);
                _properties.Add(propertyId, newProperty);
                dProperty = newProperty;
            }

            return dProperty;
        }

        #endregion

        private string _id;
        private string _name;
        private User _owner;
        private bool _mortgaged;

        public string Name => _name;
        public User Owner => _owner;
        public bool Mortgaged => _mortgaged;

        private Property(JToken property)
        {
            _name = (string) property["name"];
            _owner = User.GetUser(property["owner"]);
        }
    }
}