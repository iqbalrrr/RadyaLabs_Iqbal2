using RadyaLabs.Components.Mvc;
using RadyaLabs.Objects;
using System;
using System.Collections.Generic;
using System.Web;

namespace RadyaLabs.Tests.Objects
{
    public class AllTypesView : BaseView
    {
        public TestEnum EnumField { get; set; }
        public SByte SByteField { get; set; }
        public Byte ByteField { get; set; }
        public Int16 Int16Field { get; set; }
        public UInt16 UInt16Field { get; set; }
        public Int32 Int32Field { get; set; }
        public UInt32 UInt32Field { get; set; }
        public Int64 Int64Field { get; set; }
        public UInt64 UInt64Field { get; set; }
        public Single SingleField { get; set; }
        public Double DoubleField { get; set; }
        public Decimal DecimalField { get; set; }
        public Boolean BooleanField { get; set; }
        public DateTime DateTimeField { get; set; }

        public TestEnum? NullableEnumField { get; set; }
        public SByte? NullableSByteField { get; set; }
        public Byte? NullableByteField { get; set; }
        public Int16? NullableInt16Field { get; set; }
        public UInt16? NullableUInt16Field { get; set; }
        public Int32? NullableInt32Field { get; set; }
        public UInt32? NullableUInt32Field { get; set; }
        public Int64? NullableInt64Field { get; set; }
        public UInt64? NullableUInt64Field { get; set; }
        public Single? NullableSingleField { get; set; }
        public Double? NullableDoubleField { get; set; }
        public Decimal? NullableDecimalField { get; set; }
        public Boolean? NullableBooleanField { get; set; }
        public DateTime? NullableDateTimeField { get; set; }

        public String StringField { get; set; }

        [NotTrimmed]
        public String NotTrimmedStringField { get; set; }

        [Truncated]
        public DateTime TruncatedDateTimeField { get; set; }

        public List<HttpPostedFileBase> Files { get; set; }

        public AllTypesView Child { get; set; }
    }

    public enum TestEnum
    {
        First,
        Second
    }
}
