using System.Runtime.Serialization;

namespace Velov10.DataModel
{
    [DataContract]
    public class LyonStation
    {
        public LyonStation(double number, string name, string address, double latitude, double longitude, double bikeStands = 0,
            double availablBikeStands = 0, double availableBikes = 0)
        {
            Number = number;
            Name = name;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            BikeStands = bikeStands;
            AvailableBikeStands = availablBikeStands;
            AvailableBikes = availableBikes;
        }

        [DataMember]
        public double Number { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public string Address { get; private set; }

        [DataMember]
        public double Latitude { get; private set; }

        [DataMember]
        public double Longitude { get; private set; }

        [DataMember]
        public double BikeStands { get; private set; }

        public double AvailableBikeStands { get; private set; }

        [DataMember]
        public double AvailableBikes { get; private set; }
    }
}