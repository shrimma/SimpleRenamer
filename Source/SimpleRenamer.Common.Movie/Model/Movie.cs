using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Movie
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.Movie}" />
    public class Movie : IEquatable<Movie>
    {
        /// <summary>
        /// Gets or sets the account states.
        /// </summary>
        /// <value>
        /// The account states.
        /// </value>
        [JsonProperty("account_states")]
        public AccountState AccountStates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Movie"/> is adult.
        /// </summary>
        /// <value>
        ///   <c>true</c> if adult; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("adult")]
        public bool Adult { get; set; }

        /// <summary>
        /// Gets or sets the alternative titles.
        /// </summary>
        /// <value>
        /// The alternative titles.
        /// </value>
        [JsonProperty("alternative_titles")]
        public AlternativeTitles AlternativeTitles { get; set; }

        /// <summary>
        /// Gets or sets the backdrop path.
        /// </summary>
        /// <value>
        /// The backdrop path.
        /// </value>
        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        /// <summary>
        /// Gets or sets the belongs to collection.
        /// </summary>
        /// <value>
        /// The belongs to collection.
        /// </value>
        [JsonProperty("belongs_to_collection")]
        public SearchCollection BelongsToCollection { get; set; }

        /// <summary>
        /// Gets or sets the budget.
        /// </summary>
        /// <value>
        /// The budget.
        /// </value>
        [JsonProperty("budget")]
        public long Budget { get; set; }

        /// <summary>
        /// Gets or sets the changes.
        /// </summary>
        /// <value>
        /// The changes.
        /// </value>
        [JsonProperty("changes")]
        public ChangesContainer Changes { get; set; }

        /// <summary>
        /// Gets or sets the credits.
        /// </summary>
        /// <value>
        /// The credits.
        /// </value>
        [JsonProperty("credits")]
        public Credits Credits { get; set; }

        /// <summary>
        /// Gets or sets the genres.
        /// </summary>
        /// <value>
        /// The genres.
        /// </value>
        [JsonProperty("genres")]
        public List<Genre> Genres { get; set; } = new List<Genre>();

        /// <summary>
        /// Gets or sets the homepage.
        /// </summary>
        /// <value>
        /// The homepage.
        /// </value>
        [JsonProperty("homepage")]
        public string Homepage { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        /// <value>
        /// The images.
        /// </value>
        [JsonProperty("images")]
        public Images Images { get; set; }

        /// <summary>
        /// Gets or sets the imdb identifier.
        /// </summary>
        /// <value>
        /// The imdb identifier.
        /// </value>
        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>
        /// The keywords.
        /// </value>
        [JsonProperty("keywords")]
        public KeywordsContainer Keywords { get; set; }

        /// <summary>
        /// Gets or sets the lists.
        /// </summary>
        /// <value>
        /// The lists.
        /// </value>
        [JsonProperty("lists")]
        public SearchContainer<ListResult> Lists { get; set; }

        /// <summary>
        /// Gets or sets the original language.
        /// </summary>
        /// <value>
        /// The original language.
        /// </value>
        [JsonProperty("original_language")]
        public string OriginalLanguage { get; set; }

        /// <summary>
        /// Gets or sets the original title.
        /// </summary>
        /// <value>
        /// The original title.
        /// </value>
        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }

        /// <summary>
        /// Gets or sets the overview.
        /// </summary>
        /// <value>
        /// The overview.
        /// </value>
        [JsonProperty("overview")]
        public string Overview { get; set; }

        /// <summary>
        /// Gets or sets the popularity.
        /// </summary>
        /// <value>
        /// The popularity.
        /// </value>
        [JsonProperty("popularity")]
        public double Popularity { get; set; }

        /// <summary>
        /// Gets or sets the poster path.
        /// </summary>
        /// <value>
        /// The poster path.
        /// </value>
        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        /// <summary>
        /// Gets or sets the production companies.
        /// </summary>
        /// <value>
        /// The production companies.
        /// </value>
        [JsonProperty("production_companies")]
        public List<ProductionCompany> ProductionCompanies { get; set; } = new List<ProductionCompany>();

        /// <summary>
        /// Gets or sets the production countries.
        /// </summary>
        /// <value>
        /// The production countries.
        /// </value>
        [JsonProperty("production_countries")]
        public List<ProductionCountry> ProductionCountries { get; set; } = new List<ProductionCountry>();

        /// <summary>
        /// Gets or sets the release date.
        /// </summary>
        /// <value>
        /// The release date.
        /// </value>
        [JsonProperty("release_date")]
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets the release dates.
        /// </summary>
        /// <value>
        /// The release dates.
        /// </value>
        [JsonProperty("release_dates")]
        public ResultContainer<ReleaseDatesContainer> ReleaseDates { get; set; }

        /// <summary>
        /// Gets or sets the releases.
        /// </summary>
        /// <value>
        /// The releases.
        /// </value>
        [JsonProperty("releases")]
        public Releases Releases { get; set; }

        /// <summary>
        /// Gets or sets the revenue.
        /// </summary>
        /// <value>
        /// The revenue.
        /// </value>
        [JsonProperty("revenue")]
        public long Revenue { get; set; }

        /// <summary>
        /// Gets or sets the reviews.
        /// </summary>
        /// <value>
        /// The reviews.
        /// </value>
        [JsonProperty("reviews")]
        public SearchContainer<ReviewBase> Reviews { get; set; }

        /// <summary>
        /// Gets or sets the runtime.
        /// </summary>
        /// <value>
        /// The runtime.
        /// </value>
        [JsonProperty("runtime")]
        public int? Runtime { get; set; }

        /// <summary>
        /// Gets or sets the similar.
        /// </summary>
        /// <value>
        /// The similar.
        /// </value>
        [JsonProperty("similar")]
        public SearchContainer<SearchMovie> Similar { get; set; }

        /// <summary>
        /// Gets or sets the spoken languages.
        /// </summary>
        /// <value>
        /// The spoken languages.
        /// </value>
        [JsonProperty("spoken_languages")]
        public List<SpokenLanguage> SpokenLanguages { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the tagline.
        /// </summary>
        /// <value>
        /// The tagline.
        /// </value>
        [JsonProperty("tagline")]
        public string Tagline { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the translations.
        /// </summary>
        /// <value>
        /// The translations.
        /// </value>
        [JsonProperty("translations")]
        public TranslationsContainer Translations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Movie"/> is video.
        /// </summary>
        /// <value>
        ///   <c>true</c> if video; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("video")]
        public bool Video { get; set; }

        /// <summary>
        /// Gets or sets the videos.
        /// </summary>
        /// <value>
        /// The videos.
        /// </value>
        [JsonProperty("videos")]
        public ResultContainer<Video> Videos { get; set; }

        /// <summary>
        /// Gets or sets the vote average.
        /// </summary>
        /// <value>
        /// The vote average.
        /// </value>
        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        /// <summary>
        /// Gets or sets the vote count.
        /// </summary>
        /// <value>
        /// The vote count.
        /// </value>
        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        #region Equality
        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return this.Equals(obj as Movie);
        }

        /// <summary>
        /// Returns true if <see cref="Movie"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="Movie"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Movie other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.AccountStates == other.AccountStates ||
                    this.AccountStates != null &&
                    this.AccountStates.Equals(other.AccountStates)
                ) &&
                (
                    this.Adult == other.Adult ||
                    this.Adult.Equals(other.Adult)
                ) &&
                (
                    this.AlternativeTitles == other.AlternativeTitles ||
                    this.AlternativeTitles != null &&
                    this.AlternativeTitles.Equals(other.AlternativeTitles)
                ) &&
                (
                    this.BackdropPath == other.BackdropPath ||
                    this.BackdropPath != null &&
                    this.BackdropPath.Equals(other.BackdropPath)
                ) &&
                (
                    this.BelongsToCollection == other.BelongsToCollection ||
                    this.BelongsToCollection != null &&
                    this.BelongsToCollection.Equals(other.BelongsToCollection)
                ) &&
                (
                    this.Budget == other.Budget ||
                    this.Budget.Equals(other.Budget)
                ) &&
                (
                    this.Changes == other.Changes ||
                    this.Changes != null &&
                    this.Changes.Equals(other.Changes)
                ) &&
                (
                    this.Credits == other.Credits ||
                    this.Credits != null &&
                    this.Credits.Equals(other.Credits)
                ) &&
                (
                    this.Genres == other.Genres ||
                    this.Genres != null &&
                    this.Genres.SequenceEqual(other.Genres)
                ) &&
                (
                    this.Homepage == other.Homepage ||
                    this.Homepage != null &&
                    this.Homepage.Equals(other.Homepage)
                ) &&
                (
                    this.Id == other.Id ||
                    this.Id.Equals(other.Id)
                ) &&
                (
                    this.Images == other.Images ||
                    this.Images != null &&
                    this.Images.Equals(other.Images)
                ) &&
                (
                    this.ImdbId == other.ImdbId ||
                    this.ImdbId != null &&
                    this.ImdbId.Equals(other.ImdbId)
                ) &&
                (
                    this.Keywords == other.Keywords ||
                    this.Keywords != null &&
                    this.Keywords.Equals(other.Keywords)
                ) &&
                (
                    this.Lists == other.Lists ||
                    this.Lists != null &&
                    this.Lists.Equals(other.Lists)
                ) &&
                (
                    this.OriginalLanguage == other.OriginalLanguage ||
                    this.OriginalLanguage != null &&
                    this.OriginalLanguage.Equals(other.OriginalLanguage)
                ) &&
                (
                    this.OriginalTitle == other.OriginalTitle ||
                    this.OriginalTitle != null &&
                    this.OriginalTitle.Equals(other.OriginalTitle)
                ) &&
                (
                    this.Overview == other.Overview ||
                    this.Overview != null &&
                    this.Overview.Equals(other.Overview)
                ) &&
                (
                    this.Popularity == other.Popularity ||
                    this.Popularity.Equals(other.Popularity)
                ) &&
                (
                    this.PosterPath == other.PosterPath ||
                    this.PosterPath != null &&
                    this.PosterPath.Equals(other.PosterPath)
                ) &&
                (
                    this.ProductionCompanies == other.ProductionCompanies ||
                    this.ProductionCompanies != null &&
                    this.ProductionCompanies.SequenceEqual(other.ProductionCompanies)
                ) &&
                (
                    this.ProductionCountries == other.ProductionCountries ||
                    this.ProductionCountries != null &&
                    this.ProductionCountries.SequenceEqual(other.ProductionCountries)
                ) &&
                (
                    this.ReleaseDate == other.ReleaseDate ||
                    this.ReleaseDate != null &&
                    this.ReleaseDate.Equals(other.ReleaseDate)
                ) &&
                (
                    this.ReleaseDates == other.ReleaseDates ||
                    this.ReleaseDates != null &&
                    this.ReleaseDates.Equals(other.ReleaseDates)
                ) &&
                (
                    this.Releases == other.Releases ||
                    this.Releases != null &&
                    this.Releases.Equals(other.Releases)
                ) &&
                (
                    this.Revenue == other.Revenue ||
                    this.Revenue.Equals(other.Revenue)
                ) &&
                (
                    this.Reviews == other.Reviews ||
                    this.Reviews != null &&
                    this.Reviews.Equals(other.Reviews)
                ) &&
                (
                    this.Runtime == other.Runtime ||
                    this.Runtime != null &&
                    this.Runtime.Equals(other.Runtime)
                ) &&
                (
                    this.Similar == other.Similar ||
                    this.Similar != null &&
                    this.Similar.Equals(other.Similar)
                ) &&
                (
                    this.SpokenLanguages == other.SpokenLanguages ||
                    this.SpokenLanguages != null &&
                    this.SpokenLanguages.SequenceEqual(other.SpokenLanguages)
                ) &&
                (
                    this.Status == other.Status ||
                    this.Status != null &&
                    this.Status.Equals(other.Status)
                ) &&
                (
                    this.Tagline == other.Tagline ||
                    this.Tagline != null &&
                    this.Tagline.Equals(other.Tagline)
                ) &&
                (
                    this.Title == other.Title ||
                    this.Title != null &&
                    this.Title.Equals(other.Title)
                ) &&
                (
                    this.Translations == other.Translations ||
                    this.Translations != null &&
                    this.Translations.Equals(other.Translations)
                ) &&
                (
                    this.Video == other.Video ||
                    this.Video.Equals(other.Video)
                ) &&
                (
                    this.Videos == other.Videos ||
                    this.Videos != null &&
                    this.Videos.Equals(other.Videos)
                ) &&
                (
                    this.VoteAverage == other.VoteAverage ||
                    this.VoteAverage.Equals(other.VoteAverage)
                ) &&
                (
                    this.VoteCount == other.VoteCount ||
                    this.VoteCount.Equals(other.VoteCount)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)2166136261;
                // Suitable nullity checks etc, of course :)
                if (this.AccountStates != null)
                {
                    hash = (hash * 16777619) + this.AccountStates.GetHashCode();
                }
                hash = (hash * 16777619) + this.Adult.GetHashCode();
                if (this.AlternativeTitles != null)
                {
                    hash = (hash * 16777619) + this.AlternativeTitles.GetHashCode();
                }
                if (this.BackdropPath != null)
                {
                    hash = (hash * 16777619) + this.BackdropPath.GetHashCode();
                }
                if (this.BelongsToCollection != null)
                {
                    hash = (hash * 16777619) + this.BelongsToCollection.GetHashCode();
                }
                hash = (hash * 16777619) + this.Budget.GetHashCode();
                if (this.Changes != null)
                {
                    hash = (hash * 16777619) + this.Changes.GetHashCode();
                }
                if (this.Credits != null)
                {
                    hash = (hash * 16777619) + this.Credits.GetHashCode();
                }
                if (this.Genres != null)
                {
                    foreach (var item in Genres)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                if (this.Homepage != null)
                {
                    hash = (hash * 16777619) + this.Homepage.GetHashCode();
                }
                hash = (hash * 16777619) + this.Id.GetHashCode();
                if (this.Images != null)
                {
                    hash = (hash * 16777619) + this.Images.GetHashCode();
                }
                if (this.ImdbId != null)
                {
                    hash = (hash * 16777619) + this.ImdbId.GetHashCode();
                }
                if (this.Keywords != null)
                {
                    hash = (hash * 16777619) + this.Keywords.GetHashCode();
                }
                if (this.Lists != null)
                {
                    hash = (hash * 16777619) + this.Lists.GetHashCode();
                }
                if (this.OriginalLanguage != null)
                {
                    hash = (hash * 16777619) + this.OriginalLanguage.GetHashCode();
                }
                if (this.OriginalTitle != null)
                {
                    hash = (hash * 16777619) + this.OriginalTitle.GetHashCode();
                }
                if (this.Overview != null)
                {
                    hash = (hash * 16777619) + this.Overview.GetHashCode();
                }
                hash = (hash * 16777619) + this.Popularity.GetHashCode();
                if (this.PosterPath != null)
                {
                    hash = (hash * 16777619) + this.PosterPath.GetHashCode();
                }
                if (this.ProductionCompanies != null)
                {
                    foreach (var item in ProductionCompanies)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                if (this.ProductionCountries != null)
                {
                    foreach (var item in ProductionCountries)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                if (this.ReleaseDate != null)
                {
                    hash = (hash * 16777619) + this.ReleaseDate.GetHashCode();
                }
                if (this.ReleaseDates != null)
                {
                    hash = (hash * 16777619) + this.ReleaseDates.GetHashCode();
                }
                if (this.Releases != null)
                {
                    hash = (hash * 16777619) + this.Releases.GetHashCode();
                }

                hash = (hash * 16777619) + this.Revenue.GetHashCode();
                if (this.Reviews != null)
                {
                    hash = (hash * 16777619) + this.Reviews.GetHashCode();
                }
                if (this.Runtime != null)
                {
                    hash = (hash * 16777619) + this.Runtime.GetHashCode();
                }
                if (this.Similar != null)
                {
                    hash = (hash * 16777619) + this.Similar.GetHashCode();
                }
                if (this.SpokenLanguages != null)
                {
                    foreach (var item in SpokenLanguages)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                if (this.Status != null)
                {
                    hash = (hash * 16777619) + this.Status.GetHashCode();
                }
                if (this.Tagline != null)
                {
                    hash = (hash * 16777619) + this.Tagline.GetHashCode();
                }
                if (this.Title != null)
                {
                    hash = (hash * 16777619) + this.Title.GetHashCode();
                }
                if (this.Translations != null)
                {
                    hash = (hash * 16777619) + this.Translations.GetHashCode();
                }
                hash = (hash * 16777619) + this.Video.GetHashCode();
                if (this.Videos != null)
                {
                    hash = (hash * 16777619) + this.Videos.GetHashCode();
                }
                hash = (hash * 16777619) + this.VoteAverage.GetHashCode();
                hash = (hash * 16777619) + this.VoteCount.GetHashCode();
                return hash;
            }
        }
        #endregion Equality
    }
}
