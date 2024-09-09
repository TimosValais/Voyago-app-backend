namespace Voyago.App.Contracts.Requests;
public record CreateTripRequest(string Name,
                                decimal Budget,
                                DateTime From,
                                DateTime To);
