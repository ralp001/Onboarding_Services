// Application/Features/Students/DTOs/NigerianAddressDto.cs
using Onboarding.Domain.Enums;

namespace Onboarding.Application.DTOs;

public record NigerianAddressDto(
    string Street,
    string City,
    string LGA,
    NigerianState State,
    string PostalCode);