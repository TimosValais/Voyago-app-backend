namespace Voyago.App.Contracts.Requests;
public record CreateTripRequest(decimal Budget,
                                DateTime From,
                                DateTime To);
