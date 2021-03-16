using System;
using FluentValidation;

namespace Konya_Hiermayer.Packages.BL.Entities.Validators
{
    public class RecipientValidator : AbstractValidator<Recipient>
    {
        public RecipientValidator()
        {
            RuleFor(recipient => recipient.City)
                .Matches("^[A-Z]{1}[a-z]*-?[A-Z]?[a-z]*$")
                .WithMessage("Invalid City");
            RuleFor(recipient => recipient.Name)
                .Matches("^[A-Z]{1}[a-z]*-?[A-Z]?[a-z]*$")
                .WithMessage("Invalid Name");
            RuleFor(recipient => recipient.Street)
                .Matches("^[A-Z]{1}[a-zß]*\\s[0-9a-z]+(\\/[0-9]+)*$")
                .When(recipient => recipient.Country == "Austria" || recipient.Country == "Österreich")
                .WithMessage("Invalid Street");
            RuleFor(recipient => recipient.PostalCode)
                .Matches("A-[0-9]{4}$")
                .When(recipient => recipient.Country == "Austria" || recipient.Country == "Österreich")
                .WithMessage("Invalid Postal Code");
        }
    }
}
