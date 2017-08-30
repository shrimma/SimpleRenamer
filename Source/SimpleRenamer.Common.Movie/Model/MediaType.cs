namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Media Type
    /// </summary>
    public enum MediaType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Movie
        /// </summary>
        [EnumValue("movie")]
        Movie = 1,

        /// <summary>
        /// TV
        /// </summary>
        [EnumValue("tv")]
        Tv = 2,

        /// <summary>
        /// Person
        /// </summary>
        [EnumValue("person")]
        Person = 3
    }
}
