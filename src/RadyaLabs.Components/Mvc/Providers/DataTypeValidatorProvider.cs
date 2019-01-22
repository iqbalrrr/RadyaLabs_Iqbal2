using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace RadyaLabs.Components.Mvc
{
    public class DataTypeValidatorProvider : ClientDataTypeModelValidatorProvider
    {
        private HashSet<Type> NumericTypes { get; }

        public DataTypeValidatorProvider()
        {
            NumericTypes = new HashSet<Type>
            {
              typeof(Byte),
              typeof(SByte),
              typeof(Int16),
              typeof(UInt16),
              typeof(Int32),
              typeof(UInt32),
              typeof(Int64),
              typeof(UInt64),
              typeof(Single),
              typeof(Double),
              typeof(Decimal)
            };
        }

        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            Type type = Nullable.GetUnderlyingType(metadata.ModelType) ?? metadata.ModelType;

            if (IsDate(type, metadata))
                yield return new DateValidator(metadata, context);

            if (NumericTypes.Contains(type))
                yield return new NumberValidator(metadata, context);
        }

        private Boolean IsDate(Type type, ModelMetadata metadata)
        {
            return type == typeof(DateTime) && metadata.DataTypeName != "Time";
        }
    }
}
