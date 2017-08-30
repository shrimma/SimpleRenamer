using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Search Base Converter
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    internal class SearchBaseConverter : JsonConverter
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
            return objectType == typeof(SearchBase);
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
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            SearchBase result;
            if (jObject["media_type"] == null)
            {
                // We cannot determine the correct type, let's hope we were provided one
                result = (SearchBase)Activator.CreateInstance(objectType);
            }
            else
            {
                // Determine the type based on the media_type
                MediaType mediaType = jObject["media_type"].ToObject<MediaType>();

                switch (mediaType)
                {
                    case MediaType.Movie:
                        result = new SearchMovie();
                        break;
                    case MediaType.Tv:
                        result = new SearchMovie();
                        break;
                    case MediaType.Person:
                        result = new SearchMovie();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

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
