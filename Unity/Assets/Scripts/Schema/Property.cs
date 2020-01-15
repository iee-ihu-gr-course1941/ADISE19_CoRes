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

            if (!_properties.ContainsKey(propertyId))
            {
                Property newProperty = new Property(property);
                _properties.Add(propertyId, newProperty);
            }

            return _properties[propertyId];
        }

        #endregion

        private string _id;
        private string _name;
        private string _ownerId;
        private bool _mortgaged;
        private int _location;

        public string Name => _name;
        public string OwnerId => _ownerId;
        public bool Mortgaged => _mortgaged;
        public int Location => _location;

        private Property(JToken property)
        {
            _name = (string) property["name"];
            _ownerId = (string) property["owner"];
            _mortgaged = (bool) property["mortgaged"];
            _location = (int) property["location"];
        }
    }
}