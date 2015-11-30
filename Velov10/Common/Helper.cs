using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Velov10.DataModel;

namespace Velov10.Common
{
    public static class Helper
    {

        public static TappedEventHandler DisplayRibbon(double stationNumber, Button myButton)
        {
            return async (sender, eventArgs) =>
            {
                try
                {
                    var lyonStationResponse = await GetResponseFromApiByStationNumber(stationNumber);
                    myButton.Content = lyonStationResponse.AvailableBikes.ToString() + "/" +
                                       (lyonStationResponse.AvailableBikes + lyonStationResponse.AvailableBikeStands)
                                           .ToString();
                    await FillAndShowDialog(lyonStationResponse);
                }
                catch (UnauthorizedAccessException)
                {
                    //todo UnauthorizedAccessException
                }
            };
        }

        private static async Task FillAndShowDialog(LyonStation lyonStationResponse)
        {
            var sb = new StringBuilder();
            sb.AppendLine(lyonStationResponse.Address.ToLower());
            sb.AppendLine("Vélov' dispos : " + lyonStationResponse.AvailableBikes);
            sb.AppendLine("Places dispos : " + lyonStationResponse.AvailableBikeStands);
            var dialog = new MessageDialog(sb.ToString(), lyonStationResponse.Name);
            await dialog.ShowAsync();
        }

        private static async Task<LyonStation> GetResponseFromApiByStationNumber(double stationNumber)
        {
            var httpClient = new HttpClient();
            var msg = new HttpRequestMessage(new HttpMethod("GET"),
                new Uri("https://api.jcdecaux.com/vls/v1/stations/" + stationNumber +
                        "?contract=Lyon&apiKey=8554d38cbccde6f2ca5db7fdd689f1b588ce7786"))
            {
                Content = new HttpStringContent(string.Empty)
            };
            msg.Content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
            var response = await httpClient.SendRequestAsync(msg).AsTask();
            var deserializedResponse = await DeserializeResponseContent(response);
            return deserializedResponse;
        }

        private static async Task<LyonStation> DeserializeResponseContent(HttpResponseMessage response)
        {
            var s = await response.Content.ReadAsStringAsync();
            return FromJsonToLyonStation(s);
        }

        public static LyonStation FromJsonToLyonStation(string s)
        {
            var jsonObject = JsonObject.Parse(s);
            return LyonStationFromJsonObject(jsonObject);
        }

        private static LyonStation LyonStationFromJsonObject(JsonObject jsonObject)
        {
            var pos = jsonObject["position"].GetObject();
            return new LyonStation(
                jsonObject["number"].GetNumber(),
                jsonObject["name"].GetString(),
                jsonObject["address"].GetString(),
                pos["lat"].GetNumber(),
                pos["lng"].GetNumber(),
                jsonObject["bike_stands"].GetNumber(),
                jsonObject["available_bike_stands"].GetNumber(),
                jsonObject["available_bikes"].GetNumber());
        }

        private static LyonStation LyonStationFromJsonObjectLocal(JsonObject jsonObject)
        {
           
            return new LyonStation(
                jsonObject["number"].GetNumber(),
                jsonObject["name"].GetString(),
                jsonObject["address"].GetString(),
                jsonObject["latitude"].GetNumber(),
                jsonObject["longitude"].GetNumber());
        }

        public static async Task<IEnumerable<LyonStation>> LocalisationStations()
        {
            var jsonText = await OpenAndReadJson();
            var stations = JsonObject.Parse(jsonText).FirstOrDefault();
            var array = stations.Value.GetArray();

            return array.Select(station => LyonStationFromJsonObjectLocal(station.GetObject())).ToList();
        }

        private static async Task<string> OpenAndReadJson()
        {
            var dataUri = new Uri("ms-appx:///DataModel/LyonStations.json");
            var file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            var jsonText = await FileIO.ReadTextAsync(file);
            return jsonText;
        }
    }
}