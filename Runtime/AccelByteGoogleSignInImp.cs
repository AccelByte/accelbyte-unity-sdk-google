// Copyright (c) 2025 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager
using AccelByte.Core;
using AccelByte.Models;
using System;
using UnityEngine;

namespace AccelByte.ThirdParties.Google
{
    public class AccelByteGoogleSignInImp
    {
        private const string googleSignInClassName = "com.accelbyte.googlesigninlib.AccelByteGoogleSignIn";
        private readonly AndroidJavaObject googleSignInObj;
        private readonly string webClientId;

        public AccelByteGoogleSignInImp(string webClientId)
        {
            AndroidJavaClass googleSignInClass = new AndroidJavaClass(googleSignInClassName);
            googleSignInObj = googleSignInClass.CallStatic<AndroidJavaObject>("getInstance");
            this.webClientId = webClientId;
        }

        ~AccelByteGoogleSignInImp()
        {
            googleSignInObj?.Dispose();
        }

        public AccelByteResult<GoogleSignInResultInternal, Error> GetGoogleIdToken(GetGoogleIdTokenOptionalParametersInternal optionalParams = null)
        {
            var result = new AccelByteResult<GoogleSignInResultInternal, AccelByte.Core.Error>();
            var callbackHandler = new GoogleSignInResultCallback(tokenCredential =>
            {
                result.Resolve(tokenCredential);
            },
            err =>
            {
                result.Reject(new Error(ErrorCode.ErrorFromException, err));
            });

            bool filterByAuthorizedAccounts = true;
            bool autoSelectEnabled = false;
            bool requestServerAuthCode = false;
            string[] scopes = new string[] { };

            if (optionalParams != null)
            {
                if (optionalParams.FilterByAuthorizedAccounts != null)
                {
                    filterByAuthorizedAccounts = optionalParams.FilterByAuthorizedAccounts.Value;
                }
                if (optionalParams.AutoSelectEnabled != null)
                {
                    autoSelectEnabled = optionalParams.AutoSelectEnabled.Value;
                }
                if (!optionalParams.RequestServerAuthCode != null)
                {
                    requestServerAuthCode = optionalParams.RequestServerAuthCode.Value;
                }
#if !UNITY_2023_1_OR_NEWER
                if (optionalParams.Scopes != null && optionalParams.Scopes.Length > 0)
                {
                    scopes = optionalParams.Scopes;
                }
#endif
            }

            using (var currentActivity = GetActivity())
            {
                googleSignInObj.Call("GetGoogleIdToken"
                    , scopes
                    , webClientId
                    , filterByAuthorizedAccounts
                    , autoSelectEnabled
                    , requestServerAuthCode
                    , currentActivity
                    , callbackHandler);
            }

            return result;
        }

        public AccelByteResult<GoogleSignInResultInternal, Error> GetSignInWithGoogle(GetSignInWithGoogleOptionalParametersInternal optionalParams = null)
        {
            var result = new AccelByteResult<GoogleSignInResultInternal, Error>();
            var callbackHandler = new GoogleSignInResultCallback(tokenCredential =>
            {
                result.Resolve(tokenCredential);
            },
            err =>
            {
                result.Reject(new Error(ErrorCode.ErrorFromException, err));
            });

            bool requestServerAuthCode = false;
            string[] scopes = new string[] { };

            if (optionalParams != null)
            {
                if (!optionalParams.RequestServerAuthCode != null)
                {
                    requestServerAuthCode = optionalParams.RequestServerAuthCode.Value;
                }
#if !UNITY_2023_1_OR_NEWER
                if (optionalParams.Scopes != null &&  optionalParams.Scopes.Length > 0)
                {
                    scopes = optionalParams.Scopes;
                }
#endif
            }

            using (var currentActivity = GetActivity())
            {
                googleSignInObj.Call("GetSignInWithGoogle"
                    , scopes
                    , webClientId
                    , requestServerAuthCode
                    , currentActivity
                    , callbackHandler);
            }

            return result;
        }

        public AccelByteResult<Error> SignOut()
        {
            var result = new AccelByteResult<Error>();
            var callbackHandler = new GoogleSignOutCallback(() =>
            {
                result.Resolve();
            },
            err =>
            {
                result.Reject(new Error(ErrorCode.ErrorFromException, err));
            });

            using (var currentActivity = GetActivity())
            {
                googleSignInObj.Call("SignOut"
                    , currentActivity
                    , callbackHandler);
            }

            return result;
        }

        public static AndroidJavaObject GetActivity()
        {
            using (var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                return jc.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }

        private class GoogleSignInResultCallback : AndroidJavaProxy
        {
            Action<GoogleSignInResultInternal> onResultCallback;
            Action<string> onErrorCallback;
            public GoogleSignInResultCallback(Action<GoogleSignInResultInternal> onResultCallback
                , Action<string> onErrorCallback)
                : base("com/accelbyte/googlesigninlib/AccelByteGoogleSignInResultCallback")
            {
                this.onResultCallback = onResultCallback;
                this.onErrorCallback = onErrorCallback;
            }

            public void onError(string e)
            {
                onErrorCallback?.Invoke(e);
            }

            public void onResult(AndroidJavaObject result)
            {
                GoogleSignInResultInternal tokenResult = new GoogleSignInResultInternal();
                if (result is IDisposable)
                {
                    using ((IDisposable)result)
                    {
                        onResultCallback?.Invoke(tokenResult.From(result));
                    }
                }
                else
                {
                    onResultCallback?.Invoke(tokenResult.From(result));
                }
            }
        }

        private class GoogleSignOutCallback : AndroidJavaProxy
        {
            Action onResultCallback;
            Action<string> onErrorCallback;
            public GoogleSignOutCallback(Action onResultCallback
                , Action<string> onErrorCallback)
                : base("com/accelbyte/googlesigninlib/AccelByteGoogleSignOutResultCallback")
            {
                this.onResultCallback = onResultCallback;
                this.onErrorCallback = onErrorCallback;
            }

            public void onError(string e)
            {
                onErrorCallback?.Invoke(e);
            }

            public void onResult()
            {
                onResultCallback?.Invoke();
            }
        }
    }
}
