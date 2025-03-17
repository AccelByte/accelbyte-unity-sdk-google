// Copyright (c) 2025 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Collections;
using AccelByte.Core;
using AccelByte.Models;
using AccelByte.ThirdParties.Google;
using UnityEngine;

namespace AccelByte.Google.Samples.SignInWithGoogle
{
    public class LoginHandler
    {
        private AccelByteGoogle accelByteGoogle;
        
        public enum LoginResultType
        {
            None,
            Success,
            Failed
        }

        private LoginResultType currentLoginResultType;

        public LoginResultType GetCurrentLoginResult()
        {
            return currentLoginResultType;
        }

        public LoginHandler()
        {
            accelByteGoogle = new AccelByte.ThirdParties.Google.AccelByteGoogle();
        }

        /// <summary>
        /// Run Google Login first, then run AGS Login
        /// </summary>
        /// <param name="postLoginAction">Trigger something after login is done</param>
        public IEnumerator RunAutoLogin(Action<LoginResult> postLoginAction)
        {
            GetIdTokenResult? getResult = null;
            GetGoogleIdToken(getIdTokenResult =>
            {
                getResult = getIdTokenResult;
            });

            yield return new WaitUntil(() => getResult.HasValue);
            
            if (getResult.Value.IsSuccess)
            {
                AGSLogin(getResult.Value.IdToken, postLoginAction);
            }
            else
            {
                postLoginAction?.Invoke(new LoginResult()
                {
                    ResultType = LoginResultType.Failed,
                    Log = getResult.Value.Log
                });
            }
        }

        /// <summary>
        /// Get Google id token.
        /// It will be utilized to AGS login Later
        /// Note: this function needs to be run in MainThread
        /// </summary>
        private void GetGoogleIdToken(Action<GetIdTokenResult> onDone)
        {
            var opt = new GetGoogleIdTokenOptionalParameters()
            {
                AutoSelectEnabled = true,
                FilterByAuthorizedAccounts = true,
                RequestServerAuthCode = true,
#if !UNITY_2023_1_OR_NEWER
                Scopes = new string[]
                {
                    "profile",
                    "email"
                }
#endif
            };

            GetIdTokenResult retval = new GetIdTokenResult();
            accelByteGoogle.GetGoogleIdToken(opt)
                .OnSuccess(result =>
                {
                    retval.IsSuccess = true;
                    retval.IdToken = result.IdToken;
                    retval.Log = "Obtain Google Id Token Success";
                })
                .OnFailed(result =>
                {
                    retval.IsSuccess = false;
                    retval.Log = $"Obtain Google Id Token Failed: {result.Code} {result.Message}";
                })
                .OnComplete(() =>
                {
                    onDone?.Invoke(retval);
                });
        }

        /// <summary>
        /// Trigger AGS Login. It requires the fetched Google Id Token
        /// </summary>
        /// <param name="idToken">fetched Google Play Games IdToken</param>
        /// <param name="postLoginAction">Trigger something after login is done</param>
        private void AGSLogin(string idToken, Action<LoginResult> postLoginAction)
        {
            if (!string.IsNullOrEmpty(idToken))
            {
                var opt = new LoginWithOtherPlatformV4OptionalParameters()
                {
                    CreateHeadless = false
                };

                AccelByteSDK.GetClientRegistry().GetApi().GetUser().LoginWithOtherPlatformV4(
                    new Models.LoginPlatformType(AccelByte.Models.PlatformType.Google)
                    , idToken
                    , result =>
                    {
                        if (result.IsError)
                        {
                            currentLoginResultType = LoginResultType.Failed;
                            var failedLoginResult = new LoginResult()
                            {
                                ResultType = currentLoginResultType,
                                Log = $"Failed to Login with Google Platfrom [{result.Error.error}]: {result.Error.error_description}"
                            };
                            postLoginAction?.Invoke(failedLoginResult);
                            return;
                        }

                        currentLoginResultType = LoginResultType.Success;
                        var successLoginResult = new LoginResult()
                        {
                            ResultType = currentLoginResultType,
                            Log = "Login with AccelByte IAM success"
                        };
                        postLoginAction?.Invoke(successLoginResult);
                    });
            }
            else
            {
                currentLoginResultType = LoginResultType.Failed;
                var failedLoginResult = new LoginResult()
                {
                    ResultType = currentLoginResultType,
                    Log = "Failed to login with AGS due to missing Google IdToken"
                };
                postLoginAction?.Invoke(failedLoginResult);
            }
        }

        /// <summary>
        /// Trigger AGS Logout
        /// </summary>
        /// <param name="postLogoutAction">Trigger something after logout is done</param>
        public void AGSLogout(Action postLogoutAction)
        {
            AccelByteSDK.GetClientRegistry().GetApi().GetUser().Logout(logoutResult =>
            {
                if (logoutResult.IsError)
                {
                    Debug.LogWarning($"Failed to logout");
                    return;
                }

                currentLoginResultType = LoginResultType.None;
                Debug.Log($"Logout Success");
                postLogoutAction?.Invoke();
            });
        }

        public struct GetIdTokenResult
        {
            public bool IsSuccess;
            public string Log;
            public string IdToken;
        }
    }
}
