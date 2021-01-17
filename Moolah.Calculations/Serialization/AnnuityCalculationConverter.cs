using Moolah.Calculations.IncomeTax;
using Moolah.Calculations.NationalInsurance;
using Moolah.Calculations.StudentLoan;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Moolah.Calculations.Serialization
{
    public class AnnuityCalculationConverter : JsonConverter<IEnumerable<IAnnuityCalculation>>
    {
        private static readonly IDictionary<Type, int> discriminator = new Dictionary<Type, int>
        {
            { typeof(IncomeTaxCalculation), 1 },
            { typeof(Class1NationalInsuranceCalculation), 2 },
            { typeof(StudentLoanPlan1Calculation), 3 },
            { typeof(CombinedCalculation), 4 },
            { typeof(PercentageCalculation), 5 },
            { typeof(SumCalculation), 6 },
        };

        delegate IAnnuityCalculation GetDeserializer(ref Utf8JsonReader reader, JsonSerializerOptions options);

        private static readonly IDictionary<int, GetDeserializer> deserializer = new Dictionary<int, GetDeserializer>
        {
            { 1, (ref Utf8JsonReader r, JsonSerializerOptions o) => JsonSerializer.Deserialize<IncomeTaxCalculation>(ref r, o) },
            { 2, (ref Utf8JsonReader r, JsonSerializerOptions o) => JsonSerializer.Deserialize<Class1NationalInsuranceCalculation>(ref r, o) },
            { 3, (ref Utf8JsonReader r, JsonSerializerOptions o) => JsonSerializer.Deserialize<StudentLoanPlan1Calculation>(ref r, o) },
            { 4, (ref Utf8JsonReader r, JsonSerializerOptions o) => JsonSerializer.Deserialize<CombinedCalculation>(ref r, o) },
            { 5, (ref Utf8JsonReader r, JsonSerializerOptions o) => JsonSerializer.Deserialize<PercentageCalculation>(ref r, o) },
            { 6, (ref Utf8JsonReader r, JsonSerializerOptions o) => JsonSerializer.Deserialize<SumCalculation>(ref r, o) }
        };

        public override bool CanConvert(Type typeToConvert) =>
            typeof(IEnumerable<IAnnuityCalculation>).IsAssignableFrom(typeToConvert);

        public override IEnumerable<IAnnuityCalculation> Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            var results = new List<IAnnuityCalculation>();

            while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
            {
                var item = ReadItem(ref reader, options);
                results.Add(item);
            }

            if (reader.TokenType != JsonTokenType.EndArray)
            {
                throw new JsonException();
            }

            return results;
        }

        private IAnnuityCalculation ReadItem(
            ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            if (!reader.Read()
                    || reader.TokenType != JsonTokenType.PropertyName
                    || reader.GetString() != "TypeDiscriminator")
            {
                throw new JsonException();
            }

            if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            var typeDiscriminator = reader.GetInt32();

            IAnnuityCalculation calculation;
            if (deserializer.ContainsKey(typeDiscriminator))
            {
                if (!reader.Read() || reader.GetString() != "TypeValue")
                {
                    throw new JsonException();
                }
                if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }
                calculation = deserializer[typeDiscriminator](ref reader, options);
            }
            else
            {
                throw new NotSupportedException();
            }

            if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return calculation;
        }

        public override void Write(
        Utf8JsonWriter writer, IEnumerable<IAnnuityCalculation> calculations, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach(var calculation in calculations)
            {
                WriteItem(writer, calculation, options);
            }

            writer.WriteEndArray();
        }

        public void WriteItem(
                Utf8JsonWriter writer, IAnnuityCalculation calculation, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var type = calculation.GetType();
            if (discriminator.ContainsKey(type))
            {
                writer.WriteNumber("TypeDiscriminator", discriminator[type]);
                writer.WritePropertyName("TypeValue");
                JsonSerializer.Serialize(writer, calculation, type, options);
            }
            else
            {
                throw new NotSupportedException();
            }

            writer.WriteEndObject();
        }
    }
}