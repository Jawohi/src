using System;
using FluentValidation;

namespace Konya_Hiermayer.Packages.BL.Entities.Validators
{
    public class ParcelValidator : AbstractValidator<Parcel>
    {
        public ParcelValidator() 
        {
            RuleFor(parcel => parcel.TrackingId)
                .Matches("^[A-Z0-9]{9}$")
                .WithMessage("Tracking ID has to match given Pattern: ^[A-Z0-9]{9}$");
            RuleFor(parcel => parcel.Weight)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Weight has to be greater than 0");
        }
    }
}
