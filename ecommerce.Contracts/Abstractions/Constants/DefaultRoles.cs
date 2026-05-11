using System;

namespace ecommerce.Contracts.Abstractions.Constants;

public static class DefaultRoles
{
    public const string Admin = nameof(Admin);
    public const int AdminRoleId = 1;
    public const string AdminRoleConcurrencyStamp = "c3261412-397e-415b-8ce3-49f01af7ca55";

    public const string Merchant = nameof(Merchant);
    public const int MerchantRoleId = 2;
    public const string MerchantRoleConcurrencyStamp = "d9bd616e-1f7a-41f0-8742-a69418ea78e8";

    public const string Corperate = nameof(Corperate);
    public const int CorperateRoleId = 3;
    public const string CorperateRoleConcurrencyStamp = "1a16f412-bc6e-4e9a-9ae7-0716ee104151";

    public const string Receiver = nameof(Receiver);
    public const int ReceiverRoleId = 4;
    public const string ReceiverRoleConcurrencyStamp = "1aef7a54-7061-4a9e-b076-6cc910b4cd83";

    public const string Customer = nameof(Customer);
    public const int CustomerRoleId = 5;
    public const string CustomerRoleConcurrencyStamp = "67693004-4603-49fb-b99d-3f47620d76cf";

}
