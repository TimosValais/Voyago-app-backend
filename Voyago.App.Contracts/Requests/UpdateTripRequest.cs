using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;
public record UpdateTripRequest(TripStatus TripStatus,
                                decimal Budget,
                                DateTime From,
                                DateTime To);
