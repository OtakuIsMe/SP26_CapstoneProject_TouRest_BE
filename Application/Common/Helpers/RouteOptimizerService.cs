using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Common.Helpers
{
    public interface IRouteOptimizerService
    {
        Task OptimizeStopsAsync(Guid itineraryId);
    }

    public class RouteOptimizerService : IRouteOptimizerService
    {
        private readonly IItineraryStopRepository _stopRepository;

        public RouteOptimizerService(
            IItineraryStopRepository stopRepository)
        {
            _stopRepository = stopRepository;
        }

        public async Task OptimizeStopsAsync(Guid itineraryId)
        {
            var stops = await _stopRepository.GetByItineraryIdAsync(itineraryId);

            if (stops.Count < 2)
                return; // nothing to optimize

            var ordered = NearestNeighbor(stops);

            for (int i = 0; i < ordered.Count; i++)
                ordered[i].StopOrder = i + 1;

            await _stopRepository.UpdateRangeAsync(ordered);
        }

        private List<ItineraryStop> NearestNeighbor(List<ItineraryStop> stops)
        {
            var remaining = new List<ItineraryStop>(stops);
            var result = new List<ItineraryStop>();

            // start from whichever stop was added first (lowest StopOrder)
            var current = remaining.OrderBy(s => s.StopOrder).First();
            remaining.Remove(current);
            result.Add(current);

            while (remaining.Count > 0)
            {
                var next = remaining
                    .OrderBy(s => Haversine(current.Latitude, current.Longitude,
                                            s.Latitude, s.Longitude))
                    .First();

                result.Add(next);
                remaining.Remove(next);
                current = next;
            }

            return result;
        }

        // Haversine formula — straight-line distance between two lat/lng points in km
        private double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private double ToRad(double deg) => deg * Math.PI / 180;
    }
}
