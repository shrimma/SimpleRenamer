﻿namespace SimpleRenamer.Framework.TmdbModel
{
    public enum ChangeAction
    {
        Unknown,

        [EnumValue("added")]
        Added = 1,

        [EnumValue("created")]
        Created = 2,

        [EnumValue("updated")]
        Updated = 3,

        [EnumValue("deleted")]
        Deleted = 4
    }
}
