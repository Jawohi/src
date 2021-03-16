using System;
using FluentValidation;

namespace Konya_Hiermayer.Packages.BL.Entities.Validators
{
    public class WarehouseValidator : AbstractValidator<Warehouse>
    {
        public WarehouseValidator()
        {
            RuleFor(warehouse => warehouse.Code)
                .Matches("^[A-Z0-9]{1,9}$")
                .WithMessage("Code has to match given Pattern: ^[A-Z0-9]{1,9}$");
            RuleFor(warehouse => warehouse.Description)
                .Matches("^[A-Za-z0-9- ÖÄÜöäü]*$")
                .WithMessage("Invalid Warehouse Description");
        }
    }
}
