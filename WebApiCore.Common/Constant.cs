using System;

namespace WebApiCore.Common
{
    public static class Constant
    {
        public const string ConnectionString = "DefaultConnection";
        public const int MaximumItem = 100;
        public const int Salt = 16;
        public const int Hash = 20;
        public const int PasswordHash = 36;
        public const int AccessFailedCount = 3;

        public class BaseProperty
        {
            public const string Id = "Id";
            public const string CreatedDate = "CreatedDate";
            public const string CreatedBy = "CreatedBy";
            public const string ModifiedDate = "ModifiedDate";
            public const string ModifiedBy = "ModifiedBy";
            public const string StatusId = "StatusId";
        }

        public class AuditTrailProperty : BaseProperty
        {
            public const string ItemId = "ItemId";
            public const string TableName = "TableName";
            public const string TrackChange = "TrackChange";
            public const string TransactionId = "TransactionId";
        }
    }
}
