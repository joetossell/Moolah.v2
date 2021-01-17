//https://github.com/Macross-Software/core/blob/develop/ClassLibraries/Macross.Json.Extensions/Code/System.Text.Json.Serialization/JsonTimeSpanConverter.cs

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Moolah.Calculations.Serialization
{
	/// <summary>
	/// <see cref="JsonConverterFactory"/> to convert <see cref="Annuity"/> to and from strings. Supports <see cref="Nullable{Annuity}"/>.
	/// </summary>
	public class JsonAnnuityConverter : JsonConverterFactory
	{
		/// <inheritdoc/>
		public override bool CanConvert(Type typeToConvert)
		{
			// Don't perform a typeToConvert == null check for performance. Trust our callers will be nice.
#pragma warning disable CA1062 // Validate arguments of public methods
			return typeToConvert == typeof(Annuity)
				|| (typeToConvert.IsGenericType && IsNullableAnnuity(typeToConvert));
#pragma warning restore CA1062 // Validate arguments of public methods
		}

		/// <inheritdoc/>
		public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
		{
			// Don't perform a typeToConvert == null check for performance. Trust our callers will be nice.
#pragma warning disable CA1062 // Validate arguments of public methods
			return typeToConvert.IsGenericType
				? (JsonConverter)new JsonNullableAnnuityConverter()
				: new JsonStandardAnnuityConverter();
#pragma warning restore CA1062 // Validate arguments of public methods
		}

		private static bool IsNullableAnnuity(Type typeToConvert)
		{
			Type? UnderlyingType = Nullable.GetUnderlyingType(typeToConvert);

			return UnderlyingType != null && UnderlyingType == typeof(Annuity);
		}

		internal class JsonStandardAnnuityConverter : JsonConverter<Annuity>
		{
			/// <inheritdoc/>
			public override Annuity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType != JsonTokenType.String)
					throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(typeof(Annuity));

				string value = reader.GetString()!;
				try
				{
					return Annuity.Parse(value);
				}
				catch (Exception parseException)
				{
					throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(typeof(Annuity), value, parseException);
				}
			}

			/// <inheritdoc/>
			public override void Write(Utf8JsonWriter writer, Annuity value, JsonSerializerOptions options)
				=> writer.WriteStringValue(value.ToString());
		}

		internal class JsonNullableAnnuityConverter : JsonConverter<Annuity?>
		{
			/// <inheritdoc/>
			public override Annuity? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType != JsonTokenType.String)
					throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(typeof(Annuity?));

				string value = reader.GetString()!;
				try
				{
					return Annuity.Parse(value);
				}
				catch (Exception parseException)
				{
					throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(typeof(Annuity?), value, parseException);
				}
			}

			/// <inheritdoc/>
			public override void Write(Utf8JsonWriter writer, Annuity? value, JsonSerializerOptions options)
				=> writer.WriteStringValue(value!.Value.ToString());
		}
	}
}