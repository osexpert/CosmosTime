namespace CosmosTime
{
    internal static class Constants
    {
        internal const string FixedLengthIsoFormatWithoutZ = "yyyy'-'MM'-'ddTHH':'mm':'ss'.'fffffff";
        // this is almost the same as "o" format (roundtrip), except roundtrip uses K (kind) instead of Z (zulu)
        internal const string FixedLengthIsoFormatWithZ = FixedLengthIsoFormatWithoutZ + "Z";

        internal const string VariableLengthMicrosIsoFormatWithoutZ = "yyyy'-'MM'-'ddTHH':'mm':'ss'.'FFFFFFF";
        internal const string VariableLengthMicrosIsoFormatWithZ = VariableLengthMicrosIsoFormatWithoutZ + "Z";
        //internal const string VariableLengthMicrosIsoFormatWithTZ = VariableLengthMicrosIsoFormatWithoutZ + "K";
    }
}
