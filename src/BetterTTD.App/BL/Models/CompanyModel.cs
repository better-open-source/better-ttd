using BetterTTD.Domain.Entities;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace BetterTTD.App.BL.Models
{
    public class CompanyModel : ReactiveObject
    {
        private string _name = "Unknown";

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
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
    }
}