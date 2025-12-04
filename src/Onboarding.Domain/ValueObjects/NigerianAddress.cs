// Domain/ValueObjects/NigerianAddress.cs
using Onboarding.Domain.Enums;

namespace Onboarding.Domain.ValueObjects;

public record NigerianAddress(
    string Street,
    string City,
    string LGA,
    NigerianState State,
    string PostalCode);