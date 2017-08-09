namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// 
    /// </summary>
    public enum ChangeAction
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Added
        /// </summary>
        [EnumValue("added")]
        Added = 1,

        /// <summary>
        /// Created
        /// </summary>
        [EnumValue("created")]
        Created = 2,

        /// <summary>
        /// Updated
        /// </summary>
        [EnumValue("updated")]
        Updated = 3,

        /// <summary>
        /// Deleted
        /// </summary>
        [EnumValue("deleted")]
        Deleted = 4
    }
}
