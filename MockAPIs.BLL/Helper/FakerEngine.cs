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

                // finance
                "finance.amount"            => Math.Round(_faker.Finance.Amount(1, 1000), 2),
                "finance.currency"          => _faker.Finance.Currency().Code,
                "finance.accountname"       => _faker.Finance.AccountName(),

                // internet
                "internet.email"            => _faker.Internet.Email(),
                "internet.username"         => _faker.Internet.UserName(),
                "internet.url"              => _faker.Internet.Url(),
                "internet.ip"               => _faker.Internet.Ip(),
                "internet.useragent"        => _faker.Internet.UserAgent(),

                // image
                "image.url"                 => _faker.Image.LoremFlickrUrl(),

                // name
                "name.fullname"             => _faker.Name.FullName(),
                "name.firstname"            => _faker.Name.FirstName(),
                "name.lastname"             => _faker.Name.LastName(),
                "name.prefix"               => _faker.Name.Prefix(),

                // address
                "address.city"              => _faker.Address.City(),
                "address.country"           => _faker.Address.Country(),
                "address.streetaddress"     => _faker.Address.StreetAddress(),
                "address.zipcode"           => _faker.Address.ZipCode(),
                "address.state"             => _faker.Address.State(),

                // phone
                "phone.phonenumber"         => _faker.Phone.PhoneNumber(),

                // company
                "company.companyname"       => _faker.Company.CompanyName(),
                "company.catchphrase"       => _faker.Company.CatchPhrase(),
                "company.bs"                => _faker.Company.Bs(),

                // lorem
                "lorem.word"                => _faker.Lorem.Word(),
                "lorem.sentence"            => _faker.Lorem.Sentence(),
                "lorem.paragraph"           => _faker.Lorem.Paragraph(),

                // date
                "date.past"                 => _faker.Date.Past().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                "date.future"               => _faker.Date.Future().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                "date.recent"               => _faker.Date.Recent().ToString("yyyy-MM-ddTHH:mm:ssZ"),

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