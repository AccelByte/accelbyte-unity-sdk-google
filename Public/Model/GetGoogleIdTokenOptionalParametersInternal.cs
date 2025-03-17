// Copyright (c) 2025 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.
namespace AccelByte.ThirdParties.Google
{
    public class GetGoogleIdTokenOptionalParametersInternal
    {
        public bool? AutoSelectEnabled;
        public bool? FilterByAuthorizedAccounts;
        public bool? RequestServerAuthCode;
#if !UNITY_2023_1_OR_NEWER
        public string[] Scopes;
#endif
    }
}