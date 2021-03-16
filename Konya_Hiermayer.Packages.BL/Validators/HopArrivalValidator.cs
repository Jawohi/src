using System;
using FluentValidation;
using Konya_Hiermayer.Packages.BL.Entities;

namespace Konya_Hiermayer.Packages.BL.Validators
{
    public class HopArrivalValidator : AbstractValidator<HopArrival>
    {
        public HopArrivalValidator()
        {
            RuleFor(hoparrival => hoparrival.Hop.Code)
                .Matches("^[A-Z]{4}\\d{1,4}$")
                .WithMessage("Code has to match given Pattern: ^[A-Z]{4}\\d{1,4}$");
        }
    }
}
