using RestSharp;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Framework.TV;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Mocks
{
    internal class TestableTvdbManager : TvdbManager
    {
        public TestableTvdbManager(IConfigurationManager configManager, IHelper helper) : base(configManager, helper)
        {
        }

        private const string _loginResponse = "{\"token\":\"jwtToken\"}";
        private const string _searchResponse = "{\"data\":[{\"aliases\":[\"string\"],\"banner\":\"string\",\"firstAired\":\"string\",\"id\":0,\"network\":\"string\",\"overview\":\"string\",\"seriesName\":\"Castle\",\"status\":\"string\"},{\"aliases\":[\"string\"],\"banner\":\"string\",\"firstAired\":\"string\",\"id\":0,\"network\":\"string\",\"overview\":\"string\",\"seriesName\":\"KillJoys\",\"status\":\"string\"}]}";
        private const string _seriesResponse = "{\"data\":{\"id\":121361,\"seriesName\":\"Game of Thrones\",\"aliases\":[],\"banner\":\"graphical/121361-g19.jpg\",\"seriesId\":\"\",\"status\":\"Continuing\",\"firstAired\":\"2011-04-17\",\"network\":\"HBO\",\"networkId\":\"\",\"runtime\":\"55\",\"genre\":[\"Adventure\",\"Drama\",\"Fantasy\"],\"overview\":\"Seven noble families fight for control of the mythical land of Westeros. Friction between the houses leads to full-scale war. All while a very ancient evil awakens in the farthest north. Amidst the war, a neglected military order of misfits, the Night's Watch, is all that stands between the realms of men and the icy horrors beyond.\",\"lastUpdated\":1503484115,\"airsDayOfWeek\":\"Sunday\",\"airsTime\":\"9:00 PM\",\"rating\":\"TV-MA\",\"imdbId\":\"tt0944947\",\"zap2itId\":\"\",\"added\":\"2009-10-26 16:51:46\",\"addedBy\":10072,\"siteRating\":9.5,\"siteRatingCount\":1735}}";
        private const string _actorsResponse = "{\"data\":[{\"id\":348788,\"seriesId\":121361,\"name\":\"Ian McElhinney\",\"role\":\"Barristan Selmy\",\"sortOrder\":3,\"image\":\"actors/348788.jpg\",\"imageAuthor\":235,\"imageAdded\":\"2015-05-10 09:07:32\",\"lastUpdated\":\"2015-05-10 09:08:41\"},{\"id\":348487,\"seriesId\":121361,\"name\":\"Ed Skrein\",\"role\":\"Daario Naharis\",\"sortOrder\":3,\"image\":\"actors/348487.jpg\",\"imageAuthor\":235,\"imageAdded\":\"2015-05-03 13:07:36\",\"lastUpdated\":\"2016-12-18 11:55:13\"}]}";
        private const string _episodesResponse = "{\"links\":{\"first\":1,\"last\":1,\"next\":null,\"prev\":null},\"data\":[{\"absoluteNumber\":null,\"airedEpisodeNumber\":1,\"airedSeason\":0,\"airedSeasonID\":137481,\"dvdEpisodeNumber\":null,\"dvdSeason\":null,\"episodeName\":\"Inside Game of Thrones\",\"firstAired\":\"2010-12-05\",\"id\":3226241,\"language\":{\"episodeName\":\"en\",\"overview\":\"en\"},\"lastUpdated\":1305321193,\"overview\":\"A short look into the film-making process for the production Game of Thrones\"},{\"absoluteNumber\":1,\"airedEpisodeNumber\":1,\"airedSeason\":1,\"airedSeasonID\":364731,\"dvdEpisodeNumber\":1,\"dvdSeason\":1,\"episodeName\":\"Winter Is Coming\",\"firstAired\":\"2011-04-17\",\"id\":3254641,\"language\":{\"episodeName\":\"en\",\"overview\":\"en\"},\"lastUpdated\":1433646412,\"overview\":\"Ned Stark, Lord of Winterfell learns that his mentor, Jon Arryn, has died and that King Robert is on his way north to offer Ned Arryn’s position as the King’s Hand. Across the Narrow Sea in Pentos, Viserys Targaryen plans to wed his sister Daenerys to the nomadic Dothraki warrior leader, Khal Drogo to forge an alliance to take the throne.\"},{\"absoluteNumber\":11,\"airedEpisodeNumber\":1,\"airedSeason\":2,\"airedSeasonID\":473271,\"dvdEpisodeNumber\":1,\"dvdSeason\":2,\"episodeName\":\"The North Remembers\",\"firstAired\":\"2012-04-01\",\"id\":4161693,\"language\":{\"episodeName\":\"en\",\"overview\":\"en\"},\"lastUpdated\":1433646435,\"overview\":\"As Robb Stark and his northern army continue the war against the Lannisters, Tyrion arrives in King’s Landing to counsel Joffrey and temper the young king’s excesses.  On the island of Dragonstone, Stannis Baratheon plots an invasion to claim his late brother’s throne, allying himself with the fiery Melisandre, a strange priestess of a stranger god.  Across the sea, Daenerys, her three young dragons, and the khalasar trek through the Red Waste in search of allies, or water.  In the North, Bran presides over a threadbare Winterfell, while beyond the Wall, Jon Snow and the Night’s Watch must shelter with a devious wildling.\"}]}";
        private const string _imagesResponse = "{\"data\":[{\"id\":933623,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g26.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":5.8,\"count\":17},\"thumbnail\":\"_cache/graphical/121361-g26.jpg\"},{\"id\":700451,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g3.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":4.9,\"count\":18},\"thumbnail\":\"_cache/graphical/121361-g3.jpg\"},{\"id\":766641,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":4.8,\"count\":18},\"thumbnail\":\"_cache/graphical/121361-g.jpg\"},{\"id\":857862,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g5.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":4.2,\"count\":12},\"thumbnail\":\"_cache/graphical/121361-g5.jpg\"},{\"id\":862291,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g8.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":4.4,\"count\":11},\"thumbnail\":\"_cache/graphical/121361-g8.jpg\"},{\"id\":862297,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g9.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":6,\"count\":30},\"thumbnail\":\"_cache/graphical/121361-g9.jpg\"},{\"id\":863854,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g10.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":6.1,\"count\":29},\"thumbnail\":\"_cache/graphical/121361-g10.jpg\"},{\"id\":864336,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g12.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":6,\"count\":48},\"thumbnail\":\"_cache/graphical/121361-g12.jpg\"},{\"id\":864337,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g13.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":6.1,\"count\":48},\"thumbnail\":\"_cache/graphical/121361-g13.jpg\"},{\"id\":864338,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g14.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":5.3,\"count\":15},\"thumbnail\":\"_cache/graphical/121361-g14.jpg\"},{\"id\":879832,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g2.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":5.2,\"count\":13},\"thumbnail\":\"_cache/graphical/121361-g2.jpg\"},{\"id\":900055,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g19.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":6.4,\"count\":69},\"thumbnail\":\"_cache/graphical/121361-g19.jpg\"},{\"id\":921837,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g21.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":5.4,\"count\":15},\"thumbnail\":\"_cache/graphical/121361-g21.jpg\"},{\"id\":926585,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g22.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":6.2,\"count\":41},\"thumbnail\":\"_cache/graphical/121361-g22.jpg\"},{\"id\":931244,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g23.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":4.2,\"count\":5},\"thumbnail\":\"_cache/graphical/121361-g23.jpg\"},{\"id\":933621,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g24.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":5.9,\"count\":16},\"thumbnail\":\"_cache/graphical/121361-g24.jpg\"},{\"id\":996027,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g27.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":5.8,\"count\":12},\"thumbnail\":\"_cache/graphical/121361-g27.jpg\"},{\"id\":1010969,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g28.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":3.8,\"count\":6},\"thumbnail\":\"_cache/graphical/121361-g28.jpg\"},{\"id\":1011206,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g29.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":6,\"count\":9},\"thumbnail\":\"_cache/graphical/121361-g29.jpg\"},{\"id\":1024221,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g30.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":8,\"count\":1},\"thumbnail\":\"_cache/graphical/121361-g30.jpg\"},{\"id\":1061962,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g31.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":4.8,\"count\":10},\"thumbnail\":\"_cache/graphical/121361-g31.jpg\"},{\"id\":1063235,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g32.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":5.9,\"count\":21},\"thumbnail\":\"_cache/graphical/121361-g32.jpg\"},{\"id\":1063236,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g33.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":5.5,\"count\":12},\"thumbnail\":\"_cache/graphical/121361-g33.jpg\"},{\"id\":1063237,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g34.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":4,\"count\":5},\"thumbnail\":\"_cache/graphical/121361-g34.jpg\"},{\"id\":1066254,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g35.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":1.8,\"count\":4},\"thumbnail\":\"_cache/graphical/121361-g35.jpg\"},{\"id\":1068914,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g36.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":4.4,\"count\":5},\"thumbnail\":\"_cache/graphical/121361-g36.jpg\"},{\"id\":1069509,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g37.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":4.9,\"count\":7},\"thumbnail\":\"_cache/graphical/121361-g37.jpg\"},{\"id\":1118542,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g39.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":4,\"count\":1},\"thumbnail\":\"_cache/graphical/121361-g39.jpg\"},{\"id\":1129848,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g40.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":5.7,\"count\":3},\"thumbnail\":\"_cache/graphical/121361-g40.jpg\"},{\"id\":1177621,\"keyType\":\"series\",\"subKey\":\"graphical\",\"fileName\":\"graphical/121361-g42.jpg\",\"resolution\":\"\",\"ratingsInfo\":{\"average\":4,\"count\":1},\"thumbnail\":\"_cache/graphical/121361-g42.jpg\"}]}";

        protected override Task<IRestResponse> ExecuteRequestAsync(IRestRequest request)
        {
            IRestResponse response = null;
            //if a search then
            if (request.Resource.Contains("login"))
            {
                response = new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = _loginResponse
                };
            }
            else if (request.Resource.Contains("search"))
            {
                response = new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = _searchResponse
                };
            }
            else if (request.Resource.Contains("series") && !request.Resource.Contains("actors") && !request.Resource.Contains("episodes") && !request.Resource.Contains("images"))
            {
                response = new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = _seriesResponse
                };
            }
            else if (request.Resource.Contains("actors"))
            {
                response = new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = _actorsResponse
                };
            }
            else if (request.Resource.Contains("episodes"))
            {
                response = new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = _episodesResponse
                };
            }
            else if (request.Resource.Contains("images"))
            {
                response = new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = _imagesResponse
                };
            }

            return Task.FromResult(response);
        }
    }

    internal class ErrorCodeTestableTvdbManager : TvdbManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestableTvdbManager"/> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="helper"></param>
        public ErrorCodeTestableTvdbManager(IConfigurationManager configManager, IHelper helper) : base(configManager, helper)
        {
        }

        /// <summary>
        /// Executes the request asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// override for unit tests
        /// </remarks>
        protected override Task<IRestResponse> ExecuteRequestAsync(IRestRequest request)
        {
            IRestResponse response = new RestResponse
            {
                StatusCode = (HttpStatusCode)408,
            };
            return Task.FromResult(response);
        }
    }

    internal class WebExceptionTestableTvdbManager : TvdbManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestableTvdbManager"/> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="helper"></param>
        public WebExceptionTestableTvdbManager(IConfigurationManager configManager, IHelper helper) : base(configManager, helper)
        {
        }

        /// <summary>
        /// Executes the request asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// override for unit tests
        /// </remarks>
        protected override Task<IRestResponse> ExecuteRequestAsync(IRestRequest request)
        {
            throw new WebException();
        }
    }

    internal class ErrorExceptionTestableTvdbManager : TvdbManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestableTmdbManager"/> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="helper"></param>
        public ErrorExceptionTestableTvdbManager(IConfigurationManager configManager, IHelper helper) : base(configManager, helper)
        {
        }

        /// <summary>
        /// Executes the request asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// override for unit tests
        /// </remarks>
        protected override Task<IRestResponse> ExecuteRequestAsync(IRestRequest request)
        {
            IRestResponse response = new RestResponse
            {
                ErrorException = new ArgumentNullException("dummy")
            };
            return Task.FromResult(response);
        }
    }

    internal class UnauthorizedTestableTvdbManager : TvdbManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestableTmdbManager"/> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="helper"></param>
        public UnauthorizedTestableTvdbManager(IConfigurationManager configManager, IHelper helper) : base(configManager, helper)
        {
        }

        /// <summary>
        /// Executes the request asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// override for unit tests
        /// </remarks>
        protected override Task<IRestResponse> ExecuteRequestAsync(IRestRequest request)
        {
            IRestResponse response = null;
            if (request.Resource.Contains("search"))
            {
                response = new RestResponse
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }
            else
            {
                response = new RestResponse
                {
                    StatusCode = HttpStatusCode.OK
                };
            }
            return Task.FromResult(response);
        }
    }
}
