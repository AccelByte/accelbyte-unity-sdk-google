// Copyright (c) 2025 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.
namespace AccelByte.ThirdParties.Google
{
    public class GoogleSignInResultInternal
    {
        public readonly string DisplayName;
        public readonly string FamilyName;
        public readonly string GivenName;
        public readonly string Id;
        public readonly string IdToken;
        public readonly string PhoneNumber;
        public readonly string ProfilePictureUri;
        public readonly string ServerAuthCode;

        public GoogleSignInResultInternal()
        {
            
        }
        
        public GoogleSignInResultInternal(string displayName, string familyName, string givenName, string id, string idToken, string phoneNumber, string profilePictureUri, string serverAuthCode)
        {
            DisplayName = displayName;
            FamilyName = familyName;
            GivenName = givenName;
            Id = id;
            IdToken = idToken;
            PhoneNumber = phoneNumber;
            ProfilePictureUri = profilePictureUri;
            ServerAuthCode = serverAuthCode;
        }
    }
}