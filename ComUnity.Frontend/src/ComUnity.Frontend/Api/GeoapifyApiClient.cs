using Newtonsoft.Json;
using System.Globalization;
using System.Web;

#pragma warning disable 8618 
#pragma warning disable 8603 

namespace ComUnity.Frontend.Api
{
    public class GeoapifyApiClient
    {
        private static string ApiKey = "d9afff5422214cd995e9ecfc8c8ee1a8";
        private readonly HttpClient _httpClient;

        public GeoapifyApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeocodingResult> ReverseGeocode(double lat, double lng)
        {
            var response = await _httpClient.GetAsync($"v1/geocode/reverse?lat={HttpUtility.UrlEncode(lat.ToString("G", CultureInfo.InvariantCulture))}&lon={HttpUtility.UrlEncode(lng.ToString("G", CultureInfo.InvariantCulture))}&apiKey={ApiKey}");
            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<GeocodingResult>(result);

                return data;
            }

            throw new Exception($"Error response while fetching data from Geoapify. {response.StatusCode}.");
        }
    }

    public class Datasource
    {
        [JsonProperty("sourcename", NullValueHandling = NullValueHandling.Ignore)]
        public string Sourcename { get; set; }

        [JsonProperty("attribution", NullValueHandling = NullValueHandling.Ignore)]
        public string Attribution { get; set; }

        [JsonProperty("license", NullValueHandling = NullValueHandling.Ignore)]
        public string License { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
    }

    public class Feature
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
        public Properties Properties { get; set; }

        [JsonProperty("geometry", NullValueHandling = NullValueHandling.Ignore)]
        public Geometry Geometry { get; set; }

        [JsonProperty("bbox", NullValueHandling = NullValueHandling.Ignore)]
        public List<double> Bbox { get; set; }
    }

    public class Geometry
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("coordinates", NullValueHandling = NullValueHandling.Ignore)]
        public List<double> Coordinates { get; set; }
    }

    public class Parsed
    {
        [JsonProperty("housenumber", NullValueHandling = NullValueHandling.Ignore)]
        public string Housenumber { get; set; }

        [JsonProperty("street", NullValueHandling = NullValueHandling.Ignore)]
        public string Street { get; set; }

        [JsonProperty("postcode", NullValueHandling = NullValueHandling.Ignore)]
        public string Postcode { get; set; }

        [JsonProperty("district", NullValueHandling = NullValueHandling.Ignore)]
        public string District { get; set; }

        [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; }

        [JsonProperty("expected_type", NullValueHandling = NullValueHandling.Ignore)]
        public string ExpectedType { get; set; }
    }

    public class Properties
    {
        [JsonProperty("datasource", NullValueHandling = NullValueHandling.Ignore)]
        public Datasource Datasource { get; set; }

        [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; }

        [JsonProperty("country_code", NullValueHandling = NullValueHandling.Ignore)]
        public string CountryCode { get; set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }

        [JsonProperty("county", NullValueHandling = NullValueHandling.Ignore)]
        public string County { get; set; }

        [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
        public string City { get; set; }

        [JsonProperty("postcode", NullValueHandling = NullValueHandling.Ignore)]
        public string Postcode { get; set; }

        [JsonProperty("suburb", NullValueHandling = NullValueHandling.Ignore)]
        public string Suburb { get; set; }

        [JsonProperty("street", NullValueHandling = NullValueHandling.Ignore)]
        public string Street { get; set; }

        [JsonProperty("housenumber", NullValueHandling = NullValueHandling.Ignore)]
        public string Housenumber { get; set; }

        [JsonProperty("lon", NullValueHandling = NullValueHandling.Ignore)]
        public double Lon { get; set; }

        [JsonProperty("lat", NullValueHandling = NullValueHandling.Ignore)]
        public double Lat { get; set; }

        [JsonProperty("state_code", NullValueHandling = NullValueHandling.Ignore)]
        public string StateCode { get; set; }

        [JsonProperty("formatted", NullValueHandling = NullValueHandling.Ignore)]
        public string Formatted { get; set; }

        [JsonProperty("address_line1", NullValueHandling = NullValueHandling.Ignore)]
        public string AddressLine1 { get; set; }

        [JsonProperty("address_line2", NullValueHandling = NullValueHandling.Ignore)]
        public string AddressLine2 { get; set; }

        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty("timezone", NullValueHandling = NullValueHandling.Ignore)]
        public Timezone Timezone { get; set; }

        [JsonProperty("result_type", NullValueHandling = NullValueHandling.Ignore)]
        public string ResultType { get; set; }

        [JsonProperty("rank", NullValueHandling = NullValueHandling.Ignore)]
        public Rank Rank { get; set; }

        [JsonProperty("place_id", NullValueHandling = NullValueHandling.Ignore)]
        public string PlaceId { get; set; }
    }

    public class Query
    {
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("parsed", NullValueHandling = NullValueHandling.Ignore)]
        public Parsed Parsed { get; set; }
    }

    public class Rank
    {
        [JsonProperty("importance", NullValueHandling = NullValueHandling.Ignore)]
        public double Importance { get; set; }

        [JsonProperty("popularity", NullValueHandling = NullValueHandling.Ignore)]
        public double Popularity { get; set; }

        [JsonProperty("confidence", NullValueHandling = NullValueHandling.Ignore)]
        public double Confidence { get; set; }

        [JsonProperty("confidence_city_level", NullValueHandling = NullValueHandling.Ignore)]
        public double ConfidenceCityLevel { get; set; }

        [JsonProperty("confidence_street_level", NullValueHandling = NullValueHandling.Ignore)]
        public double ConfidenceStreetLevel { get; set; }

        [JsonProperty("match_type", NullValueHandling = NullValueHandling.Ignore)]
        public string MatchType { get; set; }
    }

    public class GeocodingResult
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("features", NullValueHandling = NullValueHandling.Ignore)]
        public List<Feature> Features { get; set; }

        [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
        public Query Query { get; set; }
    }

    public class Timezone
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("offset_STD", NullValueHandling = NullValueHandling.Ignore)]
        public string OffsetSTD { get; set; }

        [JsonProperty("offset_STD_seconds", NullValueHandling = NullValueHandling.Ignore)]
        public int OffsetSTDSeconds { get; set; }

        [JsonProperty("offset_DST", NullValueHandling = NullValueHandling.Ignore)]
        public string OffsetDST { get; set; }

        [JsonProperty("offset_DST_seconds", NullValueHandling = NullValueHandling.Ignore)]
        public int OffsetDSTSeconds { get; set; }

        [JsonProperty("abbreviation_STD", NullValueHandling = NullValueHandling.Ignore)]
        public string AbbreviationSTD { get; set; }

        [JsonProperty("abbreviation_DST", NullValueHandling = NullValueHandling.Ignore)]
        public string AbbreviationDST { get; set; }
    }


}
