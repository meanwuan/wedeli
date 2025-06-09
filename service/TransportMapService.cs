using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WeDeLi1.Dbase;

namespace WeDeLi1.Service
{
    public class TransportMapService
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, (double? lat, double? lng)> _geocodeCache;

        public TransportMapService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "WeDeLiApp/1.0 (your-email@domain.com)");
            _geocodeCache = new Dictionary<string, (double? lat, double? lng)>();
        }

        private async Task<(double? lat, double? lng)> GeocodeAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return (null, null);
            }

            if (_geocodeCache.TryGetValue(address, out var cachedCoords))
            {
                return cachedCoords;
            }

            int maxRetries = 3;
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    string fullAddress = $"{address.Trim()}, Việt Nam";
                    string url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(fullAddress)}&addressdetails=1&limit=1";
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var json = JArray.Parse(await response.Content.ReadAsStringAsync());
                    if (json.Count > 0)
                    {
                        double lat = json[0]["lat"].ToObject<double>();
                        double lng = json[0]["lon"].ToObject<double>();
                        _geocodeCache[address] = (lat, lng);
                        return (lat, lng);
                    }
                    else
                    {
                        _geocodeCache[address] = (null, null);
                        return (null, null);
                    }
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("403"))
                {
                    if (retry == maxRetries - 1)
                    {
                        return (null, null);
                    }
                    await Task.Delay(2000);
                }
                catch (Exception)
                {
                    return (null, null);
                }
            }
            return (null, null);
        }

        public async Task<object> GetUserLocation(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new { Success = false, Message = "UserId is required" };
            }

            try
            {
                using (var context = new databases())
                {
                    string userAddress = context.NguoiDungs
                        .Where(u => u.MaNguoiDung == userId)
                        .Select(u => u.DiaChi)
                        .FirstOrDefault();

                    if (string.IsNullOrEmpty(userAddress))
                    {
                        return new { Success = false, Message = "User address not found" };
                    }

                    var coordinates = await GeocodeAddress(userAddress);
                    if (!coordinates.lat.HasValue || !coordinates.lng.HasValue)
                    {
                        return new { Success = false, Message = $"Unable to convert address to coordinates: {userAddress}" };
                    }

                    return new
                    {
                        Success = true,
                        Data = new
                        {
                            Address = userAddress,
                            Latitude = coordinates.lat,
                            Longitude = coordinates.lng
                        },
                        Message = "User location retrieved successfully"
                    };
                }
            }
            catch (Exception ex)
            {
                return new { Success = false, Message = $"Error retrieving user location: {ex.Message}" };
            }
        }

        public async Task<object> GetCoordinatesFromUserAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return new { Success = false, Message = "Address is required" };
            }

            try
            {
                var coordinates = await GeocodeAddress(address);
                if (!coordinates.lat.HasValue || !coordinates.lng.HasValue)
                {
                    return new { Success = false, Message = $"Unable to convert address to coordinates: {address}" };
                }

                return new
                {
                    Success = true,
                    Data = new
                    {
                        Address = address,
                        Latitude = coordinates.lat,
                        Longitude = coordinates.lng
                    },
                    Message = "Address converted to coordinates successfully"
                };
            }
            catch (Exception ex)
            {
                return new { Success = false, Message = $"Error converting address: {ex.Message}" };
            }
        }

        public async Task<object> GetNearbyBusStations(string userId, double radiusKm)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new { Success = false, Message = "UserId is required" };
            }

            try
            {
                using (var context = new databases())
                {
                    string userAddress = context.NguoiDungs
                        .Where(u => u.MaNguoiDung == userId)
                        .Select(u => u.DiaChi)
                        .FirstOrDefault();

                    if (string.IsNullOrEmpty(userAddress))
                    {
                        return new { Success = false, Message = "User address not found" };
                    }

                    var userCoords = await GeocodeAddress(userAddress);
                    if (!userCoords.lat.HasValue || !userCoords.lng.HasValue)
                    {
                        return new { Success = false, Message = $"Unable to convert user address to coordinates: {userAddress}" };
                    }

                    var busStations = context.NhaXes
                        .Select(nx => new { nx.MaNhaXe, nx.TenChu, nx.DiaChi })
                        .ToList();

                    var busStationsWithCoords = new List<(string MaNhaXe, string TenChu, string DiaChi, double? Lat, double? Lng)>();
                    foreach (var station in busStations)
                    {
                        try
                        {
                            var coords = await GeocodeAddress(station.DiaChi);
                            busStationsWithCoords.Add((station.MaNhaXe, station.TenChu, station.DiaChi, coords.lat, coords.lng));
                            await Task.Delay(1000);
                        }
                        catch
                        {
                            busStationsWithCoords.Add((station.MaNhaXe, station.TenChu, station.DiaChi, null, null));
                        }
                    }

                    var nearbyStations = await FilterBusStationsWithinRadius(userCoords.lat.Value, userCoords.lng.Value, busStationsWithCoords, radiusKm);

                    var result = new List<object>();
                    foreach (var station in nearbyStations)
                    {
                        try
                        {
                            string distance = station.Lat.HasValue && station.Lng.HasValue ? await GetDistance(userAddress, station.DiaChi) : "N/A";
                            result.Add(new
                            {
                                MaNhaXe = station.MaNhaXe,
                                TenChu = station.TenChu,
                                DiaChi = station.DiaChi,
                                Latitude = station.Lat,
                                Longitude = station.Lng,
                                Distance = distance
                            });
                        }
                        catch
                        {
                            result.Add(new
                            {
                                MaNhaXe = station.MaNhaXe,
                                TenChu = station.TenChu,
                                DiaChi = station.DiaChi,
                                Latitude = station.Lat,
                                Longitude = station.Lng,
                                Distance = "N/A"
                            });
                        }
                    }

                    return new
                    {
                        Success = true,
                        Data = new
                        {
                            UserAddress = userAddress,
                            UserLatitude = userCoords.lat,
                            UserLongitude = userCoords.lng,
                            NearbyBusStations = result
                        },
                        Message = "Nearby bus stations retrieved successfully"
                    };
                }
            }
            catch (Exception ex)
            {
                return new { Success = false, Message = $"Error retrieving nearby bus stations: {ex.Message}" };
            }
        }

        private Task<List<(string MaNhaXe, string TenChu, string DiaChi, double? Lat, double? Lng)>> FilterBusStationsWithinRadius(double userLat, double userLng, List<(string MaNhaXe, string TenChu, string DiaChi, double? Lat, double? Lng)> busStations, double radiusKm)
        {
            var nearbyStations = new List<(string MaNhaXe, string TenChu, string DiaChi, double? Lat, double? Lng)>();

            foreach (var station in busStations)
            {
                if (station.Lat.HasValue && station.Lng.HasValue)
                {
                    double distance = CalculateDistance(userLat, userLng, station.Lat.Value, station.Lng.Value);
                    if (distance <= radiusKm)
                    {
                        nearbyStations.Add(station);
                    }
                }
            }

            return Task.FromResult(nearbyStations);
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth's radius (km)
            var dLat = ToRadian(lat2 - lat1);
            var dLon = ToRadian(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadian(double degree)
        {
            return degree * Math.PI / 180;
        }

        private async Task<string> GetDistance(string origin, string destination)
        {
            try
            {
                var originCoords = await GeocodeAddress(origin);
                var destCoords = await GeocodeAddress(destination);

                if (!originCoords.lat.HasValue || !originCoords.lng.HasValue || !destCoords.lat.HasValue || !destCoords.lng.HasValue)
                {
                    return "N/A";
                }

                string url = $"http://router.project-osrm.org/route/v1/driving/{originCoords.lng},{originCoords.lat};{destCoords.lng},{destCoords.lat}?overview=false";
                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                if (json["code"].ToString() == "Ok")
                {
                    double distanceMeters = json["routes"][0]["distance"].ToObject<double>();
                    return $"{distanceMeters / 1000:F2} km";
                }
                else
                {
                    return "N/A";
                }
            }
            catch
            {
                return "N/A";
            }
        }
    }
}