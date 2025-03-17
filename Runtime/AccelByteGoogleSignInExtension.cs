// Copyright (c) 2025 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager
using System;
using UnityEngine;

namespace AccelByte.ThirdParties.Google
{
    public static class AccelByteGoogleSignInExtension
    {
        internal static GoogleSignInResultInternal From(this GoogleSignInResultInternal credential
            , AndroidJavaObject googleSignInResultJavaObj)
        {
            if (googleSignInResultJavaObj == null)
            {
                return null;
            }

            AndroidJavaObject idTokenCredential = googleSignInResultJavaObj.Get<AndroidJavaObject>("Credential");
            string displayName = idTokenCredential.Call<String>("getDisplayName");
            string familyName = idTokenCredential.Call<String>("getFamilyName");
            string givenName = idTokenCredential.Call<String>("getGivenName");
            string id = idTokenCredential.Call<String>("getId");
            string idToken = idTokenCredential.Call<String>("getIdToken");
            string phoneNumber = idTokenCredential.Call<String>("getPhoneNumber");
            var profilePictureUriObj = idTokenCredential.Call<AndroidJavaObject>("getProfilePictureUri");
            string profilePictureUriString = idTokenCredential.Call<string>("toString");
            string serverAuthCode = googleSignInResultJavaObj.Get<string>("ServerAuthCode");

            var res = new GoogleSignInResultInternal(
                displayName = displayName,
                familyName = familyName,
                givenName = givenName,
                id = id,
                idToken = idToken,
                phoneNumber = phoneNumber,
                profilePictureUriString = profilePictureUriString,
                serverAuthCode = serverAuthCode
            );
            return res;
        }
    }
}

