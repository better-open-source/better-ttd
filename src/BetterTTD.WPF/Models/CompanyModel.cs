using BetterTTD.Domain.Entities;
using CSharpFunctionalExtensions;

namespace BetterTTD.WPF.Models
{
    public class CompanyModel : BaseModel
    {
        private CompanyModel(Company company)
        {
            Name = company.Name;
        }

        public static Result<CompanyModel> Create(Maybe<Company> maybeCompany) =>
            maybeCompany
                .Match(
                    company => new CompanyModel(company),
                    () => Result.Failure<CompanyModel>($"[{nameof(CompanyModel)}] Unknown company handled."));

        public string Name
        {
            get => Get("Unknown");
            set => Set(value);
        }
    }
}