using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Account State Converter
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    internal class AccountStateConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AccountState);
            //||
            //objectType == typeof(TvAccountState) ||
            //objectType == typeof(TvEpisodeAccountState) ||
            //objectType == typeof(TvEpisodeAccountStateWithNumber);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            // Sometimes the AccountState.Rated is an object with a value in it
            // In these instances, convert it from:
            //  "rated": { "value": 5 }
            //  "rated": False
            // To:
            //  "rating": 5
            //  "rating": null

            JToken obj = jObject["rated"];
            if (obj.Type == JTokenType.Boolean)
            {
                // It's "False", so the rating is not set
                jObject.Remove("rated");
                jObject.Add("rating", null);
            }
            else if (obj.Type == JTokenType.Object)
            {
                // Read out the value
                double rating = obj["value"].ToObject<double>();
                jObject.Remove("rated");
                jObject.Add("rating", rating);
            }

            object result = Activator.CreateInstance(objectType);

            // Populate the result
            serializer.Populate(jObject.CreateReader(), result);

            return result;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="NotImplementedException"></exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
