// MockAPIs.BLL/Helpers/FakerEngine.cs
using Bogus;
using MockAPIs.DAL.Enums;
using MockAPIs.DAL.Models;

namespace MockAPIs.BLL.Helpers
{
    public static class FakerEngine
    {
        private static readonly Faker _faker = new Faker("en");

        // generates N records based on the resource fields
        // returns a list of dictionaries — each dict is one record
        public static List<Dictionary<string, object?>> Generate(List<Field> fields, int count)
        {
            var records = new List<Dictionary<string, object?>>();

            for (int i = 0; i < count; i++)
            {
                var record = new Dictionary<string, object?>();

                // always add an id field first
                record["id"] = Guid.NewGuid().ToString();

                foreach (var field in fields)
                {
                    record[field.Name] = GenerateValue(field);
                }

                records.Add(record);
            }

            return records;
        }

        // generates a single value based on DataType and FakerHint
        private static object? GenerateValue(Field field)
        {
            // if user provided a FakerHint try to use it first
            if (!string.IsNullOrWhiteSpace(field.FakerHint))
            {
                var hintValue = ResolveHint(field.FakerHint);
                if (hintValue != null)
                    return hintValue;
            }

            // fallback to DataType if no hint or hint not recognized
            return field.DataType switch
            {
                FieldDataType.String    => _faker.Lorem.Word(),
                FieldDataType.Number    => _faker.Random.Int(1, 1000),
                FieldDataType.Boolean   => _faker.Random.Bool(),
                FieldDataType.Date      => _faker.Date.Past().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                FieldDataType.Image     => _faker.Image.LoremFlickrUrl(),
                FieldDataType.UUID      => Guid.NewGuid().ToString(),
                FieldDataType.Email     => _faker.Internet.Email(),
                FieldDataType.Name      => _faker.Name.FullName(),
                FieldDataType.Price     => Math.Round(_faker.Random.Decimal(1, 1000), 2),
                FieldDataType.Paragraph => _faker.Lorem.Paragraph(),
                _                       => _faker.Lorem.Word()
            };
        }

        // maps FakerHint dot-notation to actual Bogus calls
        // e.g. "commerce.productName" → faker.Commerce.ProductName()
        private static object? ResolveHint(string hint)
        {
            return hint.ToLower() switch
            {
                // commerce
                "commerce.productname"      => _faker.Commerce.ProductName(),
                "commerce.department"       => _faker.Commerce.Department(),
                "commerce.productadjective" => _faker.Commerce.ProductAdjective(),
                "commerce.productmaterial"  => _faker.Commerce.ProductMaterial(),
                "commerce.categories"       => _faker.Commerce.Categories(1)[0],
                "commerce.price"            => Math.Round(_faker.Random.Decimal(10, 1000), 2),
                "commerce.color"            => _faker.Commerce.Color(),
                "commerce.isbn"             => _faker.Commerce.Ean13(),

                // finance
                "finance.amount"            => Math.Round(_faker.Finance.Amount(1, 1000), 2),
                "finance.currency"          => _faker.Finance.Currency().Code,
                "finance.accountname"       => _faker.Finance.AccountName(),
                "finance.accountnumber"     => _faker.Finance.Account(),
                "finance.bic"               => _faker.Finance.Bic(),
                "finance.creditcard"        => _faker.Finance.CreditCardNumber(),
                "finance.iban"              => _faker.Finance.Iban(),

                // internet
                "internet.email"            => _faker.Internet.Email(),
                "internet.username"         => _faker.Internet.UserName(),
                "internet.url"              => _faker.Internet.Url(),
                "internet.ip"               => _faker.Internet.Ip(),
                "internet.ipv6"             => _faker.Internet.Ipv6(),
                "internet.mac"              => _faker.Internet.Mac(),
                "internet.useragent"        => _faker.Internet.UserAgent(),
                "internet.password"         => _faker.Internet.Password(),
                "internet.domain"           => _faker.Internet.DomainName(),
                "internet.avatar"           => _faker.Internet.Avatar(),

                // image
                "image.url"                 => _faker.Image.LoremFlickrUrl(),

                // name
                "name.fullname"             => _faker.Name.FullName(),
                "name.firstname"            => _faker.Name.FirstName(),
                "name.lastname"             => _faker.Name.LastName(),
                "name.prefix"               => _faker.Name.Prefix(),
                "name.suffix"               => _faker.Name.Suffix(),
                "name.jobtitle"             => _faker.Name.JobTitle(),
                "name.jobdescriptor"       => _faker.Name.JobDescriptor(),

                // address
                "address.city"              => _faker.Address.City(),
                "address.country"           => _faker.Address.Country(),
                "address.countrycode"       => _faker.Address.CountryCode(),
                "address.streetaddress"     => _faker.Address.StreetAddress(),
                "address.zipcode"           => _faker.Address.ZipCode(),
                "address.state"             => _faker.Address.State(),
                "address.latitude"          => _faker.Address.Latitude(),
                "address.longitude"         => _faker.Address.Longitude(),
                "address.buildingnumber"    => _faker.Address.BuildingNumber(),

                // phone
                "phone.phonenumber"         => _faker.Phone.PhoneNumber(),

                // company
                "company.companyname"       => _faker.Company.CompanyName(),
                "company.catchphrase"       => _faker.Company.CatchPhrase(),
                "company.bs"                => _faker.Company.Bs(),
                "company.suffix"            => _faker.Company.CompanySuffix(),

                // lorem
                "lorem.word"                => _faker.Lorem.Word(),
                "lorem.sentence"            => _faker.Lorem.Sentence(),
                "lorem.paragraph"           => _faker.Lorem.Paragraph(),
                "lorem.text"                => _faker.Lorem.Text(),
                "lorem.slug"                => _faker.Lorem.Slug(),

                // date
                "date.past"                 => _faker.Date.Past().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                "date.future"               => _faker.Date.Future().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                "date.recent"               => _faker.Date.Recent().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                "date.month"                => _faker.Date.Month(),
                "date.weekday"              => _faker.Date.Weekday(),

                // system & database
                "system.filename"           => _faker.System.FileName(),
                "system.mime"               => _faker.System.MimeType(),
                "system.filetype"           => _faker.System.FileType(),
                "system.semver"             => _faker.System.Semver(),
                "database.column"           => _faker.Database.Column(),
                "database.type"             => _faker.Database.Type(),
                "database.engine"           => _faker.Database.Engine(),

                // hacker
                "hacker.phrase"             => _faker.Hacker.Phrase(),
                "hacker.noun"               => _faker.Hacker.Noun(),
                "hacker.verb"               => _faker.Hacker.Verb(),
                "hacker.ingverb"            => _faker.Hacker.IngVerb(),
                "hacker.abbreviation"       => _faker.Hacker.Abbreviation(),

                // vehicle
                "vehicle.vin"               => _faker.Vehicle.Vin(),
                "vehicle.manufacturer"      => _faker.Vehicle.Manufacturer(),
                "vehicle.model"             => _faker.Vehicle.Model(),
                "vehicle.type"              => _faker.Vehicle.Type(),
                "vehicle.fuel"              => _faker.Vehicle.Fuel(),

                // random
                "random.number"             => _faker.Random.Int(1, 1000),
                "random.bool"               => _faker.Random.Bool(),
                "random.uuid"               => Guid.NewGuid().ToString(),

                // unrecognized hint — return null so caller falls back to DataType
                _                           => null
            };
        }
    }
}