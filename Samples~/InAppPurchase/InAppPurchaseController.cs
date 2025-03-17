// Copyright (c) 2025 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using AccelByte.Core;
using AccelByte.Google.Samples.SignInWithGoogle;
using UnityEngine;
using UnityEngine.UI;

namespace AccelByte.SdkGooglePlayGames.Samples.InAppPurchase
{
    public class InAppPurchaseController : MonoBehaviour
    {
        public LoginHandler LoginHandler;
        public Button LogoutButton;
        public Button LoginButton;
        public GameObject IAPPanel;
        public GameObject LoginPanel;

        private void Start()
        {
            LoginHandler = new LoginHandler();
            ButtonAssigning();
            LoggedOutState();
        }

        private void ButtonAssigning()
        {
            LoginButton?.onClick.AddListener(Login);
            LogoutButton?.onClick.AddListener(Logout);
        }

        private void Login()
        {
            var loginIEnumerator = LoginHandler.RunAutoLogin((loginResult) =>
            {
                if (loginResult.ResultType == LoginHandler.LoginResultType.Success)
                {
                    Debug.Log($"Login Success");
                    UpdateLoggedInState(loginResult.ResultType);
                }
                else
                {
                    Debug.LogError($"Login Failed: {loginResult.Log}");
                }
            });
            StartCoroutine(loginIEnumerator);
        }

        private void Logout()
        {
            AccelByteSDK.GetClientRegistry().GetApi().GetUser().Logout(logoutResult =>
            {
                if (logoutResult.IsError)
                {
                    Debug.LogWarning($"Failed to Logout [{logoutResult.Error.Code}] : {logoutResult.Error.Message}");
                    return;
                }
                
                LoggedOutState();
                Debug.Log($"Logout Success");
            });
        }

        private void UpdateLoggedInState(LoginHandler.LoginResultType loginResultType)
        {
            if (loginResultType == LoginHandler.LoginResultType.Success)
            {
                LoginPanel.SetActive(false);
                IAPPanel.SetActive(true);
            }
            else
            {
                LoginPanel.SetActive(true);
                IAPPanel.SetActive(false);
            }
        }

        private void LoggedOutState()
        {
            LoginPanel.SetActive(true);
            IAPPanel.SetActive(false);
        }
    }
}