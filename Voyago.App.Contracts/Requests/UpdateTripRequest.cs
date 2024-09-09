using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;
public record UpdateTripRequest(TripStatus TripStatus,
                                string Name,
                                decimal Budget,
                                DateTime From,
                                DateTime To);
